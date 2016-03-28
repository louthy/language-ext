using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
using System.Reflection;

namespace LanguageExt
{
    static class ActorInboxCommon
    {
        static FSharpFunc<A,B> ToFSharpFunc<A, B>(Func<A, B> f)
        {
#if COREFX
            return (FSharpFunc<A, B>)typeof(FSharpFunc<A, B>).GetTypeInfo().GetDeclaredMethod("op_Implicit").Invoke(null, new[] { f });
#else
            return FuncConvert.ToFSharpFunc<A,B>(new Converter<A, B>(f));
#endif
        }

        public static FSharpAsync<A> CreateAsync<A>(Func<Task<A>> f) =>
            FSharpAsync.FromContinuations(
                FuncConvert.ToFSharpFunc<Tuple<FSharpFunc<A, Microsoft.FSharp.Core.Unit>, FSharpFunc<Exception, Microsoft.FSharp.Core.Unit>, FSharpFunc<OperationCanceledException, Microsoft.FSharp.Core.Unit>>>(
                    conts =>
                    {
                        f().ContinueWith(task =>
                        {
                            try { conts.Item1.Invoke(task.Result); }
                            catch (Exception e) { conts.Item2.Invoke(e); }
                        });
                    }));

        public static FSharpMailboxProcessor<TMsg> Mailbox<TMsg>(CancellationToken cancelToken, Action<TMsg> handler)
        {
            var body = ToFSharpFunc<FSharpMailboxProcessor<TMsg>, FSharpAsync<Microsoft.FSharp.Core.Unit>>(
                mbox =>
                    CreateAsync<Microsoft.FSharp.Core.Unit>(async () =>
                    {
                        while (!cancelToken.IsCancellationRequested)
                        {
                            try
                            {
                                var msg = await FSharpAsync.StartAsTask(mbox.Receive(FSharpOption<int>.None), FSharpOption<TaskCreationOptions>.None, FSharpOption<CancellationToken>.Some(cancelToken));
                                if (notnull(msg) && !cancelToken.IsCancellationRequested)
                                {
                                    handler(msg);
                                }
                            }
                            catch (TaskCanceledException)
                            {
                                // We're being shutdown, ignore.
                            }
                            catch (Exception e)
                            {
                                logSysErr(e);
                            }
                        }
                        return null;
                    })
            );

            return FSharpMailboxProcessor<TMsg>.Start(body, FSharpOption<CancellationToken>.None);
        }

        public static FSharpMailboxProcessor<string> StartNotifyMailbox(CancellationToken cancelToken, Action<string> handler) =>
            Mailbox<string>(cancelToken, msg =>
            {
                try
                {
                    handler(msg);
                }
                catch (Exception e)
                {
                    logSysErr(e);
                }
            });


        public static InboxDirective SystemMessageInbox<S,T>(Actor<S,T> actor, IActorInbox inbox, SystemMessage msg, ActorItem parent)
        {
            return ActorContext.WithContext(new ActorItem(actor,inbox,actor.Flags), parent, ProcessId.NoSender, null, msg, msg.SessionId, () =>
            {
                switch (msg.Tag)
                {
                    case Message.TagSpec.Restart:
                        if (inbox.IsPaused)
                        {
                            inbox.Unpause();
                        }
                        actor.Restart();
                        break;

                    case Message.TagSpec.LinkChild:
                        var lc = msg as SystemLinkChildMessage;
                        actor.LinkChild(lc.Child);
                        break;

                    case Message.TagSpec.UnlinkChild:
                        var ulc = msg as SystemUnLinkChildMessage;
                        actor.UnlinkChild(ulc.Child);
                        break;

                    case Message.TagSpec.ChildFaulted:
                        var cf = msg as SystemChildFaultedMessage;
                        return actor.ChildFaulted(cf.Child, cf.Sender, cf.Exception, cf.Message);

                    case Message.TagSpec.StartupProcess:
                        actor.Startup();
                        break;

                    case Message.TagSpec.ShutdownProcess:
                        var sp = msg as ShutdownProcessMessage;
                        actor.ShutdownProcess(sp.MaintainState);
                        break;

                    case Message.TagSpec.Unpause:
                        inbox.Unpause();
                        break;

                    case Message.TagSpec.Pause:
                        inbox.Pause();
                        return InboxDirective.Pause;

                    case Message.TagSpec.Watch:
                        var awm = msg as SystemAddWatcherMessage;
                        actor.AddWatcher(awm.Id);
                        break;

                    case Message.TagSpec.UnWatch:
                        var rwm = msg as SystemRemoveWatcherMessage;
                        actor.RemoveWatcher(rwm.Id);
                        break;

                    case Message.TagSpec.DispatchWatch:
                        var dwm = msg as SystemDispatchWatchMessage;
                        actor.DispatchWatch(dwm.Id);
                        break;

                    case Message.TagSpec.DispatchUnWatch:
                        var duwm = msg as SystemDispatchUnWatchMessage;
                        actor.DispatchUnWatch(duwm.Id);
                        break;
                }
                return InboxDirective.Default;
            });
        }

        public static InboxDirective UserMessageInbox<S, T>(Actor<S, T> actor, IActorInbox inbox, UserControlMessage msg, ActorItem parent)
        {
            switch (msg.Tag)
            {
                case Message.TagSpec.UserAsk:
                    var rmsg = (ActorRequest)msg;
                    return ActorContext.WithContext(new ActorItem(actor, inbox, actor.Flags), parent, rmsg.ReplyTo, rmsg, msg, msg.SessionId, () => actor.ProcessAsk(rmsg));

                case Message.TagSpec.UserReply:
                    var urmsg = (ActorResponse)msg;
                    ActorContext.WithContext(new ActorItem(actor, inbox, actor.Flags), parent, urmsg.ReplyFrom, null, msg, msg.SessionId, () => actor.ProcessResponse(urmsg));
                    break;

                case Message.TagSpec.UserTerminated:
                    var utmsg = (TerminatedMessage)msg;
                    return ActorContext.WithContext(new ActorItem(actor, inbox, actor.Flags), parent, utmsg.Id, null, msg, msg.SessionId, () => actor.ProcessTerminated(utmsg.Id));

                case Message.TagSpec.User:
                    var umsg = (UserMessage)msg;
                    return ActorContext.WithContext(new ActorItem(actor, inbox, actor.Flags), parent, umsg.Sender, null, msg, msg.SessionId, () => actor.ProcessMessage(umsg.Content));

                case Message.TagSpec.ShutdownProcess:
                    kill(actor.Id);
                    break;
            }
            return InboxDirective.Default;
        }

        public static Option<UserControlMessage> PreProcessMessage<T>(ProcessId sender, ProcessId self, object message)
        {
            if (message == null)
            {
                var emsg = $"Message is null for tell (expected {typeof(T)})";
                tell(ActorContext.DeadLetters, DeadLetter.create(sender, self, emsg, message));
                return None;
            }

            if (message is ActorRequest)
            {
                var req = (ActorRequest)message;
                if (!(req.Message is T) && !(req.Message is Message))
                {
                    var emsg = $"Invalid message type for ask (expected {typeof(T)})";
                    tell(ActorContext.DeadLetters, DeadLetter.create(sender, self, emsg, message));

                    ActorContext.Tell(
                        sender,
                        new ActorResponse(new Exception($"Invalid message type for ask (expected {typeof(T)})"),
                        typeof(Exception).AssemblyQualifiedName,
                        sender,
                        self,
                        req.RequestId,
                        true
                        ), 
                        self
                    );

                    return None;
                }
                return Optional((UserControlMessage)message);
            }

            return new UserMessage(message, sender, sender);
        }

        public static Option<Tuple<RemoteMessageDTO, Message>> GetNextMessage(ICluster cluster, ProcessId self, string key)
        {
            Message msg = null;
            RemoteMessageDTO dto = null;

            dto = null;
            do
            {
                dto = cluster.Peek<RemoteMessageDTO>(key);
                if (dto == null)
                {
                    // Queue is empty
                    return None; 
                }
                if (dto.Tag == 0 && dto.Type == 0)
                {
                    // Message is bad
                    cluster.Dequeue<RemoteMessageDTO>(key);
                    tell(ActorContext.DeadLetters, DeadLetter.create(dto.Sender, self, null, "Failed to deserialise message: ", dto));
                    if (cluster.QueueLength(key) == 0) return None;
                }
            }
            while (dto == null || dto.Tag == 0 || dto.Type == 0);

            try
            {
                msg = MessageSerialiser.DeserialiseMsg(dto, self);
            }
            catch (Exception e)
            {
                // Message can't be deserialised
                cluster.Dequeue<RemoteMessageDTO>(key);
                tell(ActorContext.DeadLetters, DeadLetter.create(dto.Sender, self, e, "Failed to deserialise message: ", msg));
                return None;
            }

            return Some(Tuple(dto, msg));
        }

        public static string ClusterKey(ProcessId pid) =>
            pid.Path;

        public static string ClusterSettingsKey(ProcessId pid) =>
            ClusterKey(pid) + "@settings";

        public static string ClusterInboxKey(ProcessId pid, string type) =>
            ClusterKey(pid) + "-" + type + "-inbox";

        public static string ClusterUserInboxKey(ProcessId pid) =>
            ClusterInboxKey(pid, "user");

        public static string ClusterSystemInboxKey(ProcessId pid) =>
            ClusterInboxKey(pid, "system");

        public static string ClusterInboxNotifyKey(ProcessId pid, string type) =>
            ClusterInboxKey(pid, type) + "-notify";

        public static string ClusterUserInboxNotifyKey(ProcessId pid) =>
            ClusterInboxNotifyKey(pid, "user");

        public static string ClusterSystemInboxNotifyKey(ProcessId pid) =>
            ClusterInboxNotifyKey(pid, "system");

        public static string ClusterMetaDataKey(ProcessId pid) =>
            ClusterKey(pid) + "-metadata";

        public static string ClusterPubSubKey(ProcessId pid) =>
            ClusterKey(pid) + "-pubsub";

        public static string ClusterStatePubSubKey(ProcessId pid) =>
            ClusterKey(pid) + "-state-pubsub";
    }
}
