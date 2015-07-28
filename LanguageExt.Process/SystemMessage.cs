using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public abstract class SystemMessage : Message
    {
        public override Message.Type MessageType => Message.Type.System;

        public static SystemMessage Restart => new SystemRestartMessage();
        public static SystemMessage LinkChild(ProcessId pid) => new SystemLinkChildMessage(pid);
        public static SystemMessage UnLinkChild(ProcessId pid) => new SystemUnLinkChildMessage(pid);
        public static SystemMessage ChildIsFaulted(ProcessId pid,Exception e) => new SystemChildIsFaultedMessage(pid,e);
    }

    internal class SystemRestartMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.Restart;
    }

    internal class SystemLinkChildMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.LinkChild;

        public SystemLinkChildMessage(ProcessId childId)
        {
            ChildId = childId;
        }
        public ProcessId ChildId { get; }
    }

    internal class SystemUnLinkChildMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.UnLinkChild;

        public SystemUnLinkChildMessage(ProcessId childId)
        {
            ChildId = childId;
        }
        public ProcessId ChildId { get; }
    }

    internal class SystemChildIsFaultedMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.ChildIsFaulted;

        public SystemChildIsFaultedMessage(ProcessId childId, Exception e)
        {
            ChildId = childId;
            Exception = e;
        }

        public ProcessId ChildId { get; }
        public Exception Exception { get; }
    }
}