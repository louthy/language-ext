using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    internal static class ReplyActor
    {
        const int responseActors = 20;

        public static Random Inbox(Random rnd, ActorResponse reply)
        {
            var dist = Self.MakeChildId("reply-" + rnd.Next(responseActors));
            tell(dist, reply);
            return rnd;
        }

        public static void ReplyInbox(ActorResponse res)
        {
            res.Subject.OnNext(res.Message);
            res.Subject.OnCompleted();
        }

        public static Random Setup()
        {
            spawnN<ActorResponse>(responseActors, "reply", ReplyInbox);
            return new Random();
        }
    }
}
