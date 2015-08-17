using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    internal class ActorInboxCommon
    {
        public static FSharpAsync<A> CreateAsync<A>(Func<Task<A>> f) =>
            FSharpAsync.FromContinuations<A>(
                FuncConvert.ToFSharpFunc<Tuple<FSharpFunc<A, Microsoft.FSharp.Core.Unit>, FSharpFunc<Exception, Microsoft.FSharp.Core.Unit>, FSharpFunc<OperationCanceledException, Microsoft.FSharp.Core.Unit>>>(
                    conts =>
                    {
                        f().ContinueWith(task =>
                        {
                            try { conts.Item1.Invoke(task.Result); }
                            catch (Exception e) { conts.Item2.Invoke(e); }
                        });
                    }));

        public static FSharpMailboxProcessor<TMsg> Mailbox<S,T, TMsg>(Option<ICluster> cluster, ProcessFlags flags, CancellationToken cancelToken, Action<TMsg> handler)
            where TMsg : Message
        {
            var body = FuncConvert.ToFSharpFunc<FSharpMailboxProcessor<TMsg>, FSharpAsync<Microsoft.FSharp.Core.Unit>>(
                mbox =>
                    CreateAsync<Microsoft.FSharp.Core.Unit>(async () =>
                    {
                        while (!cancelToken.IsCancellationRequested)
                        {
                            try
                            {
                                var msg = await FSharpAsync.StartAsTask(mbox.Receive(FSharpOption<int>.None), FSharpOption<TaskCreationOptions>.None, FSharpOption<CancellationToken>.Some(cancelToken));
                                if (msg != null && !cancelToken.IsCancellationRequested)
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

        public static void SystemMessageInbox<S,T>(Actor<S,T> actor, SystemMessage msg)
        {
            ActorContext.WithContext(actor, ProcessId.NoSender, null, msg, () =>
            {
                switch (msg.Tag)
                {
                    case Message.TagSpec.ChildIsFaulted:
                        // TODO: Add extra strategy behaviours here
                        var scifm = (SystemChildIsFaultedMessage)msg;
                        tell(scifm.ChildId, SystemMessage.Restart);
                        tell(ActorContext.Errors, scifm.Exception);
                        break;

                    case Message.TagSpec.Restart:
                        actor.Restart();
                        break;

                    case Message.TagSpec.LinkChild:
                        var slcm = (SystemLinkChildMessage)msg;
                        actor.LinkChild(slcm.ChildId);
                        break;

                    case Message.TagSpec.UnLinkChild:
                        var ulcm = (SystemUnLinkChildMessage)msg;
                        actor.UnlinkChild(ulcm.ChildId);
                        break;
                }
            });
        }

        public static void UserMessageInbox<S, T>(Actor<S, T> actor, UserControlMessage msg)
        {
            if (msg.Tag == Message.TagSpec.UserAsk)
            {
                var rmsg = (ActorRequest)msg;
                ActorContext.WithContext(actor, rmsg.ReplyTo, rmsg, msg, () => actor.ProcessAsk(rmsg));
            }
            else if (msg.Tag == Message.TagSpec.UserReply)
            {
                var rmsg = (ActorResponse)msg;
                ActorContext.WithContext(actor, rmsg.ReplyFrom, null, msg, () => actor.ProcessMessage(msg));
            }
            else if (msg.Tag == Message.TagSpec.User)
            {
                var umsg = (UserMessage)msg;
                ActorContext.WithContext(actor, umsg.Sender, null, msg, () => actor.ProcessMessage(umsg.Content));
            }
            else if (msg.MessageType == Message.Type.UserControl)
            {
                switch (msg.Tag)
                {
                    case Message.TagSpec.Shutdown:
                        kill(actor.Id);
                        break;
                }
            }
        }

        public static Option<UserControlMessage> PreProcessMessage<T>(ProcessId sender, ProcessId self, object message)
        {
            if (message == null)
            {
                tell(ActorContext.DeadLetters, DeadLetter.create(sender, self, "Message is null for tell (expected " + typeof(T) + ")", message));
                return None;
            }

            if (message is ActorRequest)
            {
                var req = (ActorRequest)message;
                if (!typeof(T).IsAssignableFrom(req.Message.GetType()))
                {
                    tell(ActorContext.DeadLetters, DeadLetter.create(sender, self, "Invalid message type for ask (expected " + typeof(T) + ")", message));
                    return None;
                }
                return Optional((UserControlMessage)message);
            }

            if (typeof(T) != typeof(string) && message is string)
            {
                try
                {
                    // This allows for messages to arrive from JS and be dealt with at the endpoint 
                    // (where the type is known) rather than the gateway (where it isn't)
                    return Optional(new UserMessage(JsonConvert.DeserializeObject<T>((string)message), sender, sender) as UserControlMessage);
                }
                catch
                {
                    try
                    {
                        // Final attempt
                        return Optional(new UserMessage(JsonConvert.DeserializeObject<RemoteMessageDTO>((string)message), sender, sender) as UserControlMessage);
                    }
                    catch
                    {
                        tell(ActorContext.DeadLetters, DeadLetter.create(sender, self, "Invalid message type for tell (expected " + typeof(T) + ")", message));
                        return None;
                    }
                }
            }

            if (!typeof(T).IsAssignableFrom(message.GetType()))
            {
                tell(ActorContext.DeadLetters, DeadLetter.create(sender, self, "Invalid message type for tell (expected " + typeof(T) + ")", message));
                return None;
            }

            return new UserMessage(message, sender, sender);
        }

        public static Option<Tuple<RemoteMessageDTO, Message>> GetNextMessage(ICluster cluster, ProcessId self, string key)
        {
            Message msg = null;
            RemoteMessageDTO dto = null;
            do
            {
                dto = null;
                do
                {
                    dto = cluster.Peek<RemoteMessageDTO>(key);
                    if (dto == null || (dto.Tag == 0 && dto.Type == 0))
                    {
                        cluster.Dequeue<RemoteMessageDTO>(key);
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
                    logSysErr("Failed to deserialise message for " + self + " (dropping)", e);
                }
            }
            while (msg == null);

            return Some(Tuple(dto, msg));
        }
    }
}
