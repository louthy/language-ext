#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Effects.Traits;

namespace LanguageExt;

public static class ScheduleExtensions
{
    /// <summary>
    /// Converts an `IEnumerable` of positive durations to a schedule. 
    /// </summary>
    /// <param name="enumerable">Enumeration of positive durations</param>
    /// <returns>schedule</returns>
    [Pure]
    public static Schedule ToSchedule(this IEnumerable<Duration> enumerable) =>
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
