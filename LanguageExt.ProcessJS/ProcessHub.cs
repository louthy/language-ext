using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    public class ProcessHub : Hub
    {
        static object sync = new object();
        static ProcessId processHub;

        private static ProcessId FixRootName(string pid)
        {
            ProcessId cpid = pid;
            return cpid.Take(1).GetName().Value == "root"
                ? Root.Append(cpid.Skip(1))
                : cpid;
        }

        private static ProcessId FixSender(string sender) =>
            String.IsNullOrWhiteSpace(sender) || sender == "/no-sender"
                ? ProcessId.NoSender
                : sender;

        public void Tell(string pid, string message, string sender)
        {
            var spid = FixRootName(pid);
            tell(Root["js"], new InboundRelayMsg(Context.ConnectionId, message, spid, sender, false), FixSender(sender));
        }

        public void Ask(string pid, string message, string sender)
        {
            var spid = FixRootName(pid);
            tell(Root["js"], new InboundRelayMsg(Context.ConnectionId, message, spid, sender, true), FixSender(sender));
        }

        private ProcessId EnsureProcessHub()
        {
            if (processHub.IsValid)
            {
                return processHub;
            }
            else
            {
                lock (sync)
                {
                    if (processHub.IsValid) return processHub;
                    processHub = spawn<OutboundRelayMsg>("process-hub-js", msg =>
                    {
                        var conns = new List<string>();
                        conns.Add(msg.ConnectionId);
                        GlobalHost.ConnectionManager.GetHubContext<ProcessHub>().Clients.Clients(conns).onMessage(msg.Message);
                    });
                    return processHub;
                }
            }
        }

        public override Task OnConnected()
        {
            EnsureProcessHub();
            tell(Root["js"], new RelayMsg(RelayMsg.MsgTag.Connected, Context.ConnectionId, ProcessId.NoSender, ProcessId.NoSender, false));
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            tell(Root["js"], new RelayMsg(RelayMsg.MsgTag.Disconnected, Context.ConnectionId, ProcessId.NoSender, ProcessId.NoSender, false));
            return base.OnDisconnected(stopCalled);
        }
    }
}
