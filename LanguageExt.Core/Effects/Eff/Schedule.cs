#nullable enable

using System;
using LanguageExt.Common;

namespace LanguageExt
{
    internal static class ScheduleEff<RT, A> where RT : struct
    {
        public static Eff<RT, A> Repeat(Eff<RT, A> ma, Schedule schedule) =>
            schedule.Run(ma, static x => x.IsSucc);

        public static Eff<RT, A> Retry(Eff<RT, A> ma, Schedule schedule) =>
            schedule.Run(ma, static x => x.IsFail);

        public static Eff<RT, A> RepeatWhile(Eff<RT, A> ma, Schedule schedule, Func<A, bool> pred) =>
            schedule.Run(ma, x => x.Case is A a && pred(a));

        public static Eff<RT, A> RetryWhile(Eff<RT, A> ma, Schedule schedule, Func<Error, bool> pred) =>
            schedule.Run(ma, x => x.Case is Error e && pred(e));

        public static Eff<RT, A> RepeatUntil(Eff<RT, A> ma, Schedule schedule, Func<A, bool> pred) =>
            schedule.Run(ma, x => x.Case is A a && !pred(a));

        public static Eff<RT, A> RetryUntil(Eff<RT, A> ma, Schedule schedule, Func<Error, bool> pred) =>
            schedule.Run(ma, x => x.Case is Error e && !pred(e));

        public static Eff<RT, S> Fold<S>(Eff<RT, A> ma, Schedule schedule, S state, Func<S, A, S> fold) =>
            schedule.Run(ma, state, fold, static x => x.IsSucc);

        public static Eff<RT, S> FoldWhile<S>(Eff<RT, A> ma, Schedule schedule, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            schedule.Run(ma, state, fold, x => x.Case is A a && pred(a));

        public static Eff<RT, S> FoldUntil<S>(Eff<RT, A> ma, Schedule schedule, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            schedule.Run(ma, state, fold, x => x.Case is A a && !pred(a));
    }

    internal static class ScheduleEff<A>
    {
        public static Eff<A> Repeat(Eff<A> ma, Schedule schedule) =>
            schedule.Run(ma, static x => x.IsSucc);

        public static Eff<A> Retry(Eff<A> ma, Schedule schedule) =>
            schedule.Run(ma, static x => x.IsFail);

        public static Eff<A> RepeatWhile(Eff<A> ma, Schedule schedule, Func<A, bool> pred) =>
            schedule.Run(ma, x => x.Case is A a && pred(a));

        public static Eff<A> RetryWhile(Eff<A> ma, Schedule schedule, Func<Error, bool> pred) =>
            schedule.Run(ma, x => x.Case is Error e && pred(e));

        public static Eff<A> RepeatUntil(Eff<A> ma, Schedule schedule, Func<A, bool> pred) =>
            schedule.Run(ma, x => x.Case is A a && !pred(a));

        public static Eff<A> RetryUntil(Eff<A> ma, Schedule schedule, Func<Error, bool> pred) =>
            schedule.Run(ma, x => x.Case is Error e && !pred(e));

        public static Eff<S> Fold<S>(Eff<A> ma, Schedule schedule, S state, Func<S, A, S> fold) =>
            schedule.Run(ma, state, fold, static x => x.IsSucc);

        public static Eff<S> FoldWhile<S>(Eff<A> ma, Schedule schedule, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            schedule.Run(ma, state, fold, x => x.Case is A a && pred(a));

        public static Eff<S> FoldUntil<S>(Eff<A> ma, Schedule schedule, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            schedule.Run(ma, state, fold, x => x.Case is A a && !pred(a));
    }
}
