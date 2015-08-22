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
            UserControl,
            ActorSystem
        }

        public enum TagSpec
        {
            Undefined,  // To catch unitialised data

            // SystemMessageTag
            Restart,
            LinkChild,
            UnLinkChild,
            ChildIsFaulted,
            SystemAsk,

            // UserControlMessageTag
            User,
            UserAsk,
            UserReply,
            Shutdown,

            // ActorSystemMessageTag
            ShutdownProcess,
            GetChildren
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
