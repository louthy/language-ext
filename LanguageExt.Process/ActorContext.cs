using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using static LanguageExt.Prelude;
using System.Threading;
using System.Reactive.Concurrency;

namespace LanguageExt
{
    internal static class ActorContext
    {
        static Option<ICluster> cluster;
        static ProcessId root;
        static IActor rootProcess;
        static IActorInbox rootInbox;

        [ThreadStatic] static IActor self;
        [ThreadStatic] static ProcessId sender;
        [ThreadStatic] static object currentMsg;
        [ThreadStatic] static ActorRequest currentRequest;
        [ThreadStatic] static ProcessFlags processFlags;

        static object sync = new object();

        static ActorContext()
        {
            Startup(None);
        }

        public static Unit Startup(Option<ICluster> cluster)
        {
            lock (sync)
            {
                Shutdown();

                ActorContext.cluster = cluster;
                var name = ProcessId.Sep.ToString() + ActorConfig.Default.RootProcessName;
                root = new ProcessId(name);
                rootInbox = new ActorInboxLocal<ActorSystemState, object>();
                rootProcess = new Actor<ActorSystemState, object>(
                    cluster,
                    new ProcessId(ProcessId.Sep.ToString()),
                    ActorConfig.Default.RootProcessName, 
                    ActorSystem.Inbox, 
                    rootProcess => new ActorSystemState(cluster, root, rootProcess, rootInbox, GetUserProcessName(), ActorConfig.Default), 
                    ProcessFlags.Default
                );
                rootInbox.Startup(rootProcess, rootProcess.Parent, cluster);
                rootProcess.Startup();
                LocalRoot.Tell(ActorSystemMessage.Startup, ProcessId.NoSender);
            }
            return unit;
        }

        public static Unit Shutdown()
        {
            lock (sync)
            {
                if (rootInbox != null)
                {
                    Process.shutdownAll();
                }
            }
            return unit;
        }

        public static Unit Restart()
        {
            Startup(cluster);
            return unit;
        }

        public static IObservable<T> Ask<T>(ProcessId pid, object message)
        {
            var subject = new Subject<object>();
            return subject.PostSubscribeAction(() => Process.tell(AskId, new AskActorReq(message, subject, pid, Self)))
                          .Timeout(ActorConfig.Default.Timeout)
                          .Select(obj => (T)obj);
        }

        public static ProcessName GetUserProcessName() =>
            cluster.Map(x => x.NodeName)
                   .IfNone(ActorConfig.Default.UserProcessName);

        public static Unit RegisterCluster(ICluster cluster) =>
            Startup(Some(cluster));

        public static Unit DeregisterCluster() =>
            Startup(None);

        public static ProcessId ActorCreate<S, T>(ProcessId parent, ProcessName name, Func<T, Unit> actorFn, ProcessFlags flags)
        {
            return ActorCreate<S, T>(parent, name, (s, t) => { actorFn(t); return default(S); }, () => default(S), flags);
        }

        public static ProcessId ActorCreate<S, T>(ProcessId parent, ProcessName name, Action<T> actorFn, ProcessFlags flags)
        {
            return ActorCreate<S, T>(parent, name, (s, t) => { actorFn(t); return default(S); }, () => default(S), flags);
        }

        public static ProcessId ActorCreate<S,T>(ProcessId parent, ProcessName name, Func<S, T, S> actorFn, Func<S> setupFn, ProcessFlags flags)
        {
            if (parent.IsValid)
            {
                var actor = new Actor<S, T>(cluster, parent, name, actorFn, setupFn, flags);

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

                Tell(Root, ActorSystemMessage.AddToStore(actor, inbox, flags), Self);
                return actor.Id;
            }
            else
            {
                return ProcessId.None;
            }
        }

        public static Unit Tell(ProcessId to, object message, ProcessId sender) =>
            to.Path == root.Path
                ? LocalRoot.Tell(message, sender)
                : Tell(root, ActorSystemMessage.Tell(to, message, sender), sender);

        public static Unit TellUserControl(ProcessId to, UserControlMessage message) =>
            to.Path == root.Path
                ? LocalRoot.Tell(message, Self)
                : Tell(root, ActorSystemMessage.TellUserControl(to, message, Self), Self);

        public static Unit TellSystem(ProcessId to, SystemMessage message) =>
            to.Path == root.Path
                ? LocalRoot.Tell(message, Self)
                : Tell(root, ActorSystemMessage.TellSystem(to, message, Self), Self);

        public static ILocalActorInbox LocalRoot => 
            (ILocalActorInbox)rootInbox;

        public static IActorInbox RootInbox =>
            rootInbox;

        public static ProcessId Root =>
            root;

        public static ProcessId Self =>
            self != null
                ? self.Id
                : User;

        // Try to avoid using this where possible and use Self
        public static IActor SelfProcess =>
            self;

        public static ProcessId Sender =>
            sender;

        public static ProcessId Parent =>
            self.Parent;

        public static ProcessId System =>
            Root.MakeChildId(ActorConfig.Default.SystemProcessName);

        public static ProcessId User =>
            Root.MakeChildId(GetUserProcessName());

        public static ProcessId Registered =>
            Root.MakeChildId(ActorConfig.Default.RegisteredProcessName);

        public static ProcessId Errors =>
            System.MakeChildId(ActorConfig.Default.ErrorsProcessName);

        public static ProcessId DeadLetters =>
            System.MakeChildId(ActorConfig.Default.DeadLettersProcessName);

        public static Map<string, ProcessId> Children =>
            self.Children;

        private static ProcessName NodeName =>
            cluster.Map(c => c.NodeName).IfNone("user");

        internal static ProcessId AskId =>
            System.MakeChildId(ActorConfig.Default.AskProcessName + "-" + NodeName);

        public static ActorRequest CurrentRequest
        {
            get
            {
                return currentRequest;
            }
            set
            {
                currentRequest = value;
            }
        }

        public static object CurrentMsg
        {
            get
            {
                return currentMsg;
            }
            set
            {
                currentMsg = value;
            }
        }

        public static ProcessFlags ProcessFlags
        {
            get
            {
                return processFlags;
            }
            set
            {
                processFlags = value;
            }
        }

        public static ProcessId Register<T>(ProcessName name, ProcessId processId, ProcessFlags flags)
        {
            if (processId.IsValid)
            {
                return ActorCreate<ProcessId, T>(
                    Registered, 
                    name, 
                    RegisteredActor<T>.Inbox,
                    () => processId,
                    flags
                );
            }
            else
            {
                return ProcessId.None;
            }
        }

        public static Unit Deregister(ProcessName name) =>
            Process.kill(Registered.MakeChildId(name));

        public static R WithContext<R>(IActor self, ProcessId sender, object msg, Func<R> f)
        {
            var savedSelf = ActorContext.self;
            var savedSender = ActorContext.sender;
            var savedMsg = ActorContext.currentMsg;

            try
            {
                ActorContext.self = self;
                ActorContext.sender = sender;
                ActorContext.currentMsg = msg;
                return f();
            }
            finally
            {
                ActorContext.self = savedSelf;
                ActorContext.sender = savedSender;
                ActorContext.currentMsg = savedMsg;
            }
        }

        public static Unit WithContext(IActor self, ProcessId sender, object msg, Action f)
        {
            var savedSelf = ActorContext.self;
            var savedSender = ActorContext.sender;
            var savedMsg = ActorContext.currentMsg;

            try
            {
                ActorContext.self = self;
                ActorContext.sender = sender;
                ActorContext.currentMsg = msg;
                f();
                return unit;
            }
            finally
            {
                ActorContext.self = savedSelf;
                ActorContext.sender = savedSender;
                ActorContext.currentMsg = savedMsg;
            }
        }

        public static Unit Publish(object message)
        {
            if (cluster.IsSome && (ProcessFlags & ProcessFlags.RemotePublish) == ProcessFlags.RemotePublish)
            {
                cluster.IfNone(() => null).PublishToChannel(Self.Path + "-pubsub", message);
            }
            else
            {
                LocalRoot.Tell(ActorSystemMessage.Publish(Self, message), Self);
            }
            return unit;
        }

        internal static IObservable<T> Observe<T>(ProcessId pid)
        {
            return from x in Process.ask<IObservable<object>>(Root, ActorSystemMessage.ObservePub(pid, typeof(T)))
                   where x is T
                   select (T)x;
        }

    }
}
