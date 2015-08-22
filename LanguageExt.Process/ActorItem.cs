using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal class ActorItem
    {
        public readonly IActor Actor;
        public readonly IActorInbox Inbox;
        public readonly ProcessFlags Flags;

        public ActorItem(
            IActor actor,
            IActorInbox inbox,
            ProcessFlags flags
            )
        {
            Actor = actor;
            Inbox = inbox;
            Flags = flags;
        }
    }
}
