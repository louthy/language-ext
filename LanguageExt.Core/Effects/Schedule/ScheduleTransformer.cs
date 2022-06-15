using System;

namespace LanguageExt;

/// <summary>
/// Transforms a schedule into another schedule
/// </summary>
public readonly struct ScheduleTransformer
{
    readonly Func<Schedule, Schedule> Map;

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="map">Mapping functor</param>
    internal ScheduleTransformer(Func<Schedule, Schedule> map) =>
        Map = map;

    /// <summary>
    /// Apply a schedule to the transformer
    /// </summary>
    /// <param name="schedule">`Schedule` to run through the transformer</param>
    /// <returns>`Schedule` that has been run through the transformer</returns>
    public Schedule Apply(Schedule schedule) =>
        Map?.Invoke(schedule) ?? schedule;

    /// <summary>
    /// Compose the two transformers into one
    /// </summary>
    /// <param name="f">First transformer to run in the composition</param>
    /// <param name="g">Second transformer to run in the composition</param>
    /// <returns>composition of the 2 transformers</returns>
    public static ScheduleTransformer operator +(ScheduleTransformer f, ScheduleTransformer g) =>
        new(x => g.Apply(f.Apply(x)));

    public static implicit operator Schedule(ScheduleTransformer t) =>
        Schedule.Forever | t;
}
