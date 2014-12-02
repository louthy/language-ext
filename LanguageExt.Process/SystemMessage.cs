using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal enum SystemMessageTag
    {
        Shutdown,
        Restart,
        LinkChild,
        UnLinkChild,
        ChildIsFaulted
    }

    internal abstract class SystemMessage : Message
    {
        public override Message.Type MessageType => Message.Type.System;
        public abstract SystemMessageTag Tag { get; }

        public static SystemMessage Shutdown => new SystemShutdownMessage();
        public static SystemMessage Restart => new SystemRestartMessage();
        public static SystemMessage LinkChild(ProcessId pid) => new SystemLinkChildMessage(pid);
        public static SystemMessage UnLinkChild(ProcessId pid) => new SystemUnLinkChildMessage(pid);
        public static SystemMessage ChildIsFaulted(ProcessId pid,Exception e) => new SystemChildIsFaultedMessage(pid,e);
    }

    internal class SystemShutdownMessage : SystemMessage
    {
        public override SystemMessageTag Tag  => SystemMessageTag.Shutdown;
    }

    internal class SystemRestartMessage : SystemMessage
    {
        public override SystemMessageTag Tag => SystemMessageTag.Restart;
    }

    internal class SystemLinkChildMessage : SystemMessage
    {
        public override SystemMessageTag Tag => SystemMessageTag.LinkChild;

        public SystemLinkChildMessage(ProcessId childId)
        {
            ChildId = childId;
        }
        public ProcessId ChildId { get; }
    }

    internal class SystemUnLinkChildMessage : SystemMessage
    {
        public override SystemMessageTag Tag => SystemMessageTag.UnLinkChild;

        public SystemUnLinkChildMessage(ProcessId childId)
        {
            ChildId = childId;
        }
        public ProcessId ChildId { get; }
    }

    internal class SystemChildIsFaultedMessage : SystemMessage
    {
        public override SystemMessageTag Tag => SystemMessageTag.ChildIsFaulted;

        public SystemChildIsFaultedMessage(ProcessId childId, Exception e)
        {
            ChildId = childId;
            Exception = e;
        }

        public ProcessId ChildId { get; }
        public Exception Exception { get; }
    }
}