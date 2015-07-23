using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal class ActorConfig
    {
        public readonly ProcessName RootProcessName           = "root";
        public readonly ProcessName SystemProcessName         = "system";
        public readonly ProcessName UserProcessName           = "user";
        public readonly ProcessName DeadLettersProcessName    = "dead-letters";
        public readonly ProcessName NoSenderProcessName       = "no-sender";
        public readonly ProcessName RegisteredProcessName     = "registered";
        public readonly ProcessName Errors                    = "errors";
        public readonly ProcessName AskReqRes                 = "ask-req-res";
        public readonly ProcessName InboxShutdown             = "inbox-shutdown";
        public readonly TimeSpan    Timeout                   = TimeSpan.FromSeconds(10);

        public static ActorConfig Default =>
            new ActorConfig();
    }
}
