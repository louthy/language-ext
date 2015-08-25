using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal abstract class SystemMessage : Message
    {
        public override Message.Type MessageType => Message.Type.System;
        public static SystemMessage LinkChild(ActorItem item) => new SystemLinkChildMessage(item);
        public static SystemMessage UnlinkChild(ProcessId pid) => new SystemUnLinkChildMessage(pid);
        public static SystemMessage ShutdownProcess = new ShutdownProcessMessage();
        public static SystemMessage Restart => new SystemRestartMessage();
    }

    internal class SystemRestartMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.Restart;
    }

    internal class SystemLinkChildMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.LinkChild;

        public SystemLinkChildMessage(ActorItem child)
        {
            Child = child;
        }
        public ActorItem Child { get; }
    }

    internal class SystemUnLinkChildMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.UnlinkChild;

        public SystemUnLinkChildMessage(ProcessId pid)
        {
            Child = pid;
        }
        public ProcessId Child { get; }
    }

    internal class ShutdownProcessMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.ShutdownProcess;

        public override string ToString() =>
            "ShutdownProcess";
    }
}