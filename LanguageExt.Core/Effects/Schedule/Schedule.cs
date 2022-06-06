#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
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
///
/// Any `IEnumerable<Duration>` can be converted to a `Schedule` using `ToSchedule()` or `Schedule.FromDurations(...)`.
/// `Schedule` also implements `IEnumerable<Duration>` so can make use of any transformation on `IEnumerable`.
/// A `Schedule` is a struct so an `AsEnumerable()` method is also provided to avoid boxing.
/// </remarks>
/// <example>
/// This example creates a schedule that repeats 5 times, with an exponential delay between each stage, starting
/// at 10 milliseconds:
/// 
///     var s = Schedule.Recurs(5) | Schedule.Exponential(10*ms)
/// 
/// </example>
/// <example>
/// This example creates a schedule that repeats 5 times, with an exponential delay between each stage, starting
/// at 10 milliseconds and with a maximum delay of 2000 milliseconds:
/// 
///     var s = Schedule.Recurs(5) | Schedule.Exponential(10*ms) | Schedule.Spaced(2000*ms)
/// </example>
/// <example>
/// This example creates a schedule that repeats 5 times, with an exponential delay between each stage, starting
/// at 10 milliseconds and with a minimum delay of 300 milliseconds:
/// 
///     var s = Schedule.Recurs(5) | Schedule.Exponential(10*ms) & Schedule.Spaced(300*ms)
/// </example>
public readonly partial struct Schedule : IEnumerable<Duration>
{
    readonly IEnumerable<Duration> Durations;

    Schedule(IEnumerable<Duration> durations) =>
        Durations = durations;

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
    /// Access the underlying time-series of durations
    /// </summary>
    /// <returns>The underlying time-series of durations</returns>
    [Pure]
    public IEnumerable<Duration> AsEnumerable() =>
        Durations;

    [Pure]
    public IEnumerator<Duration> GetEnumerator() => 
        Durations.GetEnumerator();

    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => 
        Durations.GetEnumerator();
    
    /// <summary>
    /// Intersection of two schedules. As long as they are both running it returns the max duration.
    /// </summary>
    /// <param name="b">Schedule `b`</param>
    /// <returns>Max of schedule `this` and `b` to the length of the shortest schedule</returns>
    [Pure]
    public Schedule Intersect(Schedule b) =>
        AsEnumerable()
            .Zip(b.AsEnumerable())
            .Select(static t => (Duration)Math.Max(t.Item1, t.Item2))
            .ToSchedule();

    /// <summary>
    /// Union of two schedules. As long as any are running it returns the min duration of both or a or b. 
    /// </summary>
    /// <param name="b">Schedule `b`</param>
    /// <returns>Min of schedule `this` and `b` or `this` or `b` to the length of the longest schedule</returns>
    [Pure]
    public Schedule Union(Schedule b)
    {
        return new(Loop(Durations, b.Durations));
        
        IEnumerable<Duration> Loop(IEnumerable<Duration> sa, IEnumerable<Duration> sb)
        {
            using var aEnumerator = sa.GetEnumerator();
            using var bEnumerator = sb.GetEnumerator();

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

                hasA = hasA && aEnumerator.MoveNext();
                hasB = hasB && bEnumerator.MoveNext();
            }
        }
    }

    /// <summary>
    /// Interleave two schedules together
    /// </summary>
    /// <param name="b">Schedule `b`</param>
    /// <returns>Returns the two schedules interleaved together</returns>
    [Pure]
    public Schedule Interleave(Schedule b) =>
        AsEnumerable()
            .Zip(b.AsEnumerable(), static (d1, d2) => new[] {d1, d2})
            .SelectMany(x => x)
            .ToSchedule();

    /// <summary>
    /// Append two schedules together
    /// </summary>
    /// <param name="b">Schedule `b`</param>
    /// <returns>Returns the two schedules appended</returns>
    [Pure]
    public Schedule Append(Schedule b) =>
        new(Durations.Append(b.Durations));

    /// <summary>
    /// Take `amount` durations from the `Schedule`
    /// </summary>
    /// <param name="s">Schedule to take from</param>
    /// <param name="amount">Amount ot take</param>
    /// <returns>Schedule with `amount` or less durations</returns>
    [Pure]
    public Schedule Take(int amount) =>
        new(Durations.Take(amount));

    /// <summary>
    /// Skip `amount` durations from the `Schedule`
    /// </summary>
    /// <param name="s">Schedule to skip durations from</param>
    /// <param name="amount">Amount ot skip</param>
    /// <returns>Schedule with `amount` durations skipped</returns>
    [Pure]
    public Schedule Skip(int amount) =>
        new(Durations.Skip(amount));

    /// <summary>
    /// Take the first duration from the schedule
    /// </summary>
    [Pure]
    public Option<Duration> Head => Durations.HeadOrNone();

    /// <summary>
    /// Take all but the first duration from the schedule
    /// </summary>
    [Pure]
    public Schedule Tail => new (Durations.Tail());
    
    /// <summary>
    /// Prepend a duration in-front of the rest of the scheduled durations
    /// </summary>
    /// <param name="value">Duration to prepend</param>
    /// <returns>Schedule with the duration prepended</returns>
    [Pure] 
    public Schedule Prepend(Duration value) =>
        new (Durations.Prepend(value));
    
    /// <summary>
    /// Functor map operation for Schedule
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped schedule</returns>
    [Pure]
    public Schedule Map(Func<Duration, Duration> f) =>
        new(Durations.Select(f));
    
    /// <summary>
    /// Functor map operation for Schedule
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped schedule</returns>
    [Pure]
    public Schedule Map(Func<Duration, int, Duration> f) =>
        new(Durations.Select(f));

    /// <summary>
    /// Functor map operation for Schedule
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped schedule</returns>
    [Pure]
    public Schedule Select(Func<Duration, Duration> f) =>
        new(Durations.Select(f));

    /// <summary>
    /// Functor map operation for Schedule
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped schedule</returns>
    [Pure]
    public Schedule Select(Func<Duration, int, Duration> f) =>
        new(Durations.Select(f));

    /// <summary>
    /// Monad bind operation for Schedule
    /// </summary>
    /// <param name="f">Bind function</param>
    /// <returns>Chained schedule</returns>
    [Pure]
    public Schedule Bind(Func<Duration, Schedule> f) =>
        new(Durations.SelectMany(d => f(d).AsEnumerable()));

    /// <summary>
    /// Monad bind operation for Schedule
    /// </summary>
    /// <param name="f">Bind function</param>
    /// <returns>Chained schedule</returns>
    [Pure]
    public Schedule SelectMany(Func<Duration, Schedule> f) =>
        new(Durations.SelectMany(d => f(d).AsEnumerable()));

    /// <summary>
    /// Monad bind and project operation for Schedule
    /// </summary>
    /// <param name="s">Schedule</param>
    /// <param name="bind">Bind function</param>
    /// <param name="project">Project function</param>
    /// <returns>Chained schedule</returns>
    [Pure]
    public Schedule SelectMany(Func<Duration, Schedule> bind, Func<Duration, Duration, Duration> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    [Pure]
    internal Eff<S> Run<A, S>(Eff<A> effect, S state, Func<S, A, S> fold, Func<Fin<A>, bool> pred)
    {
        var durations = AsEnumerable();
        return EffMaybe(() =>
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
            using var enumerator = durations.GetEnumerator();
            while (enumerator.MoveNext() && Continue())
            {
                if (enumerator.Current != Duration.Zero) wait.WaitOne((int)enumerator.Current);
                RunAndFold();
            }

            return FinalResult();
        });
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Eff<A> Run<A>(Eff<A> effect, Func<Fin<A>, bool> pred) =>
        Run(effect, default(A), static (_, __) => _, pred)!;

    [Pure]
    internal Eff<RT, S> Run<RT, A, S>(Eff<RT, A> effect, S state, Func<S, A, S> fold, Func<Fin<A>, bool> pred)
        where RT : struct
    {
        var durations = AsEnumerable();
        return EffMaybe<RT, S>(env =>
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
            using var enumerator = durations.GetEnumerator();
            while (enumerator.MoveNext() && Continue())
            {
                if (enumerator.Current != Duration.Zero) wait.WaitOne((int)enumerator.Current);
                RunAndFold();
            }

            return FinalResult();
        });
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Eff<RT, A> Run<RT, A>(Eff<RT, A> effect, Func<Fin<A>, bool> pred) 
        where RT : struct =>
            Run(effect, default(A), static (_, __) => _, pred)!;

    [Pure]
    internal Aff<S> Run<A, S>(Aff<A> effect, S state, Func<S, A, S> fold, Func<Fin<A>, bool> pred)
    {
        var durations = AsEnumerable();
        return AffMaybe(async () =>
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

            using var enumerator = durations.GetEnumerator();
            while (enumerator.MoveNext() && Continue())
            {
                if (enumerator.Current != Duration.Zero)
                    await Task.Delay((int)enumerator.Current).ConfigureAwait(false);
                result = await RunAndFold().ConfigureAwait(false);
            }

            return FinalResult();
        });
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Aff<A> Run<A>(Aff<A> effect, Func<Fin<A>, bool> pred) =>
        Run(effect, default(A), static (_, __) => _, pred)!;

    [Pure]
    internal Aff<RT, S> Run<RT, A, S>(Aff<RT, A> effect, S state, Func<S, A, S> fold, Func<Fin<A>, bool> pred)
        where RT : struct, HasCancel<RT>
    {
        var durations = AsEnumerable();
        return AffMaybe<RT, S>(async env =>
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

            using var enumerator = durations.GetEnumerator();
            while (enumerator.MoveNext() && Continue())
            {
                if (enumerator.Current != Duration.Zero)
                    await Task.Delay((int)enumerator.Current).ConfigureAwait(false);
                result = await RunAndFold().ConfigureAwait(false);
            }

            return FinalResult();
        });
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Aff<RT, A> Run<RT, A>(Aff<RT, A> effect, Func<Fin<A>, bool> pred) 
        where RT : struct, HasCancel<RT> =>
            Run(effect, default(A), static (_, __) => _, pred)!;
}
