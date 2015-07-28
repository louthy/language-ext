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

            // UserControlMessageTag
            User,
            UserAsk,
            UserReply,
            Shutdown,

            // ActorSystemMessageTag
            Startup,
            AddToStore,
            RemoveFromStore,
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
