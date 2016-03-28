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

        static ProcessSystemConfig config;
        static object sync = new object();
        static volatile bool started = false;
        static Option<ICluster> cluster;
        static ActorItem rootItem;
        static ActorRequestContext userContext;
        static ClusterMonitor.State clusterState;
        static long startupTimestamp;
        static IDisposable startupSubscription;
        static Map<ProcessName, Func<ProcessId, IEnumerable<ProcessId>>> dispatchers = Map.empty<ProcessName, Func<ProcessId, IEnumerable<ProcessId>>>();

        static Map<ProcessId, Set<ProcessId>> watchers;
        static Map<ProcessId, Set<ProcessId>> watchings;

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
            if (name.Value == "disp" && cluster.IsSome) throw new ArgumentException("Cluster node name cannot be 'disp', it's reserved for internal use.");
            if (name.Value == "js") throw new ArgumentException("Node name cannot be 'js', it's reserved for ProcessJS.");

            lock (sync)
            {
                if (started) return unit;

                config = config ?? new ProcessSystemConfig(null);

                startupTimestamp = DateTime.UtcNow.Ticks;
                startupSubscription?.Dispose();
                startupSubscription = NotifyCluster(cluster, startupTimestamp);

                Dispatch.init();
                Role.init(cluster.Map(r => r.Role).IfNone("local"));
                Reg.init();

                watchers = Map.empty<ProcessId, Set<ProcessId>>();
                watchings = Map.empty<ProcessId, Set<ProcessId>>();
                var root = ProcessId.Top.Child(name);
                var rootInbox = new ActorInboxLocal<ActorSystemState, Unit>();
                var parent = new ActorItem(new NullProcess(), new NullInbox(), ProcessFlags.Default);

                var go = new AutoResetEvent(false);
                var state = new ActorSystemState(cluster, root, null, rootInbox, cluster.Map(x => x.NodeName).IfNone(ActorSystemConfig.Default.RootProcessName), ActorSystemConfig.Default);
                var rootProcess = state.RootProcess;
                state.Startup();
                userContext = new ActorRequestContext(rootProcess.Children["user"], ProcessId.NoSender, rootItem, null, null, ProcessFlags.Default);
                rootInbox.Startup(rootProcess, parent, cluster, config.GetProcessMailboxSize(rootProcess.Id));
                rootItem = new ActorItem(rootProcess, rootInbox, ProcessFlags.Default);
                started = true;

                SessionManager.Init(cluster);
                ClusterWatch(cluster);
            }
            return unit;
        }

        public static ProcessSystemConfig Config =>
            config;

        public static void SetConfig(ProcessSystemConfig mgr) =>
            config = mgr ?? new ProcessSystemConfig(null);

        public static ProcessId AddDispatcher(ProcessName name, Func<ProcessId, IEnumerable<ProcessId>> selector)
        {
            lock(sync)
            {
                dispatchers = dispatchers.AddOrUpdate(name, selector);
            }
            return ProcessId.Top["disp"][name];
        }

        public static Unit RemoveDispatcher(ProcessName name)
        {
            lock (sync)
            {
                dispatchers = dispatchers.Remove(name);
            }
            return unit;
        }

        public static Func<ProcessId, IEnumerable<ProcessId>> GetDispatcherFunc(ProcessName name) =>
            dispatchers.Find(name,
                Some: x => x,
                None: () => (ProcessId Id) => (new ProcessId[0]).AsEnumerable()
                );

        const string ClusterOnlineKey = "cluster-node-online";
        class ClusterOnline
        {
            public string Name;
            public long Timestamp;
        }

        static IDisposable NotifyCluster(Option<ICluster> cluster, long timestamp) =>
            cluster.Map(c =>
            {
                c.PublishToChannel(ClusterOnlineKey, new ClusterOnline { Name = c.NodeName.Value, Timestamp = timestamp });
                return c.SubscribeToChannel<ClusterOnline>(ClusterOnlineKey)
                        .Where( node => node.Name == c.NodeName.Value && node.Timestamp > timestamp )
                        .Take(1)
                        .Subscribe( _ =>
                        {
                            var cancel = new CancelShutdown();
                            try
                            {
                                Process.OnPreShutdown(cancel);
                            }
                            finally
                            {
                                if (!cancel.Cancelled)
                                {
                                    Process.shutdownAll();
                                    c.Disconnect();
                                }
                            }
                        });
            })
           .IfNone(() => Observable.Return(new ClusterOnline()).Take(1).Subscribe());

        public static ClusterMonitor.State ClusterState => 
            clusterState;

        private static void ClusterWatch(Option<ICluster> cluster)
        {
            var monitor = System[ActorSystemConfig.Default.MonitorProcessName];

            cluster.IfSome(c =>
            {
                Process.observe<NodeOnline>(monitor).Subscribe(x =>
                {
                    Process.logInfo("Online: " + x.Name);
                });

                Process.observe<NodeOffline>(monitor).Subscribe(x =>
                {
                    Process.logInfo("Offline: " + x.Name);
                    RemoveWatchingOfRemote(x.Name);
                });

                Process.observeState<ClusterMonitor.State>(monitor).Subscribe(x =>
                {
                    clusterState = x;
                });
            });

            Process.tell(monitor, new ClusterMonitor.Msg(ClusterMonitor.MsgTag.Heartbeat));
        }

        public static Unit Shutdown()
        {
            lock (sync)
            {
                if (rootItem != null)
                {
                    startupSubscription.Dispose();
                    startupSubscription = null;
                    var item = rootItem;
                    rootItem = null;
                    item.Actor.ShutdownProcess(true);
                    started = false;
                    Process.OnShutdown();
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

        private static Unit RemoveWatchingOfRemote(ProcessName node)
        {
            var root = ProcessId.Top[node];

            foreach (var watching in watchings)
            {
                if (watching.Key.Take(1) == root)
                {
                    RemoteDispatchTerminate(watching.Key);
                }
            }
            return unit;
        }

        public static Unit AddWatcher(ProcessId watcher, ProcessId watching)
        {
            Process.logInfo(watcher + " is watching " + watching);

            lock(sync)
            {
                watchers = watchers.AddOrUpdate(watching,
                    Some: set => set.AddOrUpdate(watcher),
                    None: ()  => Set(watcher)
                );

                watchings = watchings.AddOrUpdate(watcher,
                    Some: set => set.AddOrUpdate(watching),
                    None: ()  => Set(watching)
                );
            }
            return unit;
        }

        public static Unit RemoveWatcher(ProcessId watcher, ProcessId watching)
        {
            Process.logInfo(watcher + " stopped watching " + watching);

            lock (sync)
            {
                watchers = watchers.AddOrUpdate(watching,
                    Some: set => set.Remove(watcher),
                    None: ()  => Set.empty<ProcessId>()
                );

                if (watchers[watching].IsEmpty)
                {
                    watchers = watchers.Remove(watching);
                }

                watchings = watchings.AddOrUpdate(watcher,
                    Some: set => set.Remove(watching),
                    None: ()  => Set.empty<ProcessId>()
                );

                if (watchings[watcher].IsEmpty)
                {
                    watchers = watchers.Remove(watcher);
                }

            }
            return unit;
        }

        static Unit RemoteDispatchTerminate(ProcessId terminating)
        {
            watchings.Find(terminating).IfSome(ws =>
            {
                var term = new TerminatedMessage(terminating);
                ws.Iter(w =>
                {
                    try
                    {
                        TellUserControl(w, term);
                    }
                    catch (Exception e)
                    {
                        Process.logErr(e);
                    }
                });
            });
            watchings.Remove(terminating);
            watchers = watchers.Remove(terminating);

            return unit;
        }

        public static Unit DispatchTerminate(ProcessId terminating)
        {
            watchers.Find(terminating).IfSome(ws =>
            {
                var term = new TerminatedMessage(terminating);
                ws.Iter(w =>
                {
                    try
                    {
                        TellUserControl(w, term);
                    }
                    catch (Exception e)
                    {
                        Process.logErr(e);
                    }
                });
            });

            watchers = watchers.Remove(terminating);

            watchings.Find(terminating).IfSome(ws => ws.Iter(w => GetDispatcher(w).UnWatch(terminating)));
            watchings.Remove(terminating);

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

            handle.Wait(ActorContext.Config.Timeout);

            return responses.Where(r => !r.IsFaulted).Map(r => (T)r.Response);
        }

        public static T Ask<T>(ProcessId pid, object message)
        {
            return (T)Ask(pid, message);
        }

        public static object Ask(ProcessId pid, object message)
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
                        handle.WaitOne(ActorContext.Config.Timeout);
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
                            return response.Response;
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
                   .IfNone(ActorSystemConfig.Default.RootProcessName);

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
            Func<S, ProcessId, S> termFn,
            State<StrategyContext, Unit> strategy, 
            ProcessFlags flags, 
            int maxMailboxSize, 
            bool lazy
            ) =>
            ActorCreate<S, T>(parent, name, (s, t) => { actorFn(t); return default(S); }, () => default(S), termFn, strategy, flags, maxMailboxSize, lazy);

        public static ProcessId ActorCreate<S, T>(
            ActorItem parent, 
            ProcessName name, 
            Action<T> actorFn,
            Func<S, ProcessId, S> termFn,
            State<StrategyContext, Unit> strategy, 
            ProcessFlags flags, 
            int maxMailboxSize, 
            bool lazy
            ) =>
            ActorCreate<S, T>(parent, name, (s, t) => { actorFn(t); return default(S); }, () => default(S), termFn, strategy, flags, maxMailboxSize, lazy);

        public static ProcessId ActorCreate<S, T>(
            ActorItem parent, 
            ProcessName name, 
            Func<S, T, S> actorFn, 
            Func<S> setupFn,
            Func<S, ProcessId, S> termFn,
            State<StrategyContext, Unit> strategy, 
            ProcessFlags flags, 
            int maxMailboxSize, 
            bool lazy
            ) =>
            ActorCreate(parent, name, actorFn, _ => setupFn(), termFn, strategy, flags, maxMailboxSize, lazy);

        public static ProcessId ActorCreate<S, T>(
            ActorItem parent, 
            ProcessName name, 
            Func<S, T, S> actorFn, 
            Func<IActor, S> setupFn,
            Func<S, ProcessId, S> termFn,
            State<StrategyContext, Unit> strategy, 
            ProcessFlags flags, 
            int maxMailboxSize, 
            bool lazy)
        {
            var actor = new Actor<S, T>(cluster, parent, name, actorFn, setupFn, termFn, strategy, flags);

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

        public static readonly ProcessId Disp =
            ProcessId.Top["disp"];

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
            Root[ActorSystemConfig.Default.SystemProcessName];

        public static ProcessId User =>
            Root[ActorSystemConfig.Default.UserProcessName];

        public static ProcessId Errors =>
            System[ActorSystemConfig.Default.ErrorsProcessName];

        public static ProcessId DeadLetters =>
            System[ActorSystemConfig.Default.DeadLettersProcessName];

        public static Map<string, ProcessId> Children =>
            Context.Self.Actor.Children.Map(c => c.Actor.Id);

        private static ProcessName NodeName =>
            cluster.Map(c => c.NodeName).IfNone("user");

        internal static ProcessId AskId =>
            System[ActorSystemConfig.Default.AskProcessName];

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

        static Option<ActorItem> GetAskItem()
        {
            var children = rootItem?.Actor?.Children;
            if (notnull(children) && children.ContainsKey(ActorSystemConfig.Default.SystemProcessName.Value))
            {
                var sys = children[ActorSystemConfig.Default.SystemProcessName.Value];
                children = sys.Actor.Children;
                if (children.ContainsKey(ActorSystemConfig.Default.AskProcessName.Value))
                {
                    return Some(children[ActorSystemConfig.Default.AskProcessName.Value]);
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
            if (notnull(children) && children.ContainsKey(ActorSystemConfig.Default.SystemProcessName.Value))
            {
                var sys = children[ActorSystemConfig.Default.SystemProcessName.Value];
                children = sys.Actor.Children;
                if (children.ContainsKey(ActorSystemConfig.Default.InboxShutdownProcessName.Value))
                {
                    return Some(children[ActorSystemConfig.Default.InboxShutdownProcessName.Value]);
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

        static object regsync = new object();
        static Map<ProcessName, Set<ProcessId>> registeredProcessNames = Map<ProcessName, Set<ProcessId>>();
        static Map<ProcessId, Set<ProcessName>> registeredProcessIds   = Map<ProcessId, Set<ProcessName>>();

        public static Set<ProcessId> GetLocalRegistered(ProcessName name) =>
            registeredProcessNames
                .Find(name)
                .IfNone(Set.empty<ProcessId>());

        public static ProcessId Register(ProcessName name, ProcessId pid)
        {
            if( !pid.IsValid )
            {
                throw new InvalidProcessIdException();
            }

            Cluster.Match(
                Some: c =>
                {
                    if (IsLocal(pid) && GetDispatcher(pid).IsLocal)
                    {
                        AddLocalRegistered(name, pid);
                    }
                    else
                    {
                        // TODO - Make this transactional
                        // {
                        c.SetAddOrUpdate(ProcessId.Top["__registered"][name].Path, pid.Path);
                        c.SetAddOrUpdate(pid.Path + "-registered", name.Value);
                        // }
                    }
                },
                None: () => AddLocalRegistered(name, pid)
            );
            return Disp["reg"][name];
        }

        static void AddLocalRegistered(ProcessName name, ProcessId pid)
        {
            lock (regsync)
            {
                registeredProcessNames = registeredProcessNames.AddOrUpdate(name,
                    Some: set => set.AddOrUpdate(pid),
                    None: ()  => Set(pid)
                );
                registeredProcessIds = registeredProcessIds.AddOrUpdate(pid,
                    Some: set => set.AddOrUpdate(name),
                    None: ()  => Set(name)
                );
            }
        }

        public static Unit DeregisterByName(ProcessName name)
        {
            Cluster.Match(
                Some: c =>
                {
                    RemoveLocalRegisteredByName(name);
                    var regpath = (ProcessId.Top["__registered"][name]).Path;

                    // TODO - Make this transactional
                    // {
                    var pids = c.GetSet<string>(regpath);
                    pids.Iter(pid =>
                    {
                        c.SetRemove(pid + "-registered", name.Value);
                    });
                    c.Delete(regpath);
                    // }
                },
                None: () =>
                {
                    RemoveLocalRegisteredByName(name);
                }
            );
            return unit;
        }

        public static Unit DeregisterById(ProcessId pid)
        {
            if (!pid.IsValid) throw new InvalidProcessIdException();
            if (pid.Take(2) == Disp["reg"])
            {
                throw new InvalidProcessIdException(@"
When de-registering a Process, you should use its actual ProcessId, not its registered
ProcessId.  Multiple Processes can be registered with the same name, and therefore share
the same registered ProcessId.  The de-register system can only know for sure which Process
to de-register if you pass in the actual ProcessId.  If you want to deregister all Processes
by name then use Process.deregisterByName(name).");
            }

            Cluster.Match(
                Some: c =>
                {
                    if (IsLocal(pid) && GetDispatcher(pid).IsLocal)
                    {
                        RemoveLocalRegisteredById(pid);
                    }
                    else
                    {
                        var path = pid.Path;
                        var regpath = path + "-registered";

                        // TODO - Make this transactional
                        // {
                        var names = c.GetSet<string>(regpath);
                        names.Iter(name =>
                        {
                           c.SetRemove(ProcessId.Top["__registered"][name].Path, path);
                        });
                        c.Delete(regpath);
                        // }
                    }
                },
                None: () =>
                {
                    RemoveLocalRegisteredById(pid);
                }
            );
            return unit;
        }

        static void RemoveLocalRegisteredById(ProcessId pid)
        {
            lock (regsync)
            {
                var names = registeredProcessIds.Find(pid).IfNone(Set.empty<ProcessName>());
                names.Iter(name =>
                    registeredProcessNames = registeredProcessNames.SetItem(name, registeredProcessNames[name].Remove(pid))
                );
                registeredProcessIds = registeredProcessIds.Remove(pid);
            }
        }

        static void RemoveLocalRegisteredByName(ProcessName name)
        {
            lock (regsync)
            {
                var pids = registeredProcessNames.Find(name).IfNone(Set.empty<ProcessId>());

                pids.Iter(pid =>
                    registeredProcessIds = registeredProcessIds.SetItem(pid, registeredProcessIds[pid].Remove(name))
                );
                registeredProcessNames = registeredProcessNames.Remove(name);
            }
        }

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

        internal static Option<Func<ProcessId, IEnumerable<ProcessId>>> GetProcessSelector(ProcessId pid)
        {
            if (pid.Count() < 3) throw new InvalidProcessIdException("Invalid role Process ID");
            var type = pid.Skip(1).Take(1).GetName();
            return dispatchers.Find(type);
        }

        internal static IEnumerable<ProcessId> ResolveProcessIdSelection(ProcessId pid) =>
            GetProcessSelector(pid)
                .Map(selector => selector(pid.Skip(2)))
                .IfNone(() => new ProcessId[0]);

        internal static IActorDispatch GetPluginDispatcher(ProcessId pid) =>
            GetProcessSelector(pid)
                .Map(selector => new ActorDispatchGroup(selector(pid.Skip(2))) as IActorDispatch)
                .IfNone(() => new ActorDispatchNotExist(pid));

        internal static bool IsLocal(ProcessId pid) =>
            pid.Head() == Root;

        internal static bool IsDisp(ProcessId pid) =>
            pid.Head() == Disp;

        internal static IActorDispatch GetDispatcher(ProcessId pid) =>
            pid.IsValid
                ? pid.IsSelection
                    ? new ActorDispatchGroup(pid.GetSelection())
                    : IsDisp(pid)
                        ? GetPluginDispatcher(pid)
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

        public static Unit Kill(ProcessId pid, bool maintainState) =>
            maintainState
                ? GetDispatcher(pid).Shutdown()
                : GetDispatcher(pid).Kill();
    }
}
