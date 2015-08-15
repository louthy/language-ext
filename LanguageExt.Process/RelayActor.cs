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
                        kill(msg.ConnectionId);
                        break;

                    case RelayMsg.MsgTag.Inbound:
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

        private static ProcessId SpawnConnection(RelayMsg msg, ProcessId hub)
        {
            if (!Children.ContainsKey(msg.ConnectionId))
            {
                spawn<ProcessId, RelayMsg>(msg.ConnectionId, () => hub, ConnectionInbox);
            }
            return Children[msg.ConnectionId];
        }

        public static ProcessId ConnectionInbox(ProcessId hub, RelayMsg msg)
        {
            switch (msg.Tag)
            {
                case RelayMsg.MsgTag.Inbound:
                    var inmsg = msg as InboundRelayMsg;
                    if (msg.IsAsk)
                    {
                        // TODO: Ask 
                    }
                    else
                    {
                        tell(msg.To, inmsg.Message, Self.Append(msg.Sender));
                    }
                    break;

                case RelayMsg.MsgTag.Outbound:
                    fwd(hub, msg);
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
            Disconnected
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
