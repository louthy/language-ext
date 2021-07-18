using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.ClassInstances.Pred;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    internal static class ScheduleAff<RT, A> where RT : struct, HasCancel<RT>
    {
        static Aff<RT, S> Run<S>(
            Aff<RT, A> ma, 
            S state, 
            Schedule schedule, 
            Func<Fin<A>, (bool Continue, Fin<A> Value)> map, 
            Func<S, A, S> fold) =>
            AffMaybe<RT, S>(
                async env =>
                {
                    var repeats  = schedule.Repeats.Map(static r => r - 1);
                    var spacing0 = schedule.Spacing;
                    var spacing1 = schedule.Spacing;
   
                    while (!env.CancellationToken.IsCancellationRequested)
                    {
                        var ra  = await ma.ReRun(env).ConfigureAwait(false);
                        var (cont, value) = map(ra);
                        state = value.IsSucc ? fold(state, (A)value) : state;
                        
                        if (cont && (repeats.IsNone || (int)repeats > 0))
                        {
                            if (spacing1.IsSome)
                            {
                                await Task.Delay((int)spacing1).ConfigureAwait(false);
                                var spacingX = spacing1;
                                spacing1 = schedule.BackOff((int)spacing0, (int)spacing1);
                                spacing0 = spacingX;
                            }

                            if (repeats.IsSome)
                            {
                                repeats = (int)repeats - 1;
                            }
                        }
                        else
                        {
                            return value.IsSucc
                                       ? state
                                       : FinFail<S>(value.Error);
                        }
                    }
                    return FinFail<S>(Common.Errors.Cancelled);
                });

        public static Aff<RT, A> Repeat(Aff<RT, A> ma, Schedule schedule) =>
            Run(ma, default(A), schedule, static x => (x.IsSucc, x), static (_, x) => x);

        public static Aff<RT, A> Retry(Aff<RT, A> ma, Schedule schedule) =>
            Run(ma, default(A), schedule, static x => (x.IsFail, x), static (_, x) => x);

        public static Aff<RT, A> RepeatWhile(Aff<RT, A> ma, Schedule schedule, Func<A, bool> pred) =>
            Run(ma, default(A), schedule, x => (x.IsSucc && pred((A)x), x), static (_, x) => x);

        public static Aff<RT, A> RetryWhile(Aff<RT, A> ma, Schedule schedule, Func<A, bool> pred) =>
            Run(ma, default(A), schedule, x => (x.IsFail && pred((A)x), x), static (_, x) => x);

        public static Aff<RT, A> RepeatUntil(Aff<RT, A> ma, Schedule schedule, Func<A, bool> pred) =>
            Run(ma, default(A), schedule, x => (x.IsSucc && !pred((A)x), x), static (_, x) => x);

        public static Aff<RT, A> RetryUntil(Aff<RT, A> ma, Schedule schedule, Func<A, bool> pred) =>
            Run(ma, default(A), schedule, x => (x.IsFail && !pred((A)x), x), static (_, x) => x);

        public static Aff<RT, S> Fold<S>(Aff<RT, A> ma, Schedule schedule, S state, Func<S, A, S> fold) =>
            Run(ma, state, schedule, static x => (x.IsSucc, x), fold);

        public static Aff<RT, S> FoldWhile<S>(Aff<RT, A> ma, Schedule schedule, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            Run(ma, state, schedule, x => (x.IsSucc && pred((A)x), x), fold);

        public static Aff<RT, S> FoldUntil<S>(Aff<RT, A> ma, Schedule schedule, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            Run(ma, state, schedule, x => (x.IsSucc && !pred((A)x), x), fold);
    }
    
    internal static class ScheduleAff<A> 
    {
        static Aff<S> Run<S>(Aff<A> ma, S state, Schedule schedule, Func<Fin<A>, (bool Continue, Fin<A> Value)> map, Func<S, A, S> fold) =>
            AffMaybe<S>(
                async () =>
                {
                    var repeats  = schedule.Repeats;
                    var spacing0 = schedule.Spacing;
                    var spacing1 = schedule.Spacing;
   
                    while (true)
                    {
                        var ra = await ma.ReRun().ConfigureAwait(false);
                        var (cont, value) = map(ra);
                        state = value.IsSucc ? fold(state, (A)value) : state;
                        
                        if (cont && (repeats.IsNone || (int)repeats > 0))
                        {
                            if (spacing1.IsSome)
                            {
                                await Task.Delay((int)spacing1).ConfigureAwait(false);
                                var spacingX = spacing1;
                                spacing1 = schedule.BackOff((int)spacing0, (int)spacing1);
                                spacing0 = spacingX;
                            }

                            if (repeats.IsSome)
                            {
                                repeats = (int)repeats - 1;
                            }
                        }
                        else
                        {
                            return value.IsSucc
                                       ? state
                                       : FinFail<S>(value.Error);
                        }
                    }
                });

        public static Aff<A> Repeat(Aff<A> ma, Schedule schedule) =>
            Run(ma, default(A), schedule, static x => (x.IsSucc, x), static (_, x) => x);

        public static Aff<A> Retry(Aff<A> ma, Schedule schedule) =>
            Run(ma, default(A), schedule, static x => (x.IsFail, x), static (_, x) => x);

        public static Aff<A> RepeatWhile(Aff<A> ma, Schedule schedule, Func<A, bool> pred) =>
            Run(ma, default(A), schedule, x => (x.IsSucc && pred((A)x), x), static (_, x) => x);

        public static Aff<A> RetryWhile(Aff<A> ma, Schedule schedule, Func<A, bool> pred) =>
            Run(ma, default(A), schedule, x => (x.IsFail && pred((A)x), x), static (_, x) => x);

        public static Aff<A> RepeatUntil(Aff<A> ma, Schedule schedule, Func<A, bool> pred) =>
            Run(ma, default(A), schedule, x => (x.IsSucc && !pred((A)x), x), static (_, x) => x);

        public static Aff<A> RetryUntil(Aff<A> ma, Schedule schedule, Func<A, bool> pred) =>
            Run(ma, default(A), schedule, x => (x.IsFail && !pred((A)x), x), static (_, x) => x);

        public static Aff<S> Fold<S>(Aff<A> ma, Schedule schedule, S state, Func<S, A, S> fold) =>
            Run(ma, state, schedule, static x => (x.IsSucc, x), fold);

        public static Aff<S> FoldWhile<S>(Aff<A> ma, Schedule schedule, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            Run(ma, state, schedule, x => (x.IsSucc && pred((A)x), x), fold);

        public static Aff<S> FoldUntil<S>(Aff<A> ma, Schedule schedule, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            Run(ma, state, schedule, x => (x.IsSucc && !pred((A)x), x), fold);
    }
}
