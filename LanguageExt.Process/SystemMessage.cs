using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt.UnitsOfMeasure;

namespace LanguageExt
{
    internal abstract class SystemMessage : Message
    {
        public override Message.Type MessageType => Message.Type.System;
        public static SystemMessage LinkChild(ActorItem item) => new SystemLinkChildMessage(item);
        public static SystemMessage UnlinkChild(ProcessId pid) => new SystemUnLinkChildMessage(pid);
        public static SystemMessage ChildFaulted(ProcessId pid, Exception ex) => new SystemChildFaultedMessage(pid, ex);
        public static SystemMessage ShutdownProcess = new ShutdownProcessMessage();
        public static SystemMessage Restart(Time when) => new SystemRestartMessage(when);
    }

    internal class SystemRestartMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.Restart;

        public SystemRestartMessage(Time when)
        {
            When = when;
        }
        public Time When { get; }
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

    internal class SystemChildFaultedMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.ChildFaulted;

        public SystemChildFaultedMessage(ProcessId child, Exception exception)
        {
            Child = child;
            Exception = exception;
        }
        public ProcessId Child { get; }
        public Exception Exception { get; }
    }

    internal class ShutdownProcessMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.ShutdownProcess;

        public override string ToString() =>
            "ShutdownProcess";
    }
}