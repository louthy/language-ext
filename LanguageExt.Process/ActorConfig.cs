using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal class ActorConfig
    {
        public ProcessName RootProcessName           => "root";
        public ProcessName SystemProcessName         => "system";
        public ProcessName UserProcessName           => "user";
        public ProcessName DeadLettersProcessName    => "dead-letters";
        public ProcessName NoSenderProcessName       => "no-sender";
        public ProcessName RegisteredProcessName     => "registered";
        public ProcessName Errors                    => "errors";

        public static ActorConfig Default =>
            new ActorConfig();
    }
}
