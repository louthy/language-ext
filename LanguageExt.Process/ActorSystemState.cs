using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    /// <summary>
    /// Represents the state of the whole actor system.  Mostly it holds the store of
    /// processes (actors) and their inboxes.
    /// </summary>
    internal class ActorSystemState
    {
        public readonly Option<ICluster> Cluster;
        public readonly IActorInbox RootInbox;
        public readonly IProcess RootProcess;
        public readonly ActorConfig Config;

        ProcessId root;
        ProcessId user;
        ProcessId system;
        ProcessId deadLetters;
        ProcessId noSender;
        ProcessId registered;
        ProcessId errors;
        ProcessId inboxShutdown;
        ProcessId ask;
        ProcessId reply;

        // We can use a mutable and non-locking dictionary here because access to 
        // it will be via the root actor's message-loop only
        Dictionary<string, ActorItem> store = new Dictionary<string, ActorItem>();

        public class ActorItem
        {
            public readonly IProcess Actor;
            public readonly IActorInbox Inbox;
            public readonly ProcessFlags Flags;

            public ActorItem(
                IProcess actor,
                IActorInbox inbox,
                ProcessFlags flags
                )
            {
                Actor = actor;
                Inbox = inbox;
                Flags = flags;
            }
        }

        public ActorSystemState(Option<ICluster> cluster, ProcessId rootId, IProcess rootProcess, IActorInbox rootInbox, ActorConfig config)
        {
            root = rootId;
            Config = config;
            Cluster = cluster;
            RootProcess = rootProcess;
            RootInbox = rootInbox;

            store.Add(Root.Path, new ActorItem(RootProcess, rootInbox, ProcessFlags.Default));
        }

        public ActorSystemState Startup()
        {
            logInfo("Process system starting up");

            // Top tier
            system          = ActorCreate<object>(root, Config.SystemProcessName, publish, ProcessFlags.Default);
            user            = ActorCreate<object>(root, Config.UserProcessName, publish, ProcessFlags.Default);

            // Second tier
            deadLetters     = ActorCreate<object>(system, Config.DeadLettersProcessName, publish, ProcessFlags.Default);
            errors          = ActorCreate<Exception>(system, Config.ErrorsProcessName, publish, ProcessFlags.Default);
            noSender        = ActorCreate<object>(system, Config.NoSenderProcessName, publish, ProcessFlags.Default);
            registered      = ActorCreate<object>(system, Config.RegisteredProcessName, publish, ProcessFlags.Default);

            inboxShutdown   = ActorCreate<IActorInbox>(system, Config.InboxShutdownProcessName, inbox => inbox.Shutdown(), ProcessFlags.Default);

            ask             = ActorCreate<AskActorReq>(system, Config.AskProcessName, AskActor.Inbox, ProcessFlags.Default);
            reply           = ActorCreate<Random,ActorResponse>(system, Config.ReplyProcessName, ReplyActor.Inbox, ReplyActor.Setup, ProcessFlags.Default);

            logInfo("Process system startup complete");

            return this;
        }

        public Unit Shutdown()
        {
            logInfo("Process system shutting down");

            ShutdownProcess(User);
            user = ActorCreate<object>(root, Config.UserProcessName, publish, ProcessFlags.Default);
            store[reply.Path].Inbox.Tell(new ActorResponse(ActorContext.CurrentRequest.ReplyTo, unit, ActorContext.CurrentRequest.Subject, Root), ActorContext.Root);

            logInfo("Process system shutdown complete");

            return unit;
        }

        public ActorSystemState ShutdownProcess(ProcessId processId)
        {
            if (ProcessDoesNotExist(nameof(ShutdownProcess), processId)) return this;

            var item = store[processId.Path];
            var process = item.Actor;
            var inbox = item.Inbox;

            var parent = store[process.Parent.Path];
            parent.Actor.UnlinkChild(processId);

            ShutdownProcessRec(processId, store[inboxShutdown.Path].Inbox);
            process.Shutdown();
            store.Remove(processId.Path);

            return this;
        }

        private void ShutdownProcessRec(ProcessId processId, IActorInbox inboxShutdown)
        {
            var item = store[processId.Path];
            var process = item.Actor;
            var inbox = item.Inbox;

            foreach (var child in process.Children.Values)
            {
                ShutdownProcessRec(child, inboxShutdown);
            }
            inboxShutdown.Tell(inbox, ProcessId.NoSender);
            process.Shutdown();
            store.Remove(processId.Path);
        }

        public Unit Reply(ProcessId replyTo, object message, ProcessId sender, Subject<object> subject) =>
            store[reply.Path].Inbox.Tell(new ActorResponse(replyTo, message, subject, sender), sender);

        public Unit GetChildren(ProcessId processId)
        {
            if (ReplyToProcessDoesNotExist(nameof(GetChildren))) return unit;
            if (ProcessDoesNotExist(nameof(GetChildren), processId)) return unit;

            Map<string, ProcessId> kids = store[processId.Path].Actor.Children;

            ReplyInfo(nameof(GetChildren), processId, kids.Count);

            return store[reply.Path].Inbox.Tell(
                        new ActorResponse(ActorContext.CurrentRequest.ReplyTo, kids, ActorContext.CurrentRequest.Subject, processId),
                        processId
                    );
        }

        internal Unit Publish(ProcessId processId, object message)
        {
            if (ProcessDoesNotExist(nameof(Publish), processId)) return unit;

            return store[processId.Path].Actor.Publish(message);
        }

        internal Unit ObservePub(ProcessId processId, Type type)
        {
            if (ReplyToProcessDoesNotExist(nameof(ObservePub))) return unit;
            if (ProcessDoesNotExist(nameof(ObservePub), processId)) return unit;

            var item = store[processId.Path];

            IObservable<object> stream = null;

            if (Cluster.IsSome && (item.Flags & ProcessFlags.RemotePublish) == ProcessFlags.RemotePublish)
            {
                var cluster = Cluster.IfNone(() => null);
                stream = cluster.SubscribeToChannel(processId.Path + "-pubsub", type);
            }
            else
            {
                stream = item.Actor.PublishStream;
            }
            return store[reply.Path].Inbox.Tell(
                    new ActorResponse(
                        ActorContext.CurrentRequest.ReplyTo,
                        stream,
                        ActorContext.CurrentRequest.Subject,
                        processId
                        ),
                    processId);
        }

        internal Unit ObserveState(ProcessId processId)
        {
            if (ReplyToProcessDoesNotExist(nameof(ObservePub))) return unit;
            if (ProcessDoesNotExist(nameof(ObservePub), processId)) return unit;

            return store[reply.Path].Inbox.Tell(
                new ActorResponse(
                    ActorContext.CurrentRequest.ReplyTo,
                    store.ContainsKey(processId.Path)
                        ? store[processId.Path].Actor.StateStream
                        : null,
                    ActorContext.CurrentRequest.Subject,
                    processId
                    ),
                processId);
        }

        private Unit TellDeadLetters(ProcessId processId, ProcessId sender, ActorSystemMessageTag tag, object message)
        {
            logWarn("Sending to DeadLetters, process (" + processId + ") doesn't exist.  Message: "+message);

            return FindInStore(DeadLetters,
                        Some: item => ActorContext.WithContext(processId, item.Actor.Parent, item.Actor.Children, sender, () => item.Inbox.Tell(message, sender)),
                        None: () => unit);
        }

        public Unit Tell(ProcessId processId, ProcessId sender, ActorSystemMessageTag tag, object message) =>
            FindInStore(
                processId,
                Some: item => tag == ActorSystemMessageTag.TellSystem && message is SystemMessage           ? item.Inbox.TellSystem(message as SystemMessage)
                            : tag == ActorSystemMessageTag.TellUserControl && message is UserControlMessage ? item.Inbox.TellUserControl(message as UserControlMessage)
                            : ActorContext.WithContext(processId, item.Actor.Parent, item.Actor.Children, sender, () => item.Inbox.Tell(message, sender)),
                None: () => TellDeadLetters(processId,sender,tag,message));

        public ProcessId ActorCreate<T>(ProcessId parent, ProcessName name, Func<T, Unit> actorFn, ProcessFlags flags)
        {
            return ActorCreate<Unit, T>(parent, name, (s, t) => { actorFn(t); return unit; }, () => unit, flags);
        }

        public ProcessId ActorCreate<T>(ProcessId parent, ProcessName name, Action<T> actorFn, ProcessFlags flags)
        {
            return ActorCreate<Unit, T>(parent, name, (s, t) => { actorFn(t); return unit; }, () => unit, flags);
        }

        public ProcessId ActorCreate<S, T>(ProcessId parent, ProcessName name, Func<S, T, S> actorFn, Func<S> setupFn, ProcessFlags flags)
        {
            if (ProcessDoesNotExist(nameof(ActorCreate), parent)) return ProcessId.None;

            var actor = new Actor<S, T>(Cluster, parent, name, actorFn, setupFn, flags);
            var inbox = new ActorInbox<S, T>();
            AddOrUpdateStoreAndStartup(actor, inbox, flags);
            return actor.Id;
        }

        public ActorSystemState AddOrUpdateStoreAndStartup(IProcess process, IActorInbox inbox, ProcessFlags flags)
        {
            if (store.ContainsKey(process.Parent.Path))
            {
                var parent = store[process.Parent.Path];

                if (process.Id.Path.Length > process.Parent.Path.Length &&
                    process.Id.Path.StartsWith(process.Parent.Path))
                {
                    var path = process.Id.Path;
                    if (store.ContainsKey(path))
                    {
                        store.Remove(path);
                    }
                    store.Add(path, new ActorItem(process, inbox, flags));
                    parent.Actor.LinkChild(process.Id.Path);

                    inbox.Startup(process, process.Parent, Cluster);
                    process.Startup();
                }
            }
            return this;
        }

        public ActorSystemState RemoveFromStore(ProcessId pid)
        {
            if (pid.IsValid)
            {
                var path = pid.Path;
                if (store.ContainsKey(path))
                {
                    store.Remove(path);
                }
            }
            return this;
        }

        public T FindInStore<T>(ProcessId pid, Func<ActorItem, T> Some, Func<T> None)
        {
            if (pid.IsValid)
            {
                var path = pid.Path;
                if (store.ContainsKey(path))
                {
                    var res = store[path];
                    return Some(res);
                }
                else
                {
                    return None();
                }
            }
            else
            {
                return None();
            }
        }

        public ProcessId Root =>
            root;

        public ProcessId User =>
            user;

        public ProcessId System =>
            system;

        public ProcessId NoSender =>
            noSender;

        public ProcessId Errors =>
            errors;

        public ProcessId DeadLetters =>
            deadLetters;

        public ProcessId Registered =>
            registered;

        private Unit ReplyInfo(string func, ProcessId to, object detail)
        {
            logInfo(String.Format("{0}: reply to {1} = {2}", func, to, detail ?? "[null]"));
            return unit;
        }

        private bool ProcessDoesNotExist(string func, ProcessId pid)
        {
            if (pid.IsValid && store.ContainsKey(pid.Path))
            {
                return false;
            }
            else
            {
                logErr(func + ": process doesn't exist: " + pid);
                return true;
            }
        }

        private bool ReplyToProcessDoesNotExist(string func)
        {
            if (ActorContext.CurrentRequest != null && ActorContext.CurrentRequest.ReplyTo.IsValid && store.ContainsKey(ActorContext.CurrentRequest.ReplyTo.Path))
            {
                return false;
            }
            else
            {
                logErr(func + ": ReplyTo process doesn't exist: " + ActorContext.CurrentRequest.ReplyTo.Path);
                return true;
            }
        }
    }
}
