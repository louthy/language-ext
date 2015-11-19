using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using static LanguageExt.Prelude;
using System.Threading;

namespace LanguageExt
{
    static class ActorContext
    {
        [ThreadStatic]
        static ActorRequestContext context;

        static object sync = new object();
        static volatile bool started = false;
        static Option<ICluster> cluster;
        static ActorItem rootItem;
        static ActorRequestContext userContext;

        static ActorContext()
        {
            Startup(None);
        }

        public static Unit Startup(Option<ICluster> cluster)
        {
            if (started) return unit;

            ActorContext.cluster = cluster;
            var name = GetRootProcessName();
            if (name.Value == "root" && cluster.IsSome) throw new ArgumentException("Cluster node name cannot be 'root', it's reserved for local use only.");
            if (name.Value == "registered") throw new ArgumentException("Node name cannot be 'registered', it's reserved for registered processes.");
            if (name.Value == "js") throw new ArgumentException("Node name cannot be 'js', it's reserved for ProcessJS.");

            lock (sync)
            {
                if (started) return unit;
                var root = ProcessId.Top.Child(name);
                var rootInbox = new ActorInboxLocal<ActorSystemState, Unit>();
                var parent = new ActorItem(new NullProcess(), new NullInbox(), ProcessFlags.Default);

                var go = new AutoResetEvent(false);
                var state = new ActorSystemState(cluster, root, null, rootInbox, cluster.Map(x => x.NodeName).IfNone(ActorConfig.Default.RootProcessName), ActorConfig.Default);
                var rootProcess = state.RootProcess;
                state.Startup();
                userContext = new ActorRequestContext(rootProcess.Children["user"], ProcessId.NoSender, rootItem, null, null, ProcessFlags.Default);
                rootInbox.Startup(rootProcess, parent, cluster, ProcessSetting.DefaultMailboxSize);
                rootItem = new ActorItem(rootProcess, rootInbox, ProcessFlags.Default);
                started = true;

                SessionManager.Init(cluster);
            }
            return unit;
        }

        public static Unit Shutdown()
        {
            lock (sync)
            {
                if (rootItem != null)
                {
                    var item = rootItem;
                    rootItem = null;
                    item.Actor.ShutdownProcess(true);
                    started = false;
                    Startup(None);
                }
            }
            return unit;
        }

        public static Unit Restart()
        {
            var saved = cluster;
            Shutdown();
            Startup(saved);
            return unit;
        }

        public static Option<ICluster> Cluster => 
            cluster;

        public static IEnumerable<T> AskMany<T>(IEnumerable<ProcessId> pids, object message, int take)
        {
            take = Math.Min(take, pids.Count());

            var handle = new CountdownEvent(take);

            var responses = new List<AskActorRes>();
            foreach (var pid in pids)
            {
                var req = new AskActorReq(
                    message, 
                    res =>
                    {
                        responses.Add(res);
                        handle.Signal();

                    }, pid, Self);
                Process.tell(AskId, req);
            }

            handle.Wait(ActorConfig.Default.Timeout);

            return responses.Where(r => !r.IsFaulted).Map(r => (T)r.Response);
        }

        public static T Ask<T>(ProcessId pid, object message)
        {
            if (false) //Process.InMessageLoop)
            {
                //return SelfProcess.Actor.ProcessRequest<T>(pid, message);
            }
            else
            {
                AskActorRes response = null;
                var handle = new AutoResetEvent(false);

                var req = new AskActorReq(
                    message,
                    res =>
                    {
                        response = res;
                        handle.Set();
                    },
                    pid,
                    Self
                );

                var askItem = GetAskItem();

                askItem.IfSome(
                    ask =>
                    {
                        var inbox = ask.Inbox as ILocalActorInbox;
                        inbox.Tell(req, Self);
                        handle.WaitOne(ActorConfig.Default.Timeout);
                    });

                if (askItem)
                {
                    if (response == null)
                    {
                        throw new TimeoutException("Request timed out");
                    }
                    else
                    {
                        if (response.IsFaulted)
                        {
                            throw response.Exception;
                        }
                        else
                        {
                            return (T)response.Response;
                        }
                    }
                }
                else
                {
                    throw new Exception("Ask process doesn't exist");
                }
            }
        }

        public static ProcessName GetRootProcessName() =>
            cluster.Map(x => x.NodeName)
                   .IfNone(ActorConfig.Default.RootProcessName);

        public static Unit RegisterCluster(ICluster cluster)
        {
            started = false;
            Startup(Some(cluster));
            return unit;
        }

        public static Unit DeregisterCluster()
        {
            cluster.IfSome(c =>
            {
                c.Disconnect();
                c.Dispose();
            });
            started = false;
            return Startup(None);
        }

        public static ProcessId ActorCreate<S, T>(
            ActorItem parent, 
            ProcessName name, 
            Func<T, Unit> actorFn, 
            State<StrategyContext, Unit> strategy, 
            ProcessFlags flags, 
            int maxMailboxSize, 
            bool lazy
            ) =>
            ActorCreate<S, T>(parent, name, (s, t) => { actorFn(t); return default(S); }, () => default(S), strategy, flags, maxMailboxSize, lazy);

        public static ProcessId ActorCreate<S, T>(
            ActorItem parent, 
            ProcessName name, 
            Action<T> actorFn, 
            State<StrategyContext, Unit> strategy, 
            ProcessFlags flags, 
            int maxMailboxSize, 
            bool lazy
            ) =>
            ActorCreate<S, T>(parent, name, (s, t) => { actorFn(t); return default(S); }, () => default(S), strategy, flags, maxMailboxSize, lazy);

        public static ProcessId ActorCreate<S, T>(
            ActorItem parent, 
            ProcessName name, 
            Func<S, T, S> actorFn, 
            Func<S> setupFn, 
            State<StrategyContext, Unit> strategy, 
            ProcessFlags flags, 
            int maxMailboxSize, 
            bool lazy
            ) =>
            ActorCreate<S, T>(parent, name, actorFn, _ => setupFn(), strategy, flags, maxMailboxSize, lazy);

        public static ProcessId ActorCreate<S, T>(
            ActorItem parent, 
            ProcessName name, 
            Func<S, T, S> actorFn, 
            Func<IActor, S> setupFn, 
            State<StrategyContext, Unit> strategy, 
            ProcessFlags flags, 
            int maxMailboxSize, 
            bool lazy)
        {
            var actor = new Actor<S, T>(cluster, parent, name, actorFn, setupFn, strategy, flags);

            IActorInbox inbox = null;
            if ((flags & ProcessFlags.ListenRemoteAndLocal) == ProcessFlags.ListenRemoteAndLocal && cluster.IsSome)
            {
                inbox = new ActorInboxDual<S, T>();
            }
            else if ((flags & ProcessFlags.PersistInbox) == ProcessFlags.PersistInbox && cluster.IsSome)
            {
                inbox = new ActorInboxRemote<S, T>();
            }
            else
            {
                inbox = new ActorInboxLocal<S, T>();
            }

            var item = new ActorItem(actor, inbox, flags);

            parent.Actor.LinkChild(item);

            try
            {
                inbox.Startup(actor, parent, cluster, maxMailboxSize);
                if (!lazy)
                {
                    TellSystem(actor.Id, SystemMessage.StartupProcess);
                }
            }
            catch
            {
                item?.Actor?.ShutdownProcess(false);
                throw;
            }
            return item.Actor.Id;
        }

        public static ILocalActorInbox LocalRoot => 
            (ILocalActorInbox)rootItem.Inbox;

        public static IActorInbox RootInbox =>
            rootItem.Inbox;

        public static ProcessId Root =>
            rootItem.Actor.Id;

        public static ActorRequestContext Context
        {
            get
            {
                if (context == null)
                {
                    Startup(cluster);
                    return userContext;
                }
                else
                {
                    return context;
                }
            }
        }

        public static ProcessId Self =>
            Context.Self != null
                ? Context.Self.Actor.Id
                : User;

        public static ActorItem SelfProcess =>
            Context.Self;

        public static ProcessId Sender =>
            Context.Sender;

        public static ProcessId Parent =>
            Context.Self.Actor.Parent.Actor.Id;

        public static ProcessId System =>
            Root[ActorConfig.Default.SystemProcessName];

        public static ProcessId User =>
            Root[ActorConfig.Default.UserProcessName];

        public static ProcessId Registered =>
            Root[ActorConfig.Default.RegisteredProcessName];

        public static ProcessId Errors =>
            System[ActorConfig.Default.ErrorsProcessName];

        public static ProcessId DeadLetters =>
            System[ActorConfig.Default.DeadLettersProcessName];

        public static Map<string, ProcessId> Children =>
            Context.Self.Actor.Children.Map(c => c.Actor.Id);

        private static ProcessName NodeName =>
            cluster.Map(c => c.NodeName).IfNone("user");

        internal static ProcessId AskId =>
            System[ActorConfig.Default.AskProcessName];

        public static ActorRequest CurrentRequest
        {
            get
            {
                return Context.CurrentRequest;
            }
            set
            {
                context = Context.SetCurrentRequest(value);
            }
        }

        public static object CurrentMsg
        {
            get
            {
                return Context.CurrentMsg;
            }
            set
            {
                context = Context.SetCurrentMessage(value);
            }
        }

        public static ProcessFlags ProcessFlags
        {
            get
            {
                return Context.ProcessFlags;
            }
            set
            {
                context = Context.SetProcessFlags(value);
            }
        }

        static Option<ActorItem> GetJsItem()
        {
            var children = rootItem?.Actor?.Children;
            if (notnull(children) && children.ContainsKey("js"))
            {
                return Some(children["js"]);
            }
            else
            {
                return None;
            }
        }

        static Option<ActorItem> GetRegisteredItem()
        {
            var children = rootItem?.Actor?.Children;
            if (notnull(children) && children.ContainsKey(ActorConfig.Default.RegisteredProcessName.Value))
            {
                return Some(children[ActorConfig.Default.RegisteredProcessName.Value]);
            }
            else
            {
                return None;
            }
        }

        static Option<ActorItem> GetAskItem()
        {
            var children = rootItem?.Actor?.Children;
            if (notnull(children) && children.ContainsKey(ActorConfig.Default.SystemProcessName.Value))
            {
                var sys = children[ActorConfig.Default.SystemProcessName.Value];
                children = sys.Actor.Children;
                if (children.ContainsKey(ActorConfig.Default.AskProcessName.Value))
                {
                    return Some(children[ActorConfig.Default.AskProcessName.Value]);
                }
                else
                {
                    return None;
                }
            }
            else
            {
                return None;
            }
        }

        public static Option<ActorItem> GetInboxShutdownItem()
        {
            var children = rootItem?.Actor?.Children;
            if (notnull(children) && children.ContainsKey(ActorConfig.Default.SystemProcessName.Value))
            {
                var sys = children[ActorConfig.Default.SystemProcessName.Value];
                children = sys.Actor.Children;
                if (children.ContainsKey(ActorConfig.Default.InboxShutdownProcessName.Value))
                {
                    return Some(children[ActorConfig.Default.InboxShutdownProcessName.Value]);
                }
                else
                {
                    return None;
                }
            }
            else
            {
                return None;
            }
        }

        public static ProcessId Register<T>(ProcessName name, ProcessId processId, ProcessFlags flags, int maxMailboxSize) =>
            processId.IsValid
                ? GetRegisteredItem().Match(
                     Some: regd =>
                        ActorCreate<ProcessId, T>(
                            regd,
                            name,
                            RegisteredActor<T>.Inbox,
                            () => processId,
                            Process.DefaultStrategy,
                            flags,
                            maxMailboxSize,
                            true
                        ),
                     None: () => ProcessId.None)
                : ProcessId.None;

        public static Unit Deregister(ProcessName name) =>
            Process.kill(Registered.Child(name));

        public static R WithContext<R>(ActorItem self, ActorItem parent, ProcessId sender, ActorRequest request, object msg, Option<string> sessionId, Func<R> f)
        {
            var savedContext = context;
            var savedSession = SessionManager.SessionId;

            try
            {
                SessionManager.SessionId = sessionId;
                context = new ActorRequestContext(
                    self,
                    sender,
                    parent,
                    msg,
                    request,
                    ProcessFlags.Default
                );
                return f();
            }
            finally
            {
                SessionManager.SessionId = savedSession;
                context = savedContext;
            }
        }

        public static Unit WithContext(ActorItem self, ActorItem parent, ProcessId sender, ActorRequest request, object msg, Option<string> sessionId, Action f) =>
            WithContext(self, parent, sender, request, msg, sessionId, fun(f));

        public static Unit Publish(object message) =>
            SelfProcess.Actor.Publish(message);

        internal static IObservable<T> Observe<T>(ProcessId pid) =>
            GetDispatcher(pid).Observe<T>();

        internal static IObservable<T> ObserveState<T>(ProcessId pid) =>
            GetDispatcher(pid).ObserveState<T>();

        internal static ProcessId SenderOrDefault(ProcessId sender) =>
            sender.IsValid
                ? sender
                : Self;

        internal static IActorDispatch GetRegisteredDispatcher(ProcessId pid) =>
            GetRegisteredItem().Match(
                Some: regd =>
                    pid.Count() == 2
                        ? new ActorDispatchLocal(regd)
                        : regd.Actor.Children.ContainsKey(pid.Skip(2).GetName().Value)
                                ? GetDispatcher(pid.Tail(), rootItem, pid)
                                : cluster.Match<IActorDispatch>(
                                    Some: c  => new ActorDispatchRemote(pid.Skip(1), c),
                                    None: () => new ActorDispatchNotExist(pid)),
                None: () => new ActorDispatchNotExist(pid));

        internal static IActorDispatch GetJsDispatcher(ProcessId pid)
        {
            switch (pid.Count())
            {
                case 0:
                case 1:
                    return new ActorDispatchNotExist(pid);

                //  /root/js                            <-- relay
                case 2:
                    return GetDispatcher(pid.Skip(1), rootItem, pid);

                //  /root/js/{connection id}            <-- relay
                case 3:
                    return GetDispatcher(pid.Skip(1), rootItem, pid);

                // /root/js/{connection id}/js-root/..  --> back to JS
                default:
                    return new ActorDispatchJS(pid, pid.Take(3), rootItem.Actor.Children["js"]);
            }
        }

        internal static IActorDispatch GetLocalDispatcher(ProcessId pid) =>
            pid.Take(2) == Root["js"]
                ? GetJsDispatcher(pid)
                : GetDispatcher(pid.Tail(), rootItem, pid);

        internal static IActorDispatch GetRemoteDispatcher(ProcessId pid) =>
            cluster.Match<IActorDispatch>(
                Some: c  => new ActorDispatchRemote(pid, c),
                None: () => new ActorDispatchNotExist(pid));

        internal static bool IsRegistered(ProcessId pid) =>
            pid.Take(2).GetName().Value == ActorConfig.Default.RegisteredProcessName.Value;

        internal static bool IsLocal(ProcessId pid) =>
            pid.Head() == Root;

        internal static IActorDispatch GetDispatcher(ProcessId pid) =>
            pid.IsValid
                ? IsRegistered(pid)
                    ? GetRegisteredDispatcher(pid)
                    : IsLocal(pid)
                        ? GetLocalDispatcher(pid)
                        : GetRemoteDispatcher(pid)
                : new ActorDispatchNotExist(pid);

        static IActorDispatch GetDispatcher(ProcessId pid, ActorItem current, ProcessId orig)
        {
            if (pid == ProcessId.Top)
            {
                if (current.Inbox is ILocalActorInbox)
                {
                    return new ActorDispatchLocal(current);
                }
                else
                {
                    return cluster.Match<IActorDispatch>(
                            Some: c => new ActorDispatchRemote(orig, c),
                            None: () => new ActorDispatchNotExist(orig));
                }
            }
            else
            {
                var child = pid.Head().GetName().Value;
                if (current.Actor.Children.ContainsKey(child))
                {
                    var process = current.Actor.Children[child];
                    return GetDispatcher(pid.Tail(), process, orig);
                }
                else
                {
                    return new ActorDispatchNotExist(orig);
                }
            }
        }

        public static Unit Ask(ProcessId pid, object message, ProcessId sender) =>
            GetDispatcher(pid).Ask(message, sender);

        public static Unit Tell(ProcessId pid, object message, ProcessId sender) =>
            GetDispatcher(pid).Tell(message, sender, message is ActorRequest ? Message.TagSpec.UserAsk : Message.TagSpec.User);

        public static Unit TellUserControl(ProcessId pid, UserControlMessage message) =>
            GetDispatcher(pid).TellUserControl(message, Self);

        public static Unit TellSystem(ProcessId pid, SystemMessage message) =>
            GetDispatcher(pid).TellSystem(message, Self);

        public static Map<string, ProcessId> GetChildren(ProcessId pid) =>
            GetDispatcher(pid).GetChildren();

        public static Unit Kill(ProcessId pid) =>
            GetDispatcher(pid).Kill();
    }
}
