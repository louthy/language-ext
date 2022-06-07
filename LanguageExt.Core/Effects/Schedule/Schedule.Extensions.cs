#nullable enable

using System.Diagnostics.Contracts;

namespace LanguageExt;

public static class ScheduleExtensions
{
    /// <summary>
    /// Converts an `Seq` of positive durations to a schedule. 
    /// </summary>
    /// <param name="enumerable">Enumeration of positive durations</param>
    /// <returns>schedule</returns>
    [Pure]
    public static Schedule ToSchedule(this Seq<Duration> enumerable) =>
        Schedule.TimeSeries(enumerable);
    
    /// <summary>
    /// Converts an `Arr` of positive durations to a schedule. 
    /// </summary>
    /// <param name="enumerable">Enumeration of positive durations</param>
    /// <returns>schedule</returns>
    [Pure]
    public static Schedule ToSchedule(this Arr<Duration> enumerable) =>
        Schedule.TimeSeries(enumerable);
    
    /// <summary>
    /// Converts an `Lst` of positive durations to a schedule. 
    /// </summary>
    /// <param name="enumerable">Enumeration of positive durations</param>
    /// <returns>schedule</returns>
    [Pure]
    public static Schedule ToSchedule(this Lst<Duration> enumerable) =>
        Schedule.TimeSeries(enumerable);
    
    /// <summary>
    /// Converts an `Set` of positive durations to a schedule. 
    /// </summary>
    /// <param name="enumerable">Enumeration of positive durations</param>
    /// <returns>schedule</returns>
    [Pure]
    public static Schedule ToSchedule(this Set<Duration> enumerable) =>
        Schedule.TimeSeries(enumerable);
    
    /// <summary>
    /// Converts an `HashSet` of positive durations to a schedule. 
    /// </summary>
    /// <param name="enumerable">Enumeration of positive durations</param>
    /// <returns>schedule</returns>
    [Pure]
    public static Schedule ToSchedule(this HashSet<Duration> enumerable) =>
        Schedule.TimeSeries(enumerable);

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
