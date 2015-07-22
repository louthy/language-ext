using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Reactive.Subjects;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    internal static class ActorContext
    {
        static ProcessId root;
        static ProcessId user;
        static ProcessId system;
        static ProcessId deadLetters;
        static ProcessId noSender;
        static ProcessId registered;
        static ProcessId errors;
        static IActorInbox rootInbox;
        static Option<ICluster> cluster;
        [ThreadStatic] static ProcessId self;
        [ThreadStatic] static ProcessId sender;

        static object sync = new object();

        static ActorContext()
        {
            Startup();
        }
        
        public static Func<object, Unit> RootProcess = (msg) =>
        {
            try
            {
                if (msg is RootMessage)
                {
                    var rmsg = msg as RootMessage;
                    switch (rmsg.Tag)
                    {
                        case RootMessageTag.AddToStore:
                            {
                                var addmsg = rmsg as AddToStoreMessage;
                                var path = addmsg.Process.Id.Value;
                                ActorStore = ActorStore.AddOrUpdate(path, addmsg.Process);
                                ActorInboxStore = ActorInboxStore.AddOrUpdate(path, addmsg.Inbox);
                                addmsg.Inbox.Startup(addmsg.Process, addmsg.Process.Parent);
                                addmsg.Process.Startup();
                                break;
                            }

                        case RootMessageTag.RemoveFromStore:
                            {
                                var rmvmsg = rmsg as RemoveFromStoreMessage;
                                var path = rmvmsg.ProcessId.Value;
                                ActorStore = ActorStore.Remove(path);
                                ActorInboxStore = ActorInboxStore.Remove(path);
                                break;
                            }

                        case RootMessageTag.Tell:
                        case RootMessageTag.TellSystem:
                        case RootMessageTag.TellUserControl:
                            var tellmsg = rmsg as TellMessage;
                            ActorInboxStore.Find(
                                tellmsg.ProcessId.Value,
                                Some: x => rmsg.Tag == RootMessageTag.TellSystem && tellmsg.Message is SystemMessage ? x.TellSystem(tellmsg.Message as SystemMessage)
                                         : rmsg.Tag == RootMessageTag.TellUserControl && tellmsg.Message is UserControlMessage ? x.TellUserControl(tellmsg.Message as UserControlMessage)
                                         : WithContext(tellmsg.ProcessId, tellmsg.Sender, () => x.Tell(tellmsg.Message, sender)),
                                None: () => ActorInboxStore.Find(DeadLetters.ToString(),
                                                Some: x => WithContext(tellmsg.ProcessId, tellmsg.Sender, () => x.Tell(tellmsg.Message, sender)),
                                                None: () => unit));
                            break;

                        case RootMessageTag.ShutdownAll:
                            GetProcess(User).Children.Iter(child => TellSystem(child, SystemMessage.Shutdown));
                            break;

                        case RootMessageTag.Register:
                            var regmsg = rmsg as RegisterMessage;
                            var process = GetProcess(regmsg.ProcessId);

                            var proxy = new ActorProxy(
                                            cluster,
                                            Registered,
                                            regmsg.Name,
                                            ActorProxyTemplate.Registered,
                                            () => new ActorProxyConfig(regmsg.ProcessId)
                                        );

                            var inbox = new ActorInbox<ActorProxyConfig, object>();

                            Tell(Root, RootMessage.AddToStore(proxy, inbox), ProcessId.NoSender);
                            TellSystem(Registered, SystemMessage.LinkChild(proxy.Id));
                            break;

                        case RootMessageTag.UnRegister:
                            var unregmsg = rmsg as UnRegisterMessage;
                            map(Registered.MakeChildId(unregmsg.Name).ToString(),
                                id =>
                                    ActorStore.ContainsKey(id)
                                        ? Process.kill(id)
                                        : unit);
                            break;
                    }
                }
                else if (msg is SystemMessage)
                {
                    rootInbox.TellSystem(msg as SystemMessage);
                }
                else if (msg is UserControlMessage)
                {
                    rootInbox.TellUserControl(msg as UserControlMessage);
                }
                else
                {
                    Process.pub(msg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return unit;
        };

        public static Unit Startup()
        {
            lock (sync)
            {
                Shutdown();

                ObservableRouter.Restart();

                ActorConfig config = ActorConfig.Default;

                ActorStore = Map(Tuple("", (IProcess)new NullProcess()));
                ActorInboxStore = Map(Tuple("", (IActorInbox)new NullInbox()));

                // Root
                map(StartupRoot(), (pid, inbox) =>
                {
                    root = pid;
                    rootInbox = inbox;
                });

                // Top tier
                system = ActorCreate<Unit, object>(root, config.SystemProcessName, Process.pub);
                self = user = ActorCreate<Unit, object>(root, config.UserProcessName, Process.pub);

                // Second tier
                deadLetters = ActorCreate<Unit, object>(system, config.DeadLettersProcessName, Process.pub);
                errors = ActorCreate<Unit, Exception>(system, config.Errors, Process.pub);
                noSender = ActorCreate<Unit, object>(system, config.NoSenderProcessName, Process.pub);
                registered = ActorCreate<Unit, object>(system, config.RegisteredProcessName, Process.pub);
            }
            return unit;
        }

        private static Tuple<ProcessId, IActorInbox> StartupRoot()
        {
            var name = GetRootProcessName();
            var actor = new Actor<Unit, object>(cluster, new ProcessId("/"), name, RootProcess);
            var inbox = new ActorInbox<Unit, object>();

            ActorStore = ActorStore.Add(name.Value, actor);
            ActorInboxStore = ActorInboxStore.Add(name.Value, inbox);

            inbox.Startup(actor, actor.Parent);

            return Tuple(actor.Id, inbox as IActorInbox);
        }

        public static Unit Shutdown()
        {
            lock (sync)
            {
                if (ActorInboxStore != null)
                {
                    ActorInboxStore.Iter(inbox => inbox.Shutdown());
                    ActorInboxStore = ActorInboxStore.Clear();
                }
                if (ActorStore != null)
                {
                    ActorStore = ActorStore.Clear();
                }
                ObservableRouter.Shutdown();
            }
            return unit;
        }

        public static Unit Restart()
        {
            Startup();
            return unit;
        }

        public static ProcessName GetRootProcessName() =>
            cluster.Map(x => x.NodeName)
                   .IfNone(ActorConfig.Default.RootProcessName);

        public static Unit RegisterCluster(ICluster cluster)
        {
            lock (sync)
            {
                ActorContext.cluster = Some(cluster);
                Restart();
                return unit;
            }
        }

        public static Unit UnRegisterCluster()
        {
            lock (sync)
            {
                ActorContext.cluster = None;
                Restart();
                return unit;
            }
        }

        public static ProcessId ActorCreate<S, T>(ProcessId parent, ProcessName name, Func<T, Unit> actorFn)
        {
            return ActorCreate<S, T>(parent, name, (s, t) => { actorFn(t); return default(S); }, () => default(S));
        }

        public static ProcessId ActorCreate<S, T>(ProcessId parent, ProcessName name, Action<T> actorFn)
        {
            return ActorCreate<S, T>(parent, name, (s, t) => { actorFn(t); return default(S); }, () => default(S));
        }

        public static ProcessId ActorCreate<S,T>(ProcessId parent, ProcessName name, Func<S, T, S> actorFn, Func<S> setupFn)
        {
            if (parent.IsValid)
            {
                var actor = new Actor<S, T>(cluster, parent, name, actorFn, setupFn);
                var inbox = new ActorInbox<S, T>();
                Tell(Root, RootMessage.AddToStore(actor, inbox), ProcessId.NoSender);
                return actor.Id;
            }
            else
            {
                return ProcessId.None;
            }
        }

        private static Map<string, IProcess> ActorStore
        {
            get;
            set;
        }

        private static Map<string, IActorInbox> ActorInboxStore
        {
            get;
            set;
        }

        public static Unit Tell(ProcessId to, object message, ProcessId sender) =>
            to.Value == root.Value
                ? rootInbox.Tell(message, sender)
                : Tell(root, RootMessage.Tell(to, message, sender), sender);

        public static Unit TellUserControl(ProcessId to, UserControlMessage message) =>
            to.Value == root.Value
                ? rootInbox.Tell(message, sender)
                : Tell(root, RootMessage.TellUserControl(to, message, sender), sender);

        public static Unit TellSystem(ProcessId to, SystemMessage message) =>
            to.Value == root.Value
                ? rootInbox.Tell(message, sender)
                : Tell(root, RootMessage.TellSystem(to, message, sender), sender);

        public static ProcessId Self =>
            self.IsValid
                ? self
                : user;

        public static ProcessId Root =>
            root;

        public static ProcessId User =>
            user;

        public static ProcessId System =>
            system;

        public static ProcessId Sender =>
            sender;

        public static ProcessId NoSender =>
            noSender;

        public static ProcessId Errors =>
            errors;

        public static ProcessId DeadLetters =>
            deadLetters;

        public static ProcessId Registered =>
            registered;

        public static ProcessId Register(ProcessName name, ProcessId processId)
        {
            if (processId.IsValid)
            {
                Tell(Root, RootMessage.Register(name, processId), ActorContext.Self);
                return Registered.MakeChildId(name);
            }
            else
            {
                return ProcessId.None;
            }
        }

        public static Unit UnRegister(ProcessName name) =>
            Tell(Root, RootMessage.UnRegister(name), ActorContext.Self);

        public static R WithContext<R>(ProcessId self, ProcessId sender, Func<R> f)
        {
            var savedSelf = ActorContext.self;
            var savedSender = ActorContext.sender;

            try
            {
                ActorContext.self = self;
                ActorContext.sender = sender;
                return f();
            }
            finally
            {
                ActorContext.self = savedSelf;
                ActorContext.sender = savedSender;
            }
        }

        public static Unit WithContext(ProcessId self, ProcessId sender, Action f)
        {
            var savedSelf = ActorContext.self;
            var savedSender = ActorContext.sender;

            try
            {
                ActorContext.self = self;
                ActorContext.sender = sender;
                f();
                return unit;
            }
            finally
            {
                ActorContext.self = savedSelf;
                ActorContext.sender = savedSender;
            }
        }

        public static IActorInbox GetInbox(ProcessId pid) =>
            map(ActorInboxStore, store =>
                store.ContainsKey(pid.Value)
                    ? store[pid.Value]
                    : store[ActorContext.DeadLetters.Value]
                );

        public static IProcess GetProcess(ProcessId pid) =>
            map(ActorStore, store =>
                store.ContainsKey(pid.Value)
                    ? store[pid.Value]
                    : store[ActorContext.DeadLetters.Value]
                );
    }
}
