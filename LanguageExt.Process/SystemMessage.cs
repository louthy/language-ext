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
    }

    internal class SystemRestartMessage : SystemMessage
    {
        public override TagSpec Tag => TagSpec.Restart;
    }
}