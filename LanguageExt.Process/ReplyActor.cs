using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    /// <summary>
    /// Not currently used, but may be in future when distributed asks are done.
    /// </summary>
    internal static class ReplyActor
    {
        const int responseActors = 20;

        public static Random Inbox(Random rnd, ActorResponse reply)
        {
            var dist = Self.Child("reply-" + rnd.Next(responseActors));
            tell(dist, reply);
            return rnd;
        }

        public static void ReplyInbox(ActorResponse res)
        {
        }

        public static Random Setup()
        {
            spawnN<ActorResponse>(responseActors, "reply", ReplyInbox);
            return new Random();
        }
    }
}
