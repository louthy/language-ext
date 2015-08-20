using Newtonsoft.Json;
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
        public readonly IActor RootProcess;
        public readonly ActorConfig Config;
        public readonly ProcessName RootProcessName;

        ProcessId root;
        ProcessId user;
        ProcessId system;
        ProcessId deadLetters;
        ProcessId registered;
        ProcessId errors;
        ProcessId inboxShutdown;
        ProcessId ask;
        ProcessId js;
        ProcessId reply;

        // We can use a mutable and non-locking dictionary here because access to 
        // it will be via the root actor's message-loop only
        Dictionary<string, ActorItem> store = new Dictionary<string, ActorItem>();

        public class ActorItem
        {
            public readonly IActor Actor;
            public readonly IActorInbox Inbox;
            public readonly ProcessFlags Flags;

            public ActorItem(
                IActor actor,
                IActorInbox inbox,
                ProcessFlags flags
                )
            {
                Actor = actor;
                Inbox = inbox;
                Flags = flags;
            }
        }

        public ActorSystemState(Option<ICluster> cluster, ProcessId rootId, IActor rootProcess, IActorInbox rootInbox, ProcessName rootProcessName, ActorConfig config)
        {
            root = rootId;
            Config = config;
            Cluster = cluster;
            RootProcess = rootProcess;
            RootInbox = rootInbox;
            RootProcessName = rootProcessName;

            RootProcess.Id.Child(Config.RegisteredProcessName);

            store.Add(Root.Path, new ActorItem(RootProcess, rootInbox, ProcessFlags.Default));
        }

        private ProcessName NodeName =>
            Cluster.Map(c => c.NodeName).IfNone("user");

        public ActorSystemState Startup()
        {
            logInfo("Process system starting up");

            // Top tier
            system          = ActorCreate<object>(root, Config.SystemProcessName, publish, ProcessFlags.Default);
            user            = ActorCreate<object>(root, Config.UserProcessName, publish, ProcessFlags.Default);
            registered      = ActorCreate<object>(root, Config.RegisteredProcessName, publish, ProcessFlags.Default);
            js              = ActorCreate<ProcessId, RelayMsg>(root, "js", RelayActor.Inbox, () => User["process-hub-js"], ProcessFlags.Default);

            // Second tier
            deadLetters = ActorCreate<DeadLetter>(system, Config.DeadLettersProcessName, publish, ProcessFlags.Default);
            errors          = ActorCreate<Exception>(system, Config.ErrorsProcessName, publish, ProcessFlags.Default);

            inboxShutdown   = ActorCreate<IActorInbox>(system, Config.InboxShutdownProcessName, inbox => inbox.Shutdown(), ProcessFlags.Default);

            reply = ask     = ActorCreate<Tuple<long, Dictionary<long, AskActorReq>>, object>(system, Config.AskProcessName, AskActor.Inbox, AskActor.Setup, ProcessFlags.ListenRemoteAndLocal);

            logInfo("Process system startup complete");

            return this;
        }

        public Unit Shutdown()
        {
            logInfo("Process system shutting down");

            ShutdownProcess(User);
            user = ActorCreate<object>(root, Config.UserProcessName, publish, ProcessFlags.Default);
            Tell(new ActorResponse(unit, unit.GetType().AssemblyQualifiedName, ActorContext.CurrentRequest.ReplyTo, Root, ActorContext.CurrentRequest.RequestId), reply, ActorContext.Root);

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
            ((ILocalActorInbox)inboxShutdown).Tell(inbox, ProcessId.NoSender);
            process.Shutdown();
            store.Remove(processId.Path);
        }

        public Unit Reply(ProcessId replyTo, object message, ProcessId sender, long requestId) =>
            Tell(new ActorResponse(message, message.GetType().AssemblyQualifiedName, replyTo, sender, requestId), replyTo, sender);

        public Unit GetChildren(ProcessId processId)
        {
            if (ReplyToProcessDoesNotExist(nameof(GetChildren))) return unit;
            if (ProcessDoesNotExist(nameof(GetChildren), processId)) return unit;

            Map<string, ProcessId> kids = store[processId.Path].Actor.Children;

            ReplyInfo(nameof(GetChildren), processId, kids.Count);

            return Tell( new ActorResponse(kids, kids.GetType().AssemblyQualifiedName, ActorContext.CurrentRequest.ReplyTo, processId, ActorContext.CurrentRequest.RequestId),
                         reply,
                         processId);
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
                var subject = new Subject<object>();
                stream = subject;
                var cluster = Cluster.IfNone(() => null);
                cluster.SubscribeToChannel(processId.Path + "-pubsub", type, msg => subject.OnNext(msg));
            }
            else
            {
                stream = item.Actor.PublishStream;
            }
            return Tell(
                    new ActorResponse(
                        stream,
                        stream.GetType().AssemblyQualifiedName,
                        ActorContext.CurrentRequest.ReplyTo,
                        processId, 
                        ActorContext.CurrentRequest.RequestId
                        ),
                    reply.Path,
                    processId);
        }

        internal Unit ObserveState(ProcessId processId)
        {
            if (ReplyToProcessDoesNotExist(nameof(ObservePub))) return unit;
            if (ProcessDoesNotExist(nameof(ObservePub), processId)) return unit;

            var val = store.ContainsKey(processId.Path)
                        ? store[processId.Path].Actor.StateStream
                        : Cluster.MatchUnsafe(c =>
                        {
                            // TODO: Specify the type
                            var subject = new Subject<object>();
                            c.SubscribeToChannel(processId.Path + "-pubsub", typeof(object), msg => subject.OnNext(msg));
                            return subject;
                        }, None: () => null);

            return Tell(
                new ActorResponse(
                    val,
                    val.GetType().AssemblyQualifiedName,
                    ActorContext.CurrentRequest.ReplyTo,
                    processId, 
                    ActorContext.CurrentRequest.RequestId
                    ),
                reply,
                processId);
        }

        private Unit TellDeadLetters(ProcessId processId, ProcessId sender, Message.TagSpec tag, object message)
        {
            var letter = DeadLetter.create(sender, processId, "Recipient doesn't exist", message);
            return ActorContext.WithContext(store[processId.Path].Actor, sender, null, message, () => Tell(letter, DeadLetters, sender));
        }

        public Unit Tell(ProcessId processId, ProcessId sender, Message.TagSpec tag, object message)
        {
            switch (tag)
            {
                case Message.TagSpec.Tell:            return Tell(message, processId, sender);
                case Message.TagSpec.TellUserControl: return TellUserControl((UserControlMessage)message, processId);
                case Message.TagSpec.TellSystem:      return TellSystem((SystemMessage)message, processId);
                default:                              return TellDeadLetters(processId, sender, tag, message);
            }
        }

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

            IActorInbox inbox = null;
            if ((flags & ProcessFlags.ListenRemoteAndLocal) == ProcessFlags.ListenRemoteAndLocal && Cluster.IsSome)
            {
                inbox = new ActorInboxDual<S, T>();
            }
            else if ((flags & ProcessFlags.PersistInbox) == ProcessFlags.PersistInbox && Cluster.IsSome)
            {
                inbox = new ActorInboxRemote<S, T>();
            }
            else
            {
                inbox = new ActorInboxLocal<S, T>();
            }
            AddOrUpdateStoreAndStartup(actor, inbox, flags);
            return actor.Id;
        }

        public ActorSystemState AddOrUpdateStoreAndStartup(IActor process, IActorInbox inbox, ProcessFlags flags)
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
                    try
                    {
                        store.Add(path, new ActorItem(process, inbox, flags));
                        parent.Actor.LinkChild(process.Id.Path);
                        process.Startup();
                        inbox.Startup(process, process.Parent, Cluster, 0);
                    }
                    catch (Exception e)
                    {
                        ShutdownProcess(path);
                        Tell(e, Errors, Root);
                        logSysErr(e);
                    }
                }
            }
            else
            {
                logSysErr("Failed to AddOrUpdateStoreAndStartup for "+ process.Id + " because parent process doesn't exist: "+ process.Parent.Path);
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

        public Unit Ask(object message, ProcessId pid, ProcessId sender) =>
            AskRemote(message, pid, sender, "user", Message.Type.User, (int)Message.TagSpec.UserAsk);

        public Unit Tell(object message, ProcessId pid, ProcessId sender) =>
            message is ActorRequest
                ? AskRemote( message, pid, sender, "user", Message.Type.User, (int)Message.TagSpec.UserAsk)
                : TellRemote( message, pid, sender, "user", Message.Type.User, (int)Message.TagSpec.User);

        public Unit TellUserControl(UserControlMessage message, ProcessId pid) =>
            TellRemote(
                message,
                pid,
                User,
                "user",
                Message.Type.UserControl,
                (int)message.Tag
                );

        public Unit TellSystem(SystemMessage message, ProcessId pid) =>
            TellRemote(
                message,
                pid,
                User,
                "system",
                Message.Type.System,
                (int)message.Tag
                );

        // TODO: TellRemote and AskRemote are basically same function
        public Unit TellRemote(object message, ProcessId pid, ProcessId sender, string inbox, Message.Type type, int tag)
        {
            var islocal = store.ContainsKey(pid.Path) && store[pid.Path].Inbox is ILocalActorInbox;
            if (islocal)
            {
                var local = (ILocalActorInbox)store[pid.Path].Inbox;
                return local.Tell(message, sender);
            }
            else
            {
                // TODO: Hack - generalise a relay proxy system
                if (pid.Path.StartsWith(js.Path))
                {
                    return TellRemoteJavascriptRelay(BuildRemoteMsg(message, pid, sender, (int)type, tag), pid, sender);
                }
                else
                {
                    return Cluster.Match(
                        Some: c => TellRemote(message, pid, sender, c, inbox, (int)type, tag),
                        None: () => Tell(DeadLetter.create(sender, pid, "Remote tell not available.  Cluster not registered.", message), DeadLetters, sender));
                }
            }
        }

        // TODO: TellRemote and AskRemote are basically same function
        public Unit AskRemote(object message, ProcessId pid, ProcessId sender, string inbox, Message.Type type, int tag)
        {
            var islocal = store.ContainsKey(pid.Path) && store[pid.Path].Inbox is ILocalActorInbox;
            if (islocal)
            {
                var local = (ILocalActorInbox)store[pid.Path].Inbox;
                return local.Ask(message, sender);
            }
            else
            {
                // TODO: Hack - generalise a relay proxy system
                if (pid.Path.StartsWith(js.Path))
                {
                    return TellRemoteJavascriptRelay(BuildRemoteMsg(message, pid, sender, (int)type, tag),pid,sender);
                }
                else
                {
                    return Cluster.Match(
                        Some: c => TellRemote(message, pid, sender, c, inbox, (int)type, tag),
                        None: () => Tell(DeadLetter.create(sender, pid, "Remote ask not available.  Cluster not registered.", message), DeadLetters, sender));
                }
            }
        }

        private ProcessId CheckIfRegistered(ProcessId pid) =>
            pid.Path.StartsWith(registered.Path)
                ? pid.Skip(1)
                : pid;

        // TODO: Hack - generalise a relay proxy system for both javascript via SignalR and Redis remote
        private Unit TellRemote(object message, ProcessId pid, ProcessId sender, ICluster c, string inbox, int type, int tag)
        {
            pid = CheckIfRegistered(pid);
            var dto = BuildRemoteMsg(message, pid, sender, type, tag);
            var userInbox = pid.Path + "-" + inbox + "-inbox";
            var userInboxNotify = userInbox + "-notify";
            c.Enqueue(userInbox, dto);
            long clientsReached = c.PublishToChannel(userInboxNotify, "New message");
            return unit;
        }

        private static RemoteMessageDTO BuildRemoteMsg(object message, ProcessId to, ProcessId sender, int type, int tag)
        {
            var req = message as ActorRequest;
            var res = message as ActorResponse;

            // TODO: Eurggh.  Refactor.
            var dto = new RemoteMessageDTO()
            {
                Type = type,
                Tag = req == null
                    ? res == null
                        ? tag
                        : (int)Message.TagSpec.UserReply
                    : (int)Message.TagSpec.UserAsk,
                Child = null,
                Exception = res == null
                    ? null
                    : res.IsFaulted
                        ? "RESPERR"
                        : null,
                To = to.Path,
                RequestId = req == null
                    ? res == null
                        ? -1
                        : res.RequestId
                    : req.RequestId,
                MessageId = Guid.NewGuid(),
                Sender = res == null
                    ? sender.ToString()
                    : res.ReplyFrom.ToString(),
                ReplyTo = req == null
                    ? res == null
                        ? sender.ToString()
                        : res.ReplyTo.ToString()
                    : req.ReplyTo.ToString(),
                ContentType = message == null
                    ? null
                    : req == null
                        ? res == null
                            ? message.GetType().AssemblyQualifiedName
                            : res.Message.GetType().AssemblyQualifiedName
                        : req.Message.GetType().AssemblyQualifiedName,
                Content = message == null
                    ? null
                    : JsonConvert.SerializeObject(req == null
                                                    ? res == null
                                                        ? message
                                                        : res.Message
                                                    : req.Message, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, TypeNameAssemblyFormat = global::System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full })
            };
            return dto;
        }

        private Unit TellRemoteJavascriptRelay(RemoteMessageDTO message, ProcessId pid, ProcessId sender)
        {
            // The standard structure for remote js relay paths are  "/root/js/{connection-id}/..."
            var connectionId = pid.Skip(2).Take(1).GetName().Value;
            message.To = pid.Skip(3).Path;

            var relay = new OutboundRelayMsg(connectionId, message, message.To, sender, message.RequestId != -1);
            return js[connectionId].Tell(relay, sender);
        }
    }
}
