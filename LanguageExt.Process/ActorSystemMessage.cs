using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public abstract class ActorSystemMessage : Message
    {
        public override Message.Type MessageType => Message.Type.ActorSystem;

        internal readonly static ActorSystemMessage Startup = 
            new StartupMessage();

        internal static ActorSystemMessage AddToStore(IActor process, IActorInbox inbox, ProcessFlags flags) =>
            new AddToStoreMessage(process, inbox, flags);

        public static ActorSystemMessage Tell(ProcessId pid, object message, ProcessId sender) =>
            new TellMessage(pid, message, sender, Message.TagSpec.Tell);

        public static ActorSystemMessage TellUserControl(ProcessId pid, object message, ProcessId sender) =>
            new TellMessage(pid, message, sender, Message.TagSpec.TellUserControl);

        public static ActorSystemMessage TellSystem(ProcessId pid, object message, ProcessId sender) =>
            new TellMessage(pid, message, sender, Message.TagSpec.TellSystem);

        public static ActorSystemMessage Publish(ProcessId pid, object message) =>
            new PubMessage(pid, message);

        public static ActorSystemMessage ShutdownProcess(ProcessId pid) =>
            new ShutdownProcessMessage(pid);

        public readonly static ActorSystemMessage ShutdownAll =
            new ShutdownAllMessage();

        public static ActorSystemMessage GetChildren(ProcessId pid) =>
            new GetChildrenMessage(pid);

        public static ActorSystemMessage Reply(ProcessId replyTo, object message, long requestId) =>
            new ReplyMessage(replyTo, message, ActorContext.Self, requestId);

        public static ActorSystemMessage ObservePub(ProcessId pid, System.Type type) =>
            new ObservePubMessage(pid, type);

        public static ActorSystemMessage ObserveState(ProcessId pid) =>
            new ObserveStateMessage(pid);
    }

    public class GetChildrenMessage : ActorSystemMessage
    {
        public override TagSpec Tag => TagSpec.GetChildren;

        public readonly ProcessId ProcessId;

        [JsonConstructor]
        public GetChildrenMessage(ProcessId processId)
        {
            ProcessId = processId;
        }

        public override string ToString() => 
            "GetChildren pid: " + ProcessId;
    }

    public class StartupMessage : ActorSystemMessage
    {
        public override TagSpec Tag => TagSpec.Startup;

        public override string ToString() =>
            "Startup";
    }

    public class ShutdownProcessMessage : ActorSystemMessage
    {
        public override TagSpec Tag => TagSpec.ShutdownProcess;

        public readonly ProcessId ProcessId;

        [JsonConstructor]
        public ShutdownProcessMessage(ProcessId processId)
        {
            ProcessId = processId;
        }

        public override string ToString() =>
            "ShutdownProcess pid:" + ProcessId;
    }

    public class ObservePubMessage : ActorSystemMessage
    {
        public override TagSpec Tag => TagSpec.ObservePub;

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

    public class ObserveStateMessage : ActorSystemMessage
    {
        public override TagSpec Tag => TagSpec.ObserveState;

        public readonly ProcessId ProcessId;

        [JsonConstructor]
        public ObserveStateMessage(ProcessId processId)
        {
            ProcessId = processId;
        }

        public override string ToString() =>
            "Observe state pid: " + ProcessId;
    }

    public class ReplyMessage : ActorSystemMessage
    {
        public override TagSpec Tag => TagSpec.Reply;

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

    public class PubMessage : ActorSystemMessage
    {
        public override TagSpec Tag => TagSpec.Publish;

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

    public class TellMessage : ActorSystemMessage
    {
        public override TagSpec Tag => tag;

        public readonly ProcessId ProcessId;
        public readonly object Message;
        public readonly ProcessId Sender;
        readonly Message.TagSpec tag;

        [JsonConstructor]
        public TellMessage(ProcessId processId, object message, ProcessId sender, TagSpec tag)
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
        public override TagSpec Tag  => TagSpec.AddToStore;

        public readonly IActor Process;
        public readonly IActorInbox Inbox;
        public readonly ProcessFlags Flags;

        public AddToStoreMessage(IActor process, IActorInbox inbox, ProcessFlags flags)
        {
            Process = process;
            Inbox = inbox;
            Flags = flags;
        }

        public override string ToString() =>
            "AddToStore pid: " + Process.Id;
    }

    public class ShutdownAllMessage : ActorSystemMessage
    {
        public override TagSpec Tag => TagSpec.ShutdownAll;

        [JsonConstructor]
        public ShutdownAllMessage()
        {
        }

        public override string ToString() =>
            "ShutdownAll";
    }
}