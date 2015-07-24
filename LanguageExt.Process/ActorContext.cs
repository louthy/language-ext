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
        static IProcess rootProcess;
        static IActorInbox rootInbox;
        static Subject<ActorResponse> rootResponse = new Subject<ActorResponse>();

        [ThreadStatic] static ProcessId self;
        [ThreadStatic] static ProcessId sender;
        [ThreadStatic] static ProcessId parent;
        [ThreadStatic] static Map<string, ProcessId> children = Map.empty<string,ProcessId>();
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
                var name = GetRootProcessName();
                root = new ProcessId(ProcessId.Sep.ToString() + name);
                rootInbox = new ActorInbox<ActorSystemState, object>();
                rootResponse = new Subject<ActorResponse>();
                rootProcess = new Actor<ActorSystemState, object>(
                    cluster, 
                    new ProcessId(ProcessId.Sep.ToString()), 
                    name, 
                    ActorSystem.Inbox, 
                    rootProcess => new ActorSystemState(cluster, root, rootProcess, rootInbox, ActorConfig.Default), 
                    ProcessFlags.Default
                );
                rootInbox.Startup(rootProcess, rootProcess.Parent, cluster);
                rootProcess.Startup();
                rootInbox.Tell(ActorSystemMessage.Startup, ProcessId.NoSender);
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

        static Dictionary<int, ProcessId> ManagedThreadActors = new Dictionary<int, ProcessId>();

        public static IObservable<T> Ask<T>(ProcessId pid, object message)
        {
            var subject = new Subject<object>();
            return subject.PostSubscribeAction(() => Process.tell(pid, new ActorRequest(message, pid, Self, subject)))
                          .Timeout(ActorConfig.Default.Timeout)
                          .Select(obj => (T)obj);
        }

        public static ProcessName GetRootProcessName() =>
            cluster.Map(x => x.NodeName)
                   .IfNone(ActorConfig.Default.RootProcessName);

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
                var inbox = new ActorInbox<S, T>();
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
                ? rootInbox.Tell(message, sender)
                : Tell(root, ActorSystemMessage.Tell(to, message, sender), sender);

        public static Unit TellUserControl(ProcessId to, UserControlMessage message) =>
            to.Path == root.Path
                ? rootInbox.Tell(message, Self)
                : Tell(root, ActorSystemMessage.TellUserControl(to, message, Self), Self);

        public static Unit TellSystem(ProcessId to, SystemMessage message) =>
            to.Path == root.Path
                ? rootInbox.Tell(message, Self)
                : Tell(root, ActorSystemMessage.TellSystem(to, message, Self), Self);

        public static ProcessId Root =>
            root;

        public static ProcessId Self =>
            self.IsValid
                ? self
                : User;

        public static ProcessId Sender =>
            sender;

        public static ProcessId Parent =>
            parent;

        public static ProcessId System =>
            Root.MakeChildId(ActorConfig.Default.SystemProcessName);

        public static ProcessId User =>
            Root.MakeChildId(ActorConfig.Default.UserProcessName);

        public static ProcessId Registered =>
            System.MakeChildId(ActorConfig.Default.RegisteredProcessName);

        public static ProcessId Errors =>
            System.MakeChildId(ActorConfig.Default.ErrorsProcessName);

        public static ProcessId DeadLetters =>
            System.MakeChildId(ActorConfig.Default.DeadLettersProcessName);

        public static Map<string, ProcessId> Children =>
            children;

        internal static ProcessId AskReqRes =>
            System.MakeChildId(ActorConfig.Default.AskProcessName);

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

        public static IActorInbox RootInbox => 
            rootInbox;

        public static ProcessId Register(ProcessName name, ProcessId processId)
        {
            if (processId.IsValid)
            {
                Tell(Root, ActorSystemMessage.Register(name, processId), ActorContext.Self);
                return Registered.MakeChildId(name);
            }
            else
            {
                return ProcessId.None;
            }
        }

        public static Unit Deregister(ProcessName name) =>
            Tell(Root, ActorSystemMessage.Deregister(name), ActorContext.Self);

        public static R WithContext<R>(ProcessId self, ProcessId parent, Map<string, ProcessId> children, ProcessId sender, Func<R> f)
        {
            var savedSelf = ActorContext.self;
            var savedSender = ActorContext.sender;
            var savedParent = ActorContext.parent;
            var savedChildren = ActorContext.children ?? Map.empty<string,ProcessId>();

            try
            {
                ActorContext.self = self;
                ActorContext.sender = sender;
                ActorContext.parent = parent;
                ActorContext.children = children;
                return f();
            }
            finally
            {
                ActorContext.self = savedSelf;
                ActorContext.sender = savedSender;
                ActorContext.parent = savedParent;
                ActorContext.children = savedChildren;
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
                RootInbox.Tell(ActorSystemMessage.Publish(Self, message), Self);
            }
            return unit;
        }

        internal static IObservable<T> Observe<T>(ProcessId pid)
        {
            return from x in Process.ask<IObservable<object>>(Root, ActorSystemMessage.ObservePub(pid, (System.Type)typeof(T)))
                   where x is T
                   select (T)x;
        }

        public static Unit WithContext(ProcessId self, ProcessId parent, Map<string, ProcessId> children, ProcessId sender, Action f)
        {
            var savedSelf = ActorContext.self;
            var savedSender = ActorContext.sender;
            var savedParent = ActorContext.parent;
            var savedChildren = ActorContext.children;

            try
            {
                ActorContext.self = self;
                ActorContext.sender = sender;
                ActorContext.parent = parent;
                ActorContext.children = children;
                f();
                return unit;
            }
            finally
            {
                ActorContext.self = savedSelf;
                ActorContext.sender = savedSender;
                ActorContext.parent = savedParent;
                ActorContext.children = savedChildren;
            }
        }
    }
}
