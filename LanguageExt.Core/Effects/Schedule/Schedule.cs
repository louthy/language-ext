#nullable enable

using System.Collections;
using System.Collections.Generic;

namespace LanguageExt;

/// <summary>
/// A schedule is defined as a potentially infinite stream of durations, combined with mechanisms for composing them.
/// </summary>
/// <remarks>
/// Used heavily by `repeat`, `retry`, and `fold` with the `Aff` and `Eff` types.  Use the static methods to create parts
/// of schedulers and then union them using `|` or intersect them using `&amp;`.  Union will take the minimum of the two
/// schedules to the length of the longest, intersect will take the maximum of the two schedules to the length of the shortest.
///
/// Any `IEnumerable&lt;Duration&gt;` can be converted to a Schedule using `ToSchedule()` or `Schedule.FromDurations(...)`.
/// Schedule also implements `IEnumerable&lt;Duration&gt;` so can make use of any transformation on IEnumerable.
/// A Schedule is a struct so an `AsEnumerable()` method is also provided to avoid boxing.
/// </remarks>
/// <example>
/// This example creates a schedule that repeats 5 times, with an exponential delay between each stage, starting
/// at 10 milliseconds:
/// 
///     var s = Schedule.Recurs(5) | Schedule.Exponential(10 * ms)
/// 
/// </example>
/// <example>
/// This example creates a schedule that repeats 5 times, with an exponential delay between each stage, starting
/// at 10 milliseconds and with a maximum delay of 2000 milliseconds:
/// 
///     var s = Schedule.Recurs(5) | Schedule.Exponential(10 * ms) | Schedule.Spaced(2000 * ms)
/// </example>
/// <example>
/// This example creates a schedule that repeats 5 times, with an exponential delay between each stage, starting
/// at 10 milliseconds and with a minimum delay of 300 milliseconds:
/// 
///     var s = Schedule.Recurs(5) | Schedule.Exponential(10 * ms) &amp; Schedule.Spaced(300 * ms)
/// </example>
public readonly partial struct Schedule : IEnumerable<Duration>
{
    readonly IEnumerable<Duration> Durations;

    internal Schedule(IEnumerable<Duration> durations) =>
        Durations = durations;

    public static Schedule operator |(Schedule a, Schedule b) =>
        a.Union(b);

    public static Schedule operator |(Schedule a, ScheduleTransformer b) =>
        b(a);

    public static Schedule operator |(ScheduleTransformer a, Schedule b) =>
        a(b);

    public static Schedule operator &(Schedule a, Schedule b) =>
        a.Intersect(b);

    public static Schedule operator &(Schedule a, ScheduleTransformer b) =>
        b(a);

    public static Schedule operator &(ScheduleTransformer a, Schedule b) =>
        a(b);

    public static Schedule operator +(Schedule a, Schedule b) =>
        a.AsEnumerable().Append(b.AsEnumerable()).ToSchedule();

    public IEnumerable<Duration> AsEnumerable() =>
        Durations;

    public IEnumerator<Duration> GetEnumerator() =>
        Durations.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();
}

/// <summary>
/// Transforms a schedule into another schedule.
/// </summary>
public delegate Schedule ScheduleTransformer(Schedule schedule);
