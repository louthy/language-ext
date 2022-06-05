#nullable enable

using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt
{
    internal static class ScheduleAff<RT, A> where RT : struct, HasCancel<RT>
    {
        public static Aff<RT, A> Repeat(Aff<RT, A> ma, Schedule schedule) =>
            schedule.Run(ma, static x => x.IsSucc);

        public static Aff<RT, A> Retry(Aff<RT, A> ma, Schedule schedule) =>
            schedule.Run(ma, static x => x.IsFail);

        public static Aff<RT, A> RepeatWhile(Aff<RT, A> ma, Schedule schedule, Func<A, bool> pred) =>
            schedule.Run(ma, x => x.Case is A a && pred(a));

        public static Aff<RT, A> RetryWhile(Aff<RT, A> ma, Schedule schedule, Func<Error, bool> pred) =>
            schedule.Run(ma, x => x.Case is Error e && pred(e));

        public static Aff<RT, A> RepeatUntil(Aff<RT, A> ma, Schedule schedule, Func<A, bool> pred) =>
            schedule.Run(ma, x => x.Case is A a && !pred(a));

        public static Aff<RT, A> RetryUntil(Aff<RT, A> ma, Schedule schedule, Func<Error, bool> pred) =>
            schedule.Run(ma, x => x.Case is Error e && !pred(e));

        public static Aff<RT, S> Fold<S>(Aff<RT, A> ma, Schedule schedule, S state, Func<S, A, S> fold) =>
            schedule.Run(ma, state, fold, static x => x.IsSucc);

        public static Aff<RT, S> FoldWhile<S>(Aff<RT, A> ma, Schedule schedule, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            schedule.Run(ma, state, fold, x => x.Case is A a && pred(a));

        public static Aff<RT, S> FoldUntil<S>(Aff<RT, A> ma, Schedule schedule, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            schedule.Run(ma, state, fold, x => x.Case is A a && !pred(a));
    }

    internal static class ScheduleAff<A>
    {
        public static Aff< A> Repeat(Aff< A> ma, Schedule schedule) =>
            schedule.Run(ma, static x => x.IsSucc);

        public static Aff< A> Retry(Aff< A> ma, Schedule schedule) =>
            schedule.Run(ma, static x => x.IsFail);

        public static Aff<A> RepeatWhile(Aff<A> ma, Schedule schedule, Func<A, bool> pred) =>
            schedule.Run(ma, x => x.Case is A a && pred(a));

        public static Aff<A> RetryWhile(Aff<A> ma, Schedule schedule, Func<Error, bool> pred) =>
            schedule.Run(ma, x => x.Case is Error e && pred(e));

        public static Aff<A> RepeatUntil(Aff<A> ma, Schedule schedule, Func<A, bool> pred) =>
            schedule.Run(ma, x => x.Case is A a && !pred(a));

        public static Aff<A> RetryUntil(Aff<A> ma, Schedule schedule, Func<Error, bool> pred) =>
            schedule.Run(ma, x => x.Case is Error e && !pred(e));

        public static Aff<S> Fold<S>(Aff<A> ma, Schedule schedule, S state, Func<S, A, S> fold) =>
            schedule.Run(ma, state, fold, static x => x.IsSucc);

        public static Aff<S> FoldWhile<S>(Aff<A> ma, Schedule schedule, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            schedule.Run(ma, state, fold, x => x.Case is A a && pred(a));

        public static Aff<S> FoldUntil<S>(Aff<A> ma, Schedule schedule, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            schedule.Run(ma, state, fold, x => x.Case is A a && !pred(a));
    }
}
