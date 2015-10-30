using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public abstract class Message
    {
        public enum Type
        {
            Undefined,  // To catch unitialised data

            User,
            System,
            UserControl
        }

        public enum TagSpec
        {
            Undefined,  // To catch unitialised data

            // SystemMessageTag
            Restart,
            LinkChild,
            UnlinkChild,
            SystemAsk,

            // UserControlMessageTag
            User,
            UserAsk,
            UserReply,

            // ActorSystemMessageTag
            ShutdownProcess,
            GetChildren,

            // SystemMessageTag
            ChildFaulted
        }

        public abstract Type MessageType
        {
            get;
        }

        public abstract TagSpec Tag
        {
            get;
        }
    }
}
