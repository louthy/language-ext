using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal enum ActorSystemMessageTag
    {
        Startup,
        AddToStore,
        RemoveFromStore,
        Register,
        Deregister,
        Ask,
        Reply,
        Tell,
        TellUserControl,
        TellSystem,
        ShutdownProcess,
        GetChildren,
        ObservePub,
        ObserveState,
        Publish,
        ShutdownAll
    }

    internal abstract class ActorSystemMessage : Message
    {
        public override Message.Type MessageType => Message.Type.User;
        public abstract ActorSystemMessageTag Tag { get; }

        public readonly static ActorSystemMessage Startup = 
            new StartupMessage();

        public static ActorSystemMessage AddToStore(IProcess process, IActorInbox inbox, ProcessFlags flags) =>
            new AddToStoreMessage(process, inbox, flags);

        public static ActorSystemMessage RemoveFromStore(ProcessId pid) => 
            new RemoveFromStoreMessage(pid);

        public static ActorSystemMessage Tell(ProcessId pid, object message, ProcessId sender) =>
            new TellMessage(pid, message, sender, ActorSystemMessageTag.Tell);

        public static ActorSystemMessage TellUserControl(ProcessId pid, object message, ProcessId sender) =>
            new TellMessage(pid, message, sender, ActorSystemMessageTag.TellUserControl);

        public static ActorSystemMessage TellSystem(ProcessId pid, object message, ProcessId sender) =>
            new TellMessage(pid, message, sender, ActorSystemMessageTag.TellSystem);

        public static ActorSystemMessage Publish(ProcessId pid, object message) =>
            new PubMessage(pid, message);

        public static ActorSystemMessage ShutdownProcess(ProcessId pid) =>
            new ShutdownProcessMessage(pid);

        public readonly static ActorSystemMessage ShutdownAll =
            new ShutdownAllMessage();

        public static ActorSystemMessage GetChildren(ProcessId pid) =>
            new GetChildrenMessage(pid);

        public static ActorSystemMessage Register(ProcessName name, ProcessId pid) =>
            new RegisterMessage(name, pid);

        public static ActorSystemMessage Deregister(ProcessName name) =>
            new DeregisterMessage(name);

        public static ActorSystemMessage Reply(ProcessId replyTo, object message, long requestId) =>
            new ReplyMessage(replyTo, message, ActorContext.Self, requestId);

        public static ActorSystemMessage ObservePub(ProcessId pid, System.Type type) =>
            new ObservePubMessage(pid, type);

        public static ActorSystemMessage ObserveState(ProcessId pid) =>
            new ObserveStateMessage(pid);
    }

    internal class GetChildrenMessage: ActorSystemMessage
    {
        public override ActorSystemMessageTag Tag => ActorSystemMessageTag.GetChildren;

        public readonly ProcessId ProcessId;

        [JsonConstructor]
        public GetChildrenMessage(ProcessId processId)
        {
            ProcessId = processId;
        }

        public override string ToString() => 
            "GetChildren pid: " + ProcessId;
    }

    internal class StartupMessage : ActorSystemMessage
    {
        public override ActorSystemMessageTag Tag => ActorSystemMessageTag.Startup;

        public override string ToString() =>
            "Startup";
    }

    internal class ShutdownProcessMessage : ActorSystemMessage
    {
        public override ActorSystemMessageTag Tag => ActorSystemMessageTag.ShutdownProcess;

        public readonly ProcessId ProcessId;

        [JsonConstructor]
        public ShutdownProcessMessage(ProcessId processId)
        {
            ProcessId = processId;
        }

        public override string ToString() =>
            "ShutdownProcess pid:" + ProcessId;
    }

    internal class ObservePubMessage : ActorSystemMessage
    {
        public override ActorSystemMessageTag Tag => ActorSystemMessageTag.ObservePub;

        public readonly ProcessId ProcessId;
        public readonly System.Type MsgType;

        [JsonConstructor]
        public ObservePubMessage(ProcessId pid, System.Type msgType)
        {
            ProcessId = pid;
            MsgType = msgType;
        }

        public override string ToString() =>
            "Observe pub pid: " + ProcessId;
    }

    internal class ObserveStateMessage : ActorSystemMessage
    {
        public override ActorSystemMessageTag Tag => ActorSystemMessageTag.ObserveState;

        public readonly ProcessId ProcessId;

        [JsonConstructor]
        public ObserveStateMessage(ProcessId processId)
        {
            ProcessId = processId;
        }

        public override string ToString() =>
            "Observe state pid: " + ProcessId;
    }

    internal class ReplyMessage : ActorSystemMessage
    {
        public override ActorSystemMessageTag Tag => ActorSystemMessageTag.Reply;

        public readonly ProcessId ReplyTo;
        public readonly object Message;
        public readonly ProcessId Sender;
        public readonly long RequestId;

        [JsonConstructor]
        public ReplyMessage(ProcessId replyTo, object message, ProcessId sender, long requestId)
        {
            ReplyTo = replyTo;
            Message = message;
            Sender = sender;
            RequestId = requestId;
        }

        public override string ToString() =>
            "Reply to: " + ReplyTo + " msg: "+Message + " from: "+Sender;
    }

    internal class PubMessage : ActorSystemMessage
    {
        public override ActorSystemMessageTag Tag => ActorSystemMessageTag.Publish;

        public readonly ProcessId ProcessId;
        public readonly object Message;

        [JsonConstructor]
        public PubMessage(ProcessId processId, object message)
        {
            ProcessId = processId;
            Message = message;
        }

        public override string ToString() =>
            "Pub pid: " + ProcessId + " msg: " + Message;
    }

    internal class TellMessage : ActorSystemMessage
    {
        public override ActorSystemMessageTag Tag => tag;

        public readonly ProcessId ProcessId;
        public readonly object Message;
        public readonly ProcessId Sender;
        readonly ActorSystemMessageTag tag;

        [JsonConstructor]
        public TellMessage(ProcessId processId, object message, ProcessId sender, ActorSystemMessageTag tag)
        {
            ProcessId = processId;
            Message = message;
            Sender = sender;
            this.tag = tag;
        }

        public override string ToString() =>
            "Tell: pid: " + ProcessId + " sndr: "+Sender+" msg: " + Message;
    }

    internal class AddToStoreMessage : ActorSystemMessage
    {
        public override ActorSystemMessageTag Tag  => ActorSystemMessageTag.AddToStore;

        public readonly IProcess Process;
        public readonly IActorInbox Inbox;
        public readonly ProcessFlags Flags;

        public AddToStoreMessage(IProcess process, IActorInbox inbox, ProcessFlags flags)
        {
            Process = process;
            Inbox = inbox;
            Flags = flags;
        }

        public override string ToString() =>
            "AddToStore pid: " + Process.Id;
    }

    internal class RemoveFromStoreMessage : ActorSystemMessage
    {
        public override ActorSystemMessageTag Tag => ActorSystemMessageTag.RemoveFromStore;

        public readonly ProcessId ProcessId;

        [JsonConstructor]
        public RemoveFromStoreMessage(ProcessId processId)
        {
            ProcessId = processId;
        }

        public override string ToString() =>
            "RemoveFromStore pid: " + ProcessId;
    }

    internal class RegisterMessage : ActorSystemMessage
    {
        public override ActorSystemMessageTag Tag => ActorSystemMessageTag.Register;

        public readonly ProcessName Name;
        public readonly ProcessId ProcessId;

        [JsonConstructor]
        public RegisterMessage(ProcessName name, ProcessId processId)
        {
            Name = name;
            ProcessId = processId;
        }

        public override string ToString() =>
            "Register pid: " + ProcessId + " as "+Name;
    }

    internal class DeregisterMessage : ActorSystemMessage
    {
        public override ActorSystemMessageTag Tag => ActorSystemMessageTag.Deregister;

        public readonly ProcessName Name;

        [JsonConstructor]
        public DeregisterMessage(ProcessName name)
        {
            Name = name;
        }

        public override string ToString() =>
            "Deregister pid: " + Name;
    }

    internal class ShutdownAllMessage : ActorSystemMessage
    {
        public override ActorSystemMessageTag Tag => ActorSystemMessageTag.ShutdownAll;

        [JsonConstructor]
        public ShutdownAllMessage()
        {
        }

        public override string ToString() =>
            "ShutdownAll";
    }
}