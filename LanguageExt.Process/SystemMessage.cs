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

    internal class SystemChildIsFaultedMessage : SystemMessage
    {
        public SystemChildIsFaultedMessage(ProcessName childName, ProcessId childId, Exception e)
        {
            ChildName = childName;
            ChildId = childId;
            Exception = e;
        }

        public ProcessName ChildName { get; }
        public ProcessId ChildId { get; }
        public Exception Exception { get; }
    }
}