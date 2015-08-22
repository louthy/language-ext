using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt
{
    public abstract class ActorSystemMessage : UserControlMessage
    {
        public override Message.Type MessageType => Message.Type.ActorSystem;

        public static ActorSystemMessage ShutdownProcess =
            new ShutdownProcessMessage();

        public readonly static ActorSystemMessage GetChildren =
            new GetChildrenMessage();
    }

    public class GetChildrenMessage : ActorSystemMessage
    {
        public override TagSpec Tag => TagSpec.GetChildren;
        public override string ToString() => "GetChildren";
    }

    public class ShutdownProcessMessage : ActorSystemMessage
    {
        public override TagSpec Tag => TagSpec.ShutdownProcess;

        public override string ToString() =>
            "ShutdownProcess";
    }
}