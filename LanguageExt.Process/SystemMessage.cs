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
        public static SystemMessage ChildFaulted(ProcessId pid, ProcessId sender, Exception ex, object msg) => new SystemChildFaultedMessage(pid, sender, ex, msg);
        public static SystemMessage ShutdownProcess => new ShutdownProcessMessage();
        public static SystemMessage StartupProcess => new StartupProcessMessage();
        public static SystemMessage Restart => new SystemRestartMessage();
        public static SystemMessage Pause => new SystemPauseMessage();
        public static SystemMessage Unpause => new SystemUnpauseMessage();
        public static SystemMessage Watch(ProcessId pid) => new SystemAddWatcherMessage(pid);
        public static SystemMessage UnWatch(ProcessId pid) => new SystemRemoveWatcherMessage(pid);
        public static SystemMessage DispatchWatch(ProcessId pid) => new SystemDispatchWatchMessage(pid);
        public static SystemMessage DispatchUnWatch(ProcessId pid) => new SystemDispatchUnWatchMessage(pid);
    }

    class StartupProcessMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.StartupProcess;
    }

    class SystemPauseMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.Pause;
    }

    class SystemUnpauseMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.Unpause;
    }

    class SystemNullMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.Null;
    }

    class SystemRestartMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.Restart;
    }

    class SystemLinkChildMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.LinkChild;

        public SystemLinkChildMessage(ActorItem child)
        {
            Child = child;
        }
        public ActorItem Child { get; }
    }

    class SystemUnLinkChildMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.UnlinkChild;

        public SystemUnLinkChildMessage(ProcessId pid)
        {
            Child = pid;
        }
        public ProcessId Child { get; }
    }

    class SystemChildFaultedMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.ChildFaulted;

        public SystemChildFaultedMessage(ProcessId child, ProcessId sender, Exception exception, object message)
        {
            Child = child;
            Sender = sender;
            Exception = exception;
            Message = message;
        }
        public ProcessId Child { get; }
        public ProcessId Sender { get; }
        public Exception Exception { get; }
        public object Message { get; }
    }

    class ShutdownProcessMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.ShutdownProcess;

        public override string ToString() =>
            "ShutdownProcess";
    }

    class SystemAddWatcherMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.Watch;

        public SystemAddWatcherMessage(ProcessId id)
        {
            Id = id;
        }
        public ProcessId Id { get; }
    }

    class SystemRemoveWatcherMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.UnWatch;

        public SystemRemoveWatcherMessage(ProcessId id)
        {
            Id = id;
        }
        public ProcessId Id { get; }
    }

    class SystemDispatchWatchMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.DispatchWatch;

        public SystemDispatchWatchMessage(ProcessId id)
        {
            Id = id;
        }
        public ProcessId Id { get; }
    }

    class SystemDispatchUnWatchMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.DispatchUnWatch;

        public SystemDispatchUnWatchMessage(ProcessId id)
        {
            Id = id;
        }
        public ProcessId Id { get; }
    }
}