using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.ClassInstances.Pred;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    internal static class ScheduleEff<RT, A> 
        where RT : struct
    {
        static Eff<RT, S> Run<S>(Eff<RT, A> ma, S state, Schedule schedule, Func<Fin<A>, (bool Continue, Fin<A> Value)> map, Func<S, A, S> fold) =>
            EffMaybe<RT, S>(
                env =>
                {
                    var repeats  = schedule.Repeats.Map(static r => r - 1);
                    var spacing0 = schedule.Spacing;
                    var spacing1 = schedule.Spacing;
                    var wait     = spacing1.IsSome ? new AutoResetEvent(false) : null;
   
                    while (true)
                    {
                        var ra = ma.ReRun(env);
                        var (cont, value) = map(ra);
                        state = value.IsSucc ? fold(state, (A)value) : state;
                        
                        if (cont && (repeats.IsNone || (int)repeats > 0))
                        {
                            if (spacing1.IsSome)
                            {
                                wait?.WaitOne((int)spacing1);
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

        public static Eff<RT, A> Repeat(Eff<RT, A> ma, Schedule schedule) =>
            Run(ma, default(A), schedule, static x => (x.IsSucc, x), static (_, x) => x);

        public static Eff<RT, A> Retry(Eff<RT, A> ma, Schedule schedule) =>
            Run(ma, default(A), schedule, static x => (x.IsFail, x), static (_, x) => x);

        public static Eff<RT, A> RepeatWhile(Eff<RT, A> ma, Schedule schedule, Func<A, bool> pred) =>
            Run(ma, default(A), schedule, x => (x.IsSucc && pred((A)x), x), static (_, x) => x);

        public static Eff<RT, A> RetryWhile(Eff<RT, A> ma, Schedule schedule, Func<A, bool> pred) =>
            Run(ma, default(A), schedule, x => (x.IsFail && pred((A)x), x), static (_, x) => x);

        public static Eff<RT, A> RepeatUntil(Eff<RT, A> ma, Schedule schedule, Func<A, bool> pred) =>
            Run(ma, default(A), schedule, x => (x.IsSucc && !pred((A)x), x), static (_, x) => x);

        public static Eff<RT, A> RetryUntil(Eff<RT, A> ma, Schedule schedule, Func<A, bool> pred) =>
            Run(ma, default(A), schedule, x => (x.IsFail && !pred((A)x), x), static (_, x) => x);    

        public static Eff<RT, S> Fold<S>(Eff<RT, A> ma, Schedule schedule, S state, Func<S, A, S> fold) =>
            Run(ma, state, schedule, static x => (x.IsSucc, x), fold);

        public static Eff<RT, S> FoldWhile<S>(Eff<RT, A> ma, Schedule schedule, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            Run(ma, state, schedule, x => (x.IsSucc && pred((A)x), x), fold);

        public static Eff<RT, S> FoldUntil<S>(Eff<RT, A> ma, Schedule schedule, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            Run(ma, state, schedule, x => (x.IsSucc && !pred((A)x), x), fold);
    }
        
    internal static class ScheduleEff<A> 
    {
        static Eff<S> Run<S>(Eff<A> ma, S state, Schedule schedule, Func<Fin<A>, (bool Continue, Fin<A> Value)> map, Func<S, A, S> fold) =>
            EffMaybe<S>(
                () =>
                {
                    var repeats  = schedule.Repeats;
                    var spacing0 = schedule.Spacing;
                    var spacing1 = schedule.Spacing;
                    var wait     = spacing1.IsSome ? new AutoResetEvent(false) : null;
   
                    while (true)
                    {
                        var ra = ma.ReRun();
                        var (cont, value) = map(ra);
                        state = value.IsSucc ? fold(state, (A)value) : state;
                        
                        if (cont && (repeats.IsNone || (int)repeats > 0))
                        {
                            if (spacing1.IsSome)
                            {
                                wait?.WaitOne((int)spacing1);
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

        public static Eff<A> Repeat(Eff<A> ma, Schedule schedule) =>
            Run(ma, default(A), schedule, static x => (x.IsSucc, x), static (_, x) => x);

        public static Eff<A> Retry(Eff<A> ma, Schedule schedule) =>
            Run(ma, default(A), schedule, static x => (x.IsFail, x), static (_, x) => x);

        public static Eff<A> RepeatWhile(Eff<A> ma, Schedule schedule, Func<A, bool> pred) =>
            Run(ma, default(A), schedule, x => (x.IsSucc && pred((A)x), x), static (_, x) => x);

        public static Eff<A> RetryWhile(Eff<A> ma, Schedule schedule, Func<A, bool> pred) =>
            Run(ma, default(A), schedule, x => (x.IsFail && pred((A)x), x), static (_, x) => x);

        public static Eff<A> RepeatUntil(Eff<A> ma, Schedule schedule, Func<A, bool> pred) =>
            Run(ma, default(A), schedule, x => (x.IsSucc && !pred((A)x), x), static (_, x) => x);

        public static Eff<A> RetryUntil(Eff<A> ma, Schedule schedule, Func<A, bool> pred) =>
            Run(ma, default(A), schedule, x => (x.IsFail && !pred((A)x), x), static (_, x) => x);

        public static Eff<S> Fold<S>(Eff<A> ma, Schedule schedule, S state, Func<S, A, S> fold) =>
            Run(ma, state, schedule, static x => (x.IsSucc, x), fold);

        public static Eff<S> FoldWhile<S>(Eff<A> ma, Schedule schedule, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            Run(ma, state, schedule, x => (x.IsSucc && pred((A)x), x), fold);

        public static Eff<S> FoldUntil<S>(Eff<A> ma, Schedule schedule, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            Run(ma, state, schedule, x => (x.IsSucc && !pred((A)x), x), fold);        
    }
}
