#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Effects.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Converts an IEnumerable of positive durations to a schedule. 
    /// </summary>
    /// <param name="enumerable">enumeration of positive durations</param>
    /// <returns>schedule</returns>
    [Pure]
    public static Schedule ToSchedule(this IEnumerable<Duration> enumerable) =>
        new(enumerable);

    /// <summary>
    /// Intersection of 2 schedules. As long as they are both running it returns the max duration.
    /// </summary>
    /// <param name="a">schedule a</param>
    /// <param name="b">schedule b</param>
    /// <returns>max of schedule a and b to the length of the shortest schedule</returns>
    [Pure]
    public static Schedule Intersect(this Schedule a, Schedule b) =>
        a.AsEnumerable().Zip(b.AsEnumerable()).Select(t => (Duration)Math.Max(t.Item1, t.Item2)).ToSchedule();

    /// <summary>
    /// Union of 2 schedules. As long as any are running it returns the min duration of both or a or b. 
    /// </summary>
    /// <param name="a">schedule a</param>
    /// <param name="b">schedule b</param>
    /// <returns>min of schedule a and b or a or b to the length of the longest schedule</returns>
    [Pure]
    public static Schedule Union(this Schedule a, Schedule b)
    {
        IEnumerable<Duration> Loop()
        {
            using var aEnumerator = a.AsEnumerable().GetEnumerator();
            using var bEnumerator = b.AsEnumerable().GetEnumerator();

            var hasA = aEnumerator.MoveNext();
            var hasB = bEnumerator.MoveNext();

            while (hasA || hasB)
            {
                yield return hasA switch
                {
                    true when hasB => Math.Min(aEnumerator.Current, bEnumerator.Current),
                    true => aEnumerator.Current,
                    _ => bEnumerator.Current
                };

                hasA = aEnumerator.MoveNext();
                hasB = bEnumerator.MoveNext();
            }
        }

        return Loop().ToSchedule();
    }

    /// <summary>
    /// Interleave 2 schedules together.
    /// </summary>
    /// <param name="a">schedule a</param>
    /// <param name="b">schedule b</param>
    /// <returns>returns the 2 schedules interleaved together</returns>
    [Pure]
    public static Schedule Interleave(this Schedule a, Schedule b) =>
        a.AsEnumerable().Zip(b.AsEnumerable(), (d1, d2) => new[] { d1, d2 })
            .SelectMany(x => x)
            .ToSchedule();

    [Pure]
    internal static Eff<S> Run<A, S>(this Schedule schedule, Eff<A> effect, S state, Func<S, A, S> fold, Func<Fin<A>, bool> pred) =>
        EffMaybe(() =>
        {
            Fin<A> result;
            void RunAndFold()
            {
                result = effect.Run();
                state = result.IsSucc ? fold(state, result.value) : state;
            }
            bool Continue() => pred(result);
            Fin<S> FinalResult() => result.IsSucc ? state : FinFail<S>(result.Error);

            RunAndFold();
            if (!Continue()) return FinalResult();

            var wait = new AutoResetEvent(false);
            using var enumerator = schedule.GetEnumerator();
            while (enumerator.MoveNext() && Continue())
            {
                if (enumerator.Current != Duration.Zero) wait.WaitOne((int)enumerator.Current);
                RunAndFold();
            }

            return FinalResult();
        });

    [Pure]
    internal static Eff<A> Run<A>(this Schedule schedule, Eff<A> effect, Func<Fin<A>, bool> pred) =>
        schedule.Run(effect, default(A), static (_, __) => _, pred)!;

    [Pure]
    internal static Eff<RT, S> Run<RT, A, S>(this Schedule schedule, Eff<RT, A> effect, S state, Func<S, A, S> fold, Func<Fin<A>, bool> pred) where RT : struct =>
        EffMaybe<RT, S>(env =>
        {
            Fin<A> result;
            void RunAndFold()
            {
                result = effect.Run(env);
                state = result.IsSucc ? fold(state, result.value) : state;
            }
            bool Continue() => pred(result);
            Fin<S> FinalResult() => result.IsSucc ? state : FinFail<S>(result.Error);

            RunAndFold();
            if (!Continue()) return FinalResult();

            var wait = new AutoResetEvent(false);
            using var enumerator = schedule.GetEnumerator();
            while (enumerator.MoveNext() && Continue())
            {
                if (enumerator.Current != Duration.Zero) wait.WaitOne((int)enumerator.Current);
                RunAndFold();
            }

            return FinalResult();
        });

    [Pure]
    internal static Eff<RT, A> Run<RT, A>(this Schedule schedule, Eff<RT, A> effect, Func<Fin<A>, bool> pred) where RT : struct =>
        schedule.Run(effect, default(A), static (_, __) => _, pred)!;

    [Pure]
    internal static Aff<S> Run<A, S>(this Schedule schedule, Aff<A> effect, S state, Func<S, A, S> fold, Func<Fin<A>, bool> pred) =>
        AffMaybe(async () =>
        {
            Fin<A> result;
            async ValueTask<Fin<A>> RunAndFold()
            {
                result = await effect.Run().ConfigureAwait(false);
                state = result.IsSucc ? fold(state, result.value) : state;
                return result;
            }
            bool Continue() => pred(result);
            Fin<S> FinalResult() => result.IsSucc ? state : FinFail<S>(result.Error);

            result = await RunAndFold().ConfigureAwait(false);
            if (!Continue()) return FinalResult();

            using var enumerator = schedule.GetEnumerator();
            while (enumerator.MoveNext() && Continue())
            {
                if (enumerator.Current != Duration.Zero)
                    await Task.Delay((int)enumerator.Current).ConfigureAwait(false);
                result = await RunAndFold().ConfigureAwait(false);
            }

            return FinalResult();
        });

    [Pure]
    internal static Aff<A> Run<A>(this Schedule schedule, Aff<A> effect, Func<Fin<A>, bool> pred) =>
        schedule.Run(effect, default(A), static (_, __) => _, pred)!;

    [Pure]
    internal static Aff<RT, S> Run<RT, A, S>(this Schedule schedule, Aff<RT, A> effect, S state, Func<S, A, S> fold, Func<Fin<A>, bool> pred) where RT : struct, HasCancel<RT> =>
        AffMaybe<RT, S>(async env =>
        {
            Fin<A> result;
            async ValueTask<Fin<A>> RunAndFold()
            {
                result = await effect.Run(env).ConfigureAwait(false);
                state = result.IsSucc ? fold(state, result.value) : state;
                return result;
            }
            bool Continue() => pred(result);
            Fin<S> FinalResult() => result.IsSucc ? state : FinFail<S>(result.Error);

            result = await RunAndFold().ConfigureAwait(false);
            if (!Continue()) return FinalResult();

            using var enumerator = schedule.GetEnumerator();
            while (enumerator.MoveNext() && Continue())
            {
                if (enumerator.Current != Duration.Zero)
                    await Task.Delay((int)enumerator.Current).ConfigureAwait(false);
                result = await RunAndFold().ConfigureAwait(false);
            }

            return FinalResult();
        });

    [Pure]
    internal static Aff<RT, A> Run<RT, A>(this Schedule schedule, Aff<RT, A> effect, Func<Fin<A>, bool> pred) where RT : struct, HasCancel<RT> =>
        schedule.Run(effect, default(A), static (_, __) => _, pred)!;
}
