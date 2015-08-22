using System;
using static LanguageExt.Process;

namespace LanguageExt
{
    internal static class ActorSystem
    {
        public static ActorSystemState Inbox(ActorSystemState state, Unit msg)
        {
            return state;
        }
    }
}
