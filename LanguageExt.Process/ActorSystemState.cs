using System;
using System.Collections.Generic;
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
        ProcessId askReqRes;

        // We can use a mutable and non-locking dictionary here because access to 
        // it will be via the root actor's message-loop only
        Dictionary<string, Tuple<IProcess, IActorInbox>> store = new Dictionary<string, Tuple<IProcess, IActorInbox>>();

        public ActorSystemState(Option<ICluster> cluster, ProcessId rootId, IProcess rootProcess, IActorInbox rootInbox, ActorConfig config)
        {
            root = rootId;
            Config = config;
            Cluster = cluster;
            RootProcess = rootProcess;
            RootInbox = rootInbox;

            store.Add(Root.Value, Tuple(RootProcess, rootInbox));
        }

        public ActorSystemState Startup()
        {
            logInfo("Process system starting up");

            // Top tier
            system = ActorCreate<Unit, object>(root, Config.SystemProcessName, Process.publish);
            user = ActorCreate<Unit, object>(root, Config.UserProcessName, Process.publish);

            // Second tier
            deadLetters     = ActorCreate<Unit, object>(system, Config.DeadLettersProcessName, Process.publish);
            errors          = ActorCreate<Unit, Exception>(system, Config.Errors, Process.publish);
            noSender        = ActorCreate<Unit, object>(system, Config.NoSenderProcessName, Process.publish);
            registered      = ActorCreate<Unit, object>(system, Config.RegisteredProcessName, Process.publish);

            inboxShutdown   = ActorCreate<Unit, IActorInbox>(system, Config.InboxShutdown, inbox => inbox.Shutdown());
            askReqRes       = ActorCreate<Tuple<long, Dictionary<long, AskActorReq>>, object>(system, Config.AskReqRes, AskActor.Inbox, AskActor.Setup);

            logInfo("Process system startup complete");

            return this;
        }

        public Unit Shutdown()
        {
            logInfo("Process system shutting down");

            ShutdownProcess(User);
            user = ActorCreate<Unit, object>(root, Config.UserProcessName, Process.publish);
            store[ActorContext.ReplyToId.Value].Item2.Tell(new ActorResponse(ActorContext.CurrentRequestId, unit), ActorContext.Root);

            logInfo("Process system shutdown complete");

            return unit;
        }

        public ActorSystemState ShutdownProcess(ProcessId processId)
        {
            if (ProcessDoesNotExist(nameof(ShutdownProcess), processId)) return this;

            var item = store[processId.Value];
            var process = item.Item1;
            var inbox = item.Item2;

            var parent = store[process.Parent.Value];
            parent.Item1.UnlinkChild(processId);

            ShutdownProcessRec(processId, store[inboxShutdown.Value].Item2);
            process.Shutdown();
            store.Remove(processId.Value);

            return this;
        }

        private void ShutdownProcessRec(ProcessId processId, IActorInbox inboxShutdown)
        {
            var item = store[processId.Value];
            var process = item.Item1;
            var inbox = item.Item2;

            foreach (var child in process.Children.Values)
            {
                ShutdownProcessRec(child, inboxShutdown);
            }
            inboxShutdown.Tell(inbox, ProcessId.NoSender);
            process.Shutdown();
            store.Remove(processId.Value);
        }

        public Unit Reply(object message, long requestid, ProcessId sender) =>
            store[askReqRes.Value].Item2.Tell(new ActorResponse(requestid, message), sender);

        public Unit GetChildren(ProcessId processId)
        {
            if (ReplyToProcessDoesNotExist(nameof(GetState))) return unit;
            if (ProcessDoesNotExist(nameof(GetState), processId)) return unit;

            Map<string, ProcessId> kids = store[processId.Value].Item1.Children;

            ReplyInfo(nameof(GetChildren), processId, kids.Count);

            return store[ActorContext.ReplyToId.Value].Item2.Tell(
                        new ActorResponse(ActorContext.CurrentRequestId, kids),
                        processId
                    );
        }

        internal Unit Publish(ProcessId processId, object message)
        {
            if (ProcessDoesNotExist(nameof(Publish), processId)) return unit;

            return store[processId.Value].Item1.Publish(message);
        }

        internal Unit GetState(ProcessId processId)
        {
            if (ReplyToProcessDoesNotExist(nameof(GetState))) return unit;
            if (ProcessDoesNotExist(nameof(GetState), processId)) return unit;

            object state = store[processId.Value].Item1.GetState();

            ReplyInfo(nameof(GetState), processId, state);

            return store[ActorContext.ReplyToId.Value].Item2.Tell(
                        new ActorResponse(ActorContext.CurrentRequestId, state),
                        processId);
        }

        internal Unit ObservePub(ProcessId processId)
        {
            if (ReplyToProcessDoesNotExist(nameof(ObservePub))) return unit;
            if (ProcessDoesNotExist(nameof(ObservePub), processId)) return unit;

            return store[ActorContext.ReplyToId.Value].Item2.Tell(
                new ActorResponse(
                    ActorContext.CurrentRequestId,
                    store[processId.Value].Item1.PublishStream
                    ),
                processId);
        }

        internal Unit ObserveState(ProcessId processId)
        {
            if (ReplyToProcessDoesNotExist(nameof(ObservePub))) return unit;
            if (ProcessDoesNotExist(nameof(ObservePub), processId)) return unit;

            return store[ActorContext.ReplyToId.Value].Item2.Tell(
                new ActorResponse(
                    ActorContext.CurrentRequestId,
                    store.ContainsKey(processId.Value)
                        ? store[processId.Value].Item1.StateStream
                        : null
                    ),
                processId);
        }

        private Unit TellDeadLetters(ProcessId processId, ProcessId sender, ActorSystemMessageTag tag, object message)
        {
            logWarn("Sending to DeadLetters, process (" + processId + ") doesn't exist.  Message: "+message);

            return FindInStore(DeadLetters,
                        Some: (process, inbox) => ActorContext.WithContext(processId, process.Parent, process.Children, sender, () => inbox.Tell(message, sender)),
                        None: () => unit);
        }

        public Unit Tell(ProcessId processId, ProcessId sender, ActorSystemMessageTag tag, object message) =>
            FindInStore(
                processId,
                Some: (process,inbox) => tag == ActorSystemMessageTag.TellSystem && message is SystemMessage           ? inbox.TellSystem(message as SystemMessage)
                                       : tag == ActorSystemMessageTag.TellUserControl && message is UserControlMessage ? inbox.TellUserControl(message as UserControlMessage)
                                       : ActorContext.WithContext(processId, process.Parent, process.Children, sender, () => inbox.Tell(message, sender)),
                None: () => TellDeadLetters(processId,sender,tag,message));

        public ProcessId ActorCreate<S, T>(ProcessId parent, ProcessName name, Func<T, Unit> actorFn)
        {
            return ActorCreate<S, T>(parent, name, (s, t) => { actorFn(t); return default(S); }, () => default(S));
        }

        public ProcessId ActorCreate<S, T>(ProcessId parent, ProcessName name, Action<T> actorFn)
        {
            return ActorCreate<S, T>(parent, name, (s, t) => { actorFn(t); return default(S); }, () => default(S));
        }

        public ProcessId ActorCreate<S, T>(ProcessId parent, ProcessName name, Func<S, T, S> actorFn, Func<S> setupFn)
        {
            if (ProcessDoesNotExist(nameof(ActorCreate), parent)) return ProcessId.None;

            var actor = new Actor<S, T>(Cluster, parent, name, actorFn, setupFn);
            var inbox = new ActorInbox<S, T>();
            AddOrUpdateStoreAndStartup(actor, inbox);
            return actor.Id;
        }

        public ActorSystemState AddOrUpdateStoreAndStartup(IProcess process, IActorInbox inbox)
        {
            if (store.ContainsKey(process.Parent.Value))
            {
                var parent = store[process.Parent.Value];

                if (process.Id.Value.Length > process.Parent.Value.Length &&
                    process.Id.Value.StartsWith(process.Parent.Value))
                {
                    var path = process.Id.Value;
                    if (store.ContainsKey(path))
                    {
                        store.Remove(path);
                    }
                    store.Add(path, Tuple(process, inbox));
                    parent.Item1.LinkChild(process.Id.Value);

                    inbox.Startup(process, process.Parent);
                    process.Startup();
                }
            }
            return this;
        }

        public ActorSystemState RemoveFromStore(ProcessId pid)
        {
            if (pid.IsValid)
            {
                var path = pid.Value;
                if (store.ContainsKey(path))
                {
                    store.Remove(path);
                }
            }
            return this;
        }

        public T FindInStore<T>(ProcessId pid, Func<IProcess, IActorInbox, T> Some, Func<T> None)
        {
            if (pid.IsValid)
            {
                var path = pid.Value;
                if (store.ContainsKey(path))
                {
                    var res = store[path];
                    return Some(res.Item1, res.Item2);
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

        public T FindInStore<T>(ProcessId pid, Func<IActorInbox, T> Some, Func<T> None)
        {
            if (pid.IsValid)
            {
                var path = pid.Value;
                if (store.ContainsKey(path))
                {
                    var res = store[path];
                    return Some(res.Item2);
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
            if (pid.IsValid && store.ContainsKey(pid.Value))
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
            if (ActorContext.ReplyToId.IsValid && store.ContainsKey(ActorContext.ReplyToId.Value))
            {
                return false;
            }
            else
            {
                logErr(func + ": ReplyTo process doesn't exist: " + ActorContext.ReplyToId.Value);
                return true;
            }
        }
    }
}
