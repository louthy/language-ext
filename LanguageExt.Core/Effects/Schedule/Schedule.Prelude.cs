#nullable enable

using System;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Intersection of two schedules. As long as they are both running it returns the max duration
    /// </summary>
    /// <param name="a">Schedule `a`</param>
    /// <param name="b">Schedule `b`</param>
    /// <returns>Max of schedule `a` and `b` to the length of the shortest schedule</returns>
    [Pure]
    public static Schedule intersect(Schedule a, Schedule b) =>
        a.Intersect(b);

    /// <summary>
    /// Union of two schedules. As long as any are running it returns the min duration of both or a or b
    /// </summary>
    /// <param name="a">Schedule `a`</param>
    /// <param name="b">Schedule `b`</param>
    /// <returns>Min of schedule `a` and `b` or `a` or `b` to the length of the longest schedule</returns>
    [Pure]
    public static Schedule union(Schedule a, Schedule b) =>
        a.Union(b);

    /// <summary>
    /// Interleave two schedules together
    /// </summary>
    /// <param name="a">Schedule `a`</param>
    /// <param name="b">Schedule `b`</param>
    /// <returns>Returns the two schedules interleaved together</returns>
    [Pure]
    public static Schedule interleave(Schedule a, Schedule b) =>
        a.Interleave(b);

    /// <summary>
    /// Append two schedules together
    /// </summary>
    /// <param name="a">Schedule `a`</param>
    /// <param name="b">Schedule `b`</param>
    /// <returns>Returns the two schedules appended</returns>
    [Pure]
    public static Schedule append(Schedule a, Schedule b) =>
        a.Append(b);

    /// <summary>
    /// Take `amount` durations from the `Schedule`
    /// </summary>
    /// <param name="s">Schedule to take from</param>
    /// <param name="amount">Amount ot take</param>
    /// <returns>Schedule with `amount` or less durations</returns>
    [Pure]
    public static Schedule take(Schedule s, int amount) =>
        s.Take(amount);

    /// <summary>
    /// Skip `amount` durations from the `Schedule`
    /// </summary>
    /// <param name="s">Schedule to skip durations from</param>
    /// <param name="amount">Amount ot skip</param>
    /// <returns>Schedule with `amount` durations skipped</returns>
    [Pure]
    public static Schedule skip(Schedule s, int amount) =>
        s.Skip(amount);

    /// <summary>
    /// Take all but the first duration from the schedule
    /// </summary>
    [Pure]
    public static Schedule tail(Schedule s) =>
        s.Tail;

    /// <summary>
    /// Functor map operation for Schedule
    /// </summary>
    /// <param name="s">Schedule</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped schedule</returns>
    [Pure]
    public static Schedule map(Schedule s, Func<Duration, Duration> f) =>
        s.Map(f);
    
    /// <summary>
    /// Filter operation for Schedule
    /// </summary>
    /// <param name="s">Schedule</param>
    /// <param name="pred">predicate</param>
    /// <returns>Filtered schedule</returns>
    [Pure]
    public static Schedule filter(Schedule s, Func<Duration, bool> pred) =>
        s.Filter(pred);

    /// <summary>
    /// Monad bind operation for Schedule
    /// </summary>
    /// <param name="s">Schedule</param>
    /// <param name="f">Bind function</param>
    /// <returns>Chained schedule</returns>
    [Pure]
    public static Schedule bind(Schedule s, Func<Duration, Schedule> f) =>
        s.Bind(f);
}
