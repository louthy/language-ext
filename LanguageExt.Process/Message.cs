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

            User                        = 1,
            System                      = 2,
            UserControl                 = 3
        }

        public enum TagSpec
        {
            Undefined,  // To catch unitialised data

            // SystemMessageTag
            Restart                     = 1,
            LinkChild                   = 2,
            UnlinkChild                 = 3,
            SystemAsk                   = 4,
            ChildFaulted                = 10,
            Pause                       = 11,
            Unpause                     = 12,

            // UserControlMessageTag
            User                        = 5,
            UserAsk                     = 6,
            UserReply                   = 7,
            Null                        = 13, ///< Does nothing but wake up an inbox

            // ActorSystemMessageTag
            StartupProcess              = 14,
            ShutdownProcess             = 8,
            GetChildren                 = 9
        }

        public abstract Type MessageType
        {
            get;
        }

        public abstract TagSpec Tag
        {
            get;
        }

        public string SessionId;

        public Message()
        {
            SessionId = ActorContext.Context.SessionId.IfNoneUnsafe((string)null);
        }
    }
}
