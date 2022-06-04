using System.Collections;
using System.Collections.Generic;

namespace LanguageExt;

/// <summary>
/// Provides a mechanism for composing scheduled events
/// </summary>
/// <remarks>
/// Used heavily by `repeat`, `retry`, and `fold` with the `Aff` and `Eff` types.  Use the static methods to create parts
/// of schedulers and then union them using `|` or intersect them using `&`.  Union will take the minimum of the two
/// schedulers, intersect will take the maximum. 
/// </remarks>
/// <example>
/// This example creates a schedule that repeats 5 times, with an exponential delay between each stage, starting
/// at 10 milliseconds:
/// 
///     var s = Schedule.Recurs(5) | Schedule.Exponential(10)
/// 
/// </example>
/// <example>
/// This example creates a schedule that repeats 5 times, with an exponential delay between each stage, starting
/// at 10 milliseconds and with a maximum delay of 2000 milliseconds:
/// 
///     var s = Schedule.Recurs(5) | Schedule.Exponential(10) | Schedule.Spaced(2000)
/// </example>
public readonly partial struct Schedule : IEnumerable<PositiveDuration>
{
    private readonly IEnumerable<PositiveDuration> _enumerable;
    internal Schedule(IEnumerable<PositiveDuration> enumerable) => _enumerable = enumerable;
    public IEnumerable<PositiveDuration> AsEnumerable() => _enumerable;

    public static Schedule operator |(Schedule a, Schedule b) => a.Union(b);
    public static Schedule operator |(Schedule a, ScheduleTransformer b) => b(a);
    public static Schedule operator |(ScheduleTransformer a, Schedule b) => a(b);

    public static Schedule operator &(Schedule a, Schedule b) => a.Intersect(b);
    public static Schedule operator &(Schedule a, ScheduleTransformer b) => b(a);
    public static Schedule operator &(ScheduleTransformer a, Schedule b) => a(b);

    public static Schedule operator +(Schedule a, Schedule b) => a.AsEnumerable().Append(b.AsEnumerable()).ToSchedule();

    public IEnumerator<PositiveDuration> GetEnumerator() => _enumerable.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// Transforms a schedule into another schedule.
/// </summary>
public delegate Schedule ScheduleTransformer(Schedule schedule);
