using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanguageExt.ProcessJS.Tests
{
    public static class PingPong
    {
        public static void Inbox(string msg)
        {
            if (msg == "ping")
            {
                ProcessLog.tellInfo("pong");
                Process.tell(Process.Sender, "pong");
            }
            else 
            {
                ProcessLog.tellInfo("ping");
                Process.tell(Process.Sender, "ping");
            }
        }
    }
}