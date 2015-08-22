using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    internal static class RelayActor
    {
        public static ProcessId Inbox(ProcessId hub, RelayMsg msg)
        {
            try
            {
                switch (msg.Tag)
                {
                    case RelayMsg.MsgTag.Connected:
                        SpawnConnection(msg, hub);
                        break;

                    case RelayMsg.MsgTag.Disconnected:
                        kill(Self[msg.ConnectionId]);
                        break;

                    case RelayMsg.MsgTag.Inbound:
                    case RelayMsg.MsgTag.Subscribe:
                    case RelayMsg.MsgTag.Unsubscribe:
                        fwd(SpawnConnection(msg, hub), msg);
                        break;
                }
            }
            catch (Exception e)
            {
                tell(Errors, e);
            }
            return hub;
        }

        private static ProcessId SpawnConnection(RelayMsg msg, ProcessId hub) =>
            Children.ContainsKey(msg.ConnectionId)
                ? Self[msg.ConnectionId]
                : spawn<ProcessId, RelayMsg>(msg.ConnectionId, () => hub, ConnectionInbox);

        public static ProcessId ConnectionInbox(ProcessId hub, RelayMsg rmsg)
        {
            switch (rmsg.Tag)
            {
                case RelayMsg.MsgTag.Inbound:
                    var inmsg = rmsg as InboundRelayMsg;
                    if (rmsg.IsAsk)
                    {
                        // Ask not supported
                        tell(Errors, "'ask' not supported from JS to server.");
                    }
                    else
                    {
                        tell(rmsg.To, inmsg.Message, rmsg.Sender.IsValid ? Self.Append(rmsg.Sender) : ProcessId.NoSender);
                    }
                    break;

                case RelayMsg.MsgTag.Outbound:
                    fwd(hub, rmsg);
                    break;

                case RelayMsg.MsgTag.Subscribe:
                    var pid          = rmsg.To;
                    var subscriber   = rmsg.Sender;
                    var connectionId = rmsg.ConnectionId;

                    ActorContext.SelfProcess.Actor.AddSubscription(
                        rmsg.To,
                        ActorContext.Observe<object>(pid).Subscribe(x =>
                            tell(hub, 
                                 new OutboundRelayMsg(
                                     connectionId,
                                     new RemoteMessageDTO {
                                        MessageId   = Guid.NewGuid(),
                                        Content     = JsonConvert.SerializeObject(x),
                                        Sender      = pid.Path,
                                        To          = subscriber.Path,
                                        ContentType = x.GetType().AssemblyQualifiedName,
                                        ReplyTo     = pid.Path,
                                        Tag         = (int)Message.TagSpec.User,
                                        Type        = (int)Message.Type.User
                                     },
                                     subscriber,
                                     pid,
                                     false),
                                 pid)));
                    break;

                case RelayMsg.MsgTag.Unsubscribe:
                    ActorContext.SelfProcess.Actor.RemoveSubscription(rmsg.To);
                    break;
            }
            return hub;
        }
    }

    public class RelayMsg
    {
        public enum MsgTag
        {
            Inbound,
            Outbound,
            Connected,
            Disconnected,
            Subscribe,
            Unsubscribe,
        }

        public readonly ProcessId To;
        public readonly ProcessId Sender;
        public readonly string ConnectionId;
        public readonly bool IsAsk;
        public readonly MsgTag Tag;

        public RelayMsg(MsgTag tag, string connectionId, ProcessId to, ProcessId sender, bool isAsk)
        {
            Tag = tag;
            ConnectionId = connectionId;
            IsAsk = isAsk;
            To = to;
            Sender = sender;
        }
    }

    public class InboundRelayMsg : RelayMsg
    {
        public readonly string Message;

        public InboundRelayMsg(string connectionId, string msg, ProcessId to, ProcessId sender, bool isAsk)
            :
            base(MsgTag.Inbound, connectionId, to, sender, isAsk)
        {
            Message = msg;
        }
    }

    public class OutboundRelayMsg : RelayMsg
    {
        public readonly RemoteMessageDTO Message;

        public OutboundRelayMsg(string connectionId, RemoteMessageDTO msg, ProcessId to, ProcessId sender, bool isAsk)
            :
            base(MsgTag.Outbound, connectionId, to, sender, isAsk)
        {
            Message = msg;
        }
    }
}
