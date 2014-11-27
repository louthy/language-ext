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

        public static SystemMessage Shutdown => new SystemShutdownMessage();
        public static SystemMessage Suspend => new SystemSuspendMessage();
        public static SystemMessage Restart => new SystemRestartMessage();
        public static SystemMessage LinkChild(ProcessId pid) => new SystemLinkChildMessage(pid);
        public static SystemMessage UnLinkChild(ProcessId pid) => new SystemUnLinkChildMessage(pid);
        public static SystemMessage ChildIsFaulted(ProcessId pid,Exception e) => new SystemChildIsFaultedMessage(pid,e);
    }

    internal class SystemShutdownMessage : SystemMessage
    {
    }

    internal class SystemSuspendMessage : SystemMessage
    {
    }

    internal class SystemRestartMessage : SystemMessage
    {
    }

    internal class SystemLinkChildMessage : SystemMessage
    {
        public SystemLinkChildMessage(ProcessId childId)
        {
            ChildId = childId;
        }
        public ProcessId ChildId { get; }
    }

    internal class SystemUnLinkChildMessage : SystemMessage
    {
        public SystemUnLinkChildMessage(ProcessId childId)
        {
            ChildId = childId;
        }
        public ProcessId ChildId { get; }
    }

    internal class SystemChildIsFaultedMessage : SystemMessage
    {
        public SystemChildIsFaultedMessage(ProcessId childId, Exception e)
        {
            ChildId = childId;
            Exception = e;
        }

        public ProcessId ChildId { get; }
        public Exception Exception { get; }
    }
}