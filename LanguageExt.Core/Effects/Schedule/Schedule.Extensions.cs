#nullable enable

using System.Diagnostics.Contracts;

namespace LanguageExt;

public static class ScheduleExtensions
{
    /// <summary>
    /// Converts a `Seq` of positive durations to a schedule
    /// </summary>
    /// <param name="seq">Seq of positive durations</param>
    /// <returns>schedule</returns>
    [Pure]
    public static Schedule ToSchedule(this Seq<Duration> seq) =>
        Schedule.TimeSeries(seq);
    
    /// <summary>
    /// Converts a `Arr` of positive durations to a schedule
    /// </summary>
    /// <param name="array">array of positive durations</param>
    /// <returns>schedule</returns>
    [Pure]
    public static Schedule ToSchedule(this Arr<Duration> array) =>
        Schedule.TimeSeries(array);
    
    /// <summary>
    /// Converts a `Lst` of positive durations to a schedule 
    /// </summary>
    /// <param name="list">list of positive durations</param>
    /// <returns>schedule</returns>
    [Pure]
    public static Schedule ToSchedule(this Lst<Duration> list) =>
        Schedule.TimeSeries(list);
    
    /// <summary>
    /// Converts a `Set` of positive durations to a schedule
    /// </summary>
    /// <param name="set">set of positive durations</param>
    /// <returns>schedule</returns>
    [Pure]
    public static Schedule ToSchedule(this Set<Duration> set) =>
        Schedule.TimeSeries(set);
    
    /// <summary>
    /// Converts a `HashSet` of positive durations to a schedule
    /// </summary>
    /// <param name="hashSet">hashset of positive durations</param>
    /// <returns>schedule</returns>
    [Pure]
    public static Schedule ToSchedule(this HashSet<Duration> hashSet) =>
        Schedule.TimeSeries(hashSet);

    /// <summary>
    /// Prepend a duration to the schedule
    /// </summary>
    /// <param name="value">Duration to prepend</param>
    /// <param name="s">Schedule</param>
    /// <returns>Schedule with the duration prepended</returns>
    [Pure]
    public static Schedule Cons(this Duration value, Schedule s) =>
        s.Prepend(value);
}
