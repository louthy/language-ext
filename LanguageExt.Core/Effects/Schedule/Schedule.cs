#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// A schedule is defined as a potentially infinite stream of durations, combined with mechanisms for composing them.
/// </summary>
/// <remarks>
/// Used heavily by `repeat`, `retry`, and `fold` with the `Aff` and `Eff` types.  Use the static methods to create parts
/// of schedulers and then union them using `|` or intersect them using `&`.  Union will take the minimum of the two
/// schedules to the length of the longest, intersect will take the maximum of the two schedules to the length of the shortest.
/// </remarks>
/// <example>
/// This example creates a schedule that repeats 5 times, with an exponential delay between each stage, starting
/// at 10 milliseconds:
/// 
///     var s = Schedule.recurs(5) | Schedule.exponential(10*ms)
/// 
/// </example>
/// <example>
/// This example creates a schedule that repeats 5 times, with an exponential delay between each stage, starting
/// at 10 milliseconds and with a maximum delay of 2000 milliseconds:
/// 
///     var s = Schedule.recurs(5) | Schedule.exponential(10*ms) | Schedule.spaced(2000*ms)
/// </example>
/// <example>
/// This example creates a schedule that repeats 5 times, with an exponential delay between each stage, starting
/// at 10 milliseconds and with a minimum delay of 300 milliseconds:
/// 
///     var s = Schedule.recurs(5) | Schedule.exponential(10*ms) & Schedule.spaced(300*ms)
/// </example>
public abstract partial record Schedule
{
    [Pure]
    public static Schedule operator |(Schedule a, Schedule b) =>
        a.Union(b);

    [Pure]
    public static Schedule operator |(Schedule a, ScheduleTransformer b) =>
        b.Apply(a);

    [Pure]
    public static Schedule operator |(ScheduleTransformer a, Schedule b) =>
        a.Apply(b);

    [Pure]
    public static Schedule operator &(Schedule a, Schedule b) =>
        a.Intersect(b);

    [Pure]
    public static Schedule operator &(Schedule a, ScheduleTransformer b) =>
        b.Apply(a);

    [Pure]
    public static Schedule operator &(ScheduleTransformer a, Schedule b) =>
        a.Apply(b);

    [Pure]
    public static Schedule operator +(Schedule a, Schedule b) =>
        a.Append(b);

    /// <summary>
    /// Realise the underlying time-series of durations
    /// </summary>
    /// <returns>The underlying time-series of durations</returns>
    [Pure]
    public abstract IEnumerable<Duration> Run();

    /// <summary>
    /// Intersection of two schedules. As long as they are both running it returns the max duration
    /// </summary>
    /// <param name="b">Schedule `b`</param>
    /// <returns>Max of schedule `this` and `b` to the length of the shortest schedule</returns>
    [Pure]
    public Schedule Intersect(Schedule b) =>
        new SchIntersect(this, b);

    /// <summary>
    /// Union of two schedules. As long as any are running it returns the min duration of both or a or b
    /// </summary>
    /// <param name="b">Schedule `b`</param>
    /// <returns>Min of schedule `this` and `b` or `this` or `b` to the length of the longest schedule</returns>
    [Pure]
    public Schedule Union(Schedule b) =>
        new SchUnion(this, b);

    /// <summary>
    /// Interleave two schedules together
    /// </summary>
    /// <param name="b">Schedule `b`</param>
    /// <returns>Returns the two schedules interleaved together</returns>
    [Pure]
    public Schedule Interleave(Schedule b) =>
        new SchInterleave(this, b);

    /// <summary>
    /// Append two schedules together
    /// </summary>
    /// <param name="b">Schedule `b`</param>
    /// <returns>Returns the two schedules appended</returns>
    [Pure]
    public Schedule Append(Schedule b) =>
        new SchAppend(this, b);

    /// <summary>
    /// Take `amount` durations from the `Schedule`
    /// </summary>
    /// <param name="s">Schedule to take from</param>
    /// <param name="amount">Amount ot take</param>
    /// <returns>Schedule with `amount` or less durations</returns>
    [Pure]
    public Schedule Take(int amount) =>
        new SchTake(this, amount);

    /// <summary>
    /// Skip `amount` durations from the `Schedule`
    /// </summary>
    /// <param name="s">Schedule to skip durations from</param>
    /// <param name="amount">Amount ot skip</param>
    /// <returns>Schedule with `amount` durations skipped</returns>
    [Pure]
    public Schedule Skip(int amount) =>
        new SchSkip(this, amount);

    /// <summary>
    /// Take all but the first duration from the schedule
    /// </summary>
    [Pure]
    public Schedule Tail =>
        new SchTail(this);

    /// <summary>
    /// Prepend a duration in-front of the rest of the scheduled durations
    /// </summary>
    /// <param name="value">Duration to prepend</param>
    /// <returns>Schedule with the duration prepended</returns>
    [Pure]
    public Schedule Prepend(Duration value) =>
        new SchCons(value, this);

    /// <summary>
    /// Functor map operation for Schedule
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped schedule</returns>
    [Pure]
    public Schedule Map(Func<Duration, Duration> f) =>
        new SchMap(this, f);

    /// <summary>
    /// Functor map operation for Schedule
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped schedule</returns>
    [Pure]
    public Schedule Map(Func<Duration, int, Duration> f) =>
        new SchMapIndex(this, f);

    /// <summary>
    /// Filter operation for Schedule
    /// </summary>
    /// <param name="pred">predicate</param>
    /// <returns>Filtered schedule</returns>
    [Pure]
    public Schedule Filter(Func<Duration, bool> pred) =>
        new SchFilter(this, pred);

    /// <summary>
    /// Filter operation for Schedule
    /// </summary>
    /// <param name="pred">predicate</param>
    /// <returns>Filtered schedule</returns>
    [Pure]
    public Schedule Where(Func<Duration, bool> pred) =>
        new SchFilter(this, pred);

    /// <summary>
    /// Functor map operation for Schedule
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped schedule</returns>
    [Pure]
    public Schedule Select(Func<Duration, Duration> f) =>
        new SchMap(this, f);

    /// <summary>
    /// Functor map operation for Schedule
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped schedule</returns>
    [Pure]
    public Schedule Select(Func<Duration, int, Duration> f) =>
        new SchMapIndex(this, f);

    /// <summary>
    /// Monad bind operation for Schedule
    /// </summary>
    /// <param name="f">Bind function</param>
    /// <returns>Chained schedule</returns>
    [Pure]
    public Schedule Bind(Func<Duration, Schedule> f) =>
        new SchBind(this, f);

    /// <summary>
    /// Monad bind operation for Schedule
    /// </summary>
    /// <param name="f">Bind function</param>
    /// <returns>Chained schedule</returns>
    [Pure]
    public Schedule SelectMany(Func<Duration, Schedule> f) =>
        new SchBind(this, f);

    /// <summary>
    /// Monad bind and project operation for Schedule
    /// </summary>
    /// <param name="s">Schedule</param>
    /// <param name="bind">Bind function</param>
    /// <param name="project">Project function</param>
    /// <returns>Chained schedule</returns>
    [Pure]
    public Schedule SelectMany(Func<Duration, Schedule> bind, Func<Duration, Duration, Duration> project) =>
        new SchBind2(this, bind, project);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Fin<S> FinalResult<A, S>(Fin<A> effectResult, S state) =>
        effectResult.IsSucc ? state : FinFail<S>(effectResult.Error);

    [Pure]
    internal Eff<S> Run<A, S>(Eff<A> effect, S state, Func<S, A, S> fold, Func<Fin<A>, bool> pred)
    {
        var durations = Run();
        return EffMaybe(() =>
        {
            static (Fin<A> EffectResult, S State) RunAndFold(Eff<A> effect, S state, Func<S, A, S> fold)
            {
                var newResult = effect.Run();
                var newState = newResult.IsSucc ? fold(state, newResult.value) : state;
                return (newResult, newState);
            }

            var results = RunAndFold(effect, state, fold);

            if (!pred(results.EffectResult))
                return FinalResult(results.EffectResult, results.State);

            var wait = new AutoResetEvent(false);
            using var enumerator = durations.GetEnumerator();
            while (enumerator.MoveNext() && pred(results.EffectResult))
            {
                if (enumerator.Current != Duration.Zero) wait.WaitOne((int)enumerator.Current);
                results = RunAndFold(effect, results.State, fold);
            }

            return FinalResult(results.EffectResult, results.State);
        });
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Eff<A> Run<A>(Eff<A> effect, Func<Fin<A>, bool> pred) =>
        Run(effect, default(A), static (_, result) => result, pred)!;

    [Pure]
    internal Eff<RT, S> Run<RT, A, S>(Eff<RT, A> effect, S state, Func<S, A, S> fold, Func<Fin<A>, bool> pred)
        where RT : struct
    {
        var durations = Run();
        return EffMaybe<RT, S>(env =>
        {
            static (Fin<A> EffectResult, S State) RunAndFold(RT env, Eff<RT, A> effect, S state, Func<S, A, S> fold)
            {
                var newResult = effect.Run(env);
                var newState = newResult.IsSucc ? fold(state, newResult.value) : state;
                return (newResult, newState);
            }

            var results = RunAndFold(env, effect, state, fold);

            if (!pred(results.EffectResult))
                return FinalResult(results.EffectResult, results.State);

            var wait = new AutoResetEvent(false);
            using var enumerator = durations.GetEnumerator();
            while (enumerator.MoveNext() && pred(results.EffectResult))
            {
                if (enumerator.Current != Duration.Zero) wait.WaitOne((int)enumerator.Current);
                results = RunAndFold(env, effect, results.State, fold);
            }

            return FinalResult(results.EffectResult, results.State);
        });
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Eff<RT, A> Run<RT, A>(Eff<RT, A> effect, Func<Fin<A>, bool> pred) where RT : struct =>
        Run(effect, default(A), static (_, result) => result, pred)!;

    [Pure]
    internal Aff<S> Run<A, S>(Aff<A> effect, S state, Func<S, A, S> fold, Func<Fin<A>, bool> pred)
    {
        var durations = Run();
        return AffMaybe(async () =>
        {
            static async ValueTask<(Fin<A> EffectResult, S State)> RunAndFold(
                Aff<A> effect,
                S state,
                Func<S, A, S> fold)
            {
                var newResult = await effect.Run().ConfigureAwait(false);
                var newState = newResult.IsSucc ? fold(state, newResult.value) : state;
                return (newResult, newState);
            }

            var results = await RunAndFold(effect, state, fold).ConfigureAwait(false);

            if (!pred(results.EffectResult))
                return FinalResult(results.EffectResult, results.State);

            using var enumerator = durations.GetEnumerator();
            while (enumerator.MoveNext() && pred(results.EffectResult))
            {
                if (enumerator.Current != Duration.Zero)
                    await Task.Delay((int)enumerator.Current).ConfigureAwait(false);
                results = await RunAndFold(effect, results.State, fold).ConfigureAwait(false);
            }

            return FinalResult(results.EffectResult, results.State);
        });
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Aff<A> Run<A>(Aff<A> effect, Func<Fin<A>, bool> pred) =>
        Run(effect, default(A), static (_, result) => result, pred)!;

    [Pure]
    internal Aff<RT, S> Run<RT, A, S>(Aff<RT, A> effect, S state, Func<S, A, S> fold, Func<Fin<A>, bool> pred)
        where RT : struct, HasCancel<RT>
    {
        var durations = Run();
        return AffMaybe<RT, S>(async env =>
        {
            static async ValueTask<(Fin<A> EffectResult, S State)> RunAndFold(
                RT env,
                Aff<RT, A> effect,
                S state,
                Func<S, A, S> fold)
            {
                var newResult = await effect.Run(env).ConfigureAwait(false);
                var newState = newResult.IsSucc ? fold(state, newResult.value) : state;
                return (newResult, newState);
            }

            bool Continue(Fin<A> effectResult) =>
                pred(effectResult) && !env.CancellationToken.IsCancellationRequested;

            var results = await RunAndFold(env, effect, state, fold).ConfigureAwait(false);

            if (!Continue(results.EffectResult))
                return FinalResult(results.EffectResult, results.State);

            using var enumerator = durations.GetEnumerator();
            while (enumerator.MoveNext() && Continue(results.EffectResult))
            {
                if (enumerator.Current != Duration.Zero)
                    await Task.Delay((int)enumerator.Current).ConfigureAwait(false);
                results = await RunAndFold(env, effect, results.State, fold).ConfigureAwait(false);
            }

            return FinalResult(results.EffectResult, results.State);
        });
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Aff<RT, A> Run<RT, A>(Aff<RT, A> effect, Func<Fin<A>, bool> pred) where RT : struct, HasCancel<RT> =>
        Run(effect, default(A), static (_, result) => result, pred)!;
}
