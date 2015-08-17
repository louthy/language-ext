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

        public static Unit Startup(Option<ICluster> cluster, int version = 0)
        {
            ActorContext.cluster = cluster;
            var name = GetRootProcessName();
            if (name.Value == "root" && cluster.IsSome) throw new ArgumentException("Cluster node name cannot be 'root', it's reserved for local use only.");
            if (name.Value == "registered") throw new ArgumentException("Node name cannot be 'registered', it's reserved for registered processes.");

            lock (sync)
            {
                root = ProcessId.Top.Child(name);
                rootInbox = new ActorInboxLocal<ActorSystemState, object>();
                rootProcess = new Actor<ActorSystemState, object>(
                    cluster,
                    ProcessId.Top,
                    ActorConfig.Default.RootProcessName, 
                    ActorSystem.Inbox, 
                    rootProcess => new ActorSystemState(cluster, root, rootProcess, rootInbox, GetRootProcessName(), ActorConfig.Default), 
                    ProcessFlags.Default
                );
                rootInbox.Startup(rootProcess, rootProcess.Parent, cluster, version);
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
                    Process.ask<Unit>(ActorContext.Root, ActorSystemMessage.ShutdownAll);
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

            Process.tell(AskId, req);
            handle.WaitOne(ActorConfig.Default.Timeout);

            if (response == null)
            {
                throw new TimeoutException("Request timed out");
            }
            else
            {
                if (response.IsFaulted)
                {
                    throw new Exception("Request failed.", response.Exception);
                }
                else
                {
                    return (T)response.Response;
                }
            }
        }

        public static ProcessName GetRootProcessName() =>
            cluster.Map(x => x.NodeName)
                   .IfNone(ActorConfig.Default.RootProcessName);

        public static Unit RegisterCluster(ICluster cluster) =>
            Startup(Some(cluster));

        public static Unit DeregisterCluster()
        {
            cluster.IfSome(c =>
            {
                c.Disconnect();
                c.Dispose();
            });
            return Startup(None);
        }

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
            message == null
                ? raise<Unit>(new ArgumentNullException(nameof(message), "Messages can't be null"))
                : to.Path == root.Path
                    ? LocalRoot.Tell(message, SenderOrDefault(sender))
                    : Tell(root, ActorSystemMessage.Tell(to, message, SenderOrDefault(sender)), SenderOrDefault(sender));

        public static Unit TellUserControl(ProcessId to, UserControlMessage message) =>
            message == null
                ? raise<Unit>(new ArgumentNullException(nameof(message), "User control messages can't be null"))
                : to.Path == root.Path
                    ? LocalRoot.Tell(message, Self)
                    : Tell(root, ActorSystemMessage.TellUserControl(to, message, Self), Self);

        public static Unit TellSystem(ProcessId to, SystemMessage message) =>
            message == null
                ? raise<Unit>(new ArgumentNullException(nameof(message), "System messages can't be null"))
                : to.Path == root.Path
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
            Root.Child(ActorConfig.Default.SystemProcessName);

        public static ProcessId User =>
            Root.Child(ActorConfig.Default.UserProcessName);

        public static ProcessId Registered =>
            Root.Child(ActorConfig.Default.RegisteredProcessName);

        public static ProcessId Errors =>
            System.Child(ActorConfig.Default.ErrorsProcessName);

        public static ProcessId DeadLetters =>
            System.Child(ActorConfig.Default.DeadLettersProcessName);

        public static Map<string, ProcessId> Children =>
            self.Children;

        private static ProcessName NodeName =>
            cluster.Map(c => c.NodeName).IfNone("user");

        internal static ProcessId AskId =>
            System.Child(ActorConfig.Default.AskProcessName);

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
            Process.kill(Registered.Child(name));

        public static R WithContext<R>(IActor self, ProcessId sender, ActorRequest request, object msg, Func<R> f)
        {
            var savedSelf = ActorContext.self;
            var savedSender = ActorContext.sender;
            var savedMsg = ActorContext.currentMsg;
            var savedReq = ActorContext.currentRequest;

            try
            {
                ActorContext.self = self;
                ActorContext.sender = sender;
                ActorContext.currentMsg = msg;
                ActorContext.currentRequest = request;
                return f();
            }
            finally
            {
                ActorContext.self = savedSelf;
                ActorContext.sender = savedSender;
                ActorContext.currentMsg = savedMsg;
                ActorContext.currentRequest = savedReq;
            }
        }

        public static Unit WithContext(IActor self, ProcessId sender, ActorRequest request, object msg, Action f)
        {
            var savedSelf = ActorContext.self;
            var savedSender = ActorContext.sender;
            var savedMsg = ActorContext.currentMsg;
            var savedReq = ActorContext.currentRequest;

            try
            {
                ActorContext.self = self;
                ActorContext.sender = sender;
                ActorContext.currentMsg = msg;
                ActorContext.currentRequest = request;
                f();
                return unit;
            }
            finally
            {
                ActorContext.self = savedSelf;
                ActorContext.sender = savedSender;
                ActorContext.currentMsg = savedMsg;
                ActorContext.currentRequest = savedReq;
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

        internal static ProcessId SenderOrDefault(ProcessId sender) =>
            sender.IsValid
                ? sender
                : Self;
    }
}
