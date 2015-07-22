using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal enum RootMessageTag
    {
        AddToStore,
        RemoveFromStore,
        Register,
        UnRegister,
        Tell,
        TellUserControl,
        TellSystem,
        ShutdownAll
    }

    internal abstract class RootMessage : Message
    {
        public override Message.Type MessageType => Message.Type.User;
        public abstract RootMessageTag Tag { get; }

        public static RootMessage AddToStore(IProcess process, IActorInbox inbox) => 
            new AddToStoreMessage(process,inbox);

        public static RootMessage RemoveFromStore(ProcessId pid) => 
            new RemoveFromStoreMessage(pid);

        public static RootMessage Tell(ProcessId pid, object message, ProcessId sender) =>
            new TellMessage(pid, message, sender, RootMessageTag.Tell);

        public static RootMessage TellUserControl(ProcessId pid, object message, ProcessId sender) =>
            new TellMessage(pid, message, sender, RootMessageTag.TellUserControl);

        public static RootMessage TellSystem(ProcessId pid, object message, ProcessId sender) =>
            new TellMessage(pid, message, sender, RootMessageTag.TellSystem);

        public static RootMessage ShutdownAll =>
            new ShutdownAllMessage();

        public static RootMessage Register(ProcessName name, ProcessId pid) =>
            new RegisterMessage(name, pid);

        public static RootMessage UnRegister(ProcessName name) =>
            new UnRegisterMessage(name);
    }

    internal class TellMessage : RootMessage
    {
        public override RootMessageTag Tag => tag;

        public readonly ProcessId ProcessId;
        public readonly object Message;
        public readonly ProcessId Sender;
        public readonly RootMessageTag tag;

        public TellMessage(ProcessId pid, object message, ProcessId sender, RootMessageTag tag)
        {
            ProcessId = pid;
            Message = message;
            Sender = sender;
            this.tag = tag;
        }
    }

    internal class AddToStoreMessage : RootMessage
    {
        public override RootMessageTag Tag  => RootMessageTag.AddToStore;

        public readonly IProcess Process;
        public readonly IActorInbox Inbox;

        public AddToStoreMessage(IProcess process, IActorInbox inbox)
        {
            Process = process;
            Inbox = inbox;
        }
    }

    internal class RemoveFromStoreMessage : RootMessage
    {
        public override RootMessageTag Tag => RootMessageTag.RemoveFromStore;

        public readonly ProcessId ProcessId;

        public RemoveFromStoreMessage(ProcessId processId)
        {
            ProcessId = processId;
        }
    }

    internal class RegisterMessage : RootMessage
    {
        public override RootMessageTag Tag => RootMessageTag.Register;

        public readonly ProcessName Name;
        public readonly ProcessId ProcessId;

        public RegisterMessage(ProcessName name, ProcessId processId)
        {
            Name = name;
            ProcessId = processId;
        }
    }

    internal class UnRegisterMessage : RootMessage
    {
        public override RootMessageTag Tag => RootMessageTag.UnRegister;

        public readonly ProcessName Name;

        public UnRegisterMessage(ProcessName name)
        {
            Name = name;
        }
    }

    internal class ShutdownAllMessage : RootMessage
    {
        public override RootMessageTag Tag => RootMessageTag.ShutdownAll;

        public ShutdownAllMessage()
        {
        }
    }
}