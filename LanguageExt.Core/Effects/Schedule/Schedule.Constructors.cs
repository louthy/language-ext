#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt;

using Durations = IEnumerable<Duration>;

public abstract partial record Schedule
{
    /// <summary>
    /// Identity or no-op schedule result transformer
    /// </summary>
    public static readonly ScheduleTransformer Identity =
        Transform(identity);

    /// <summary>
    /// `Schedule` constructor that recurs for the specified durations
    /// </summary>
    /// <param name="durations">durations to apply</param>
    [Pure]
    public static Schedule TimeSeries(params Duration[] durations) =>
        new SchItems(durations);

    /// <summary>
    /// `Schedule` constructor that recurs for the specified durations
    /// </summary>
    /// <param name="durations">durations to apply</param>
    [Pure]
    public static Schedule TimeSeries(Arr<Duration> durations) =>
        new SchItems(durations.Value);

    /// <summary>
    /// `Schedule` constructor that recurs for the specified durations
    /// </summary>
    /// <param name="durations">durations to apply</param>
    [Pure]
    public static Schedule TimeSeries(Seq<Duration> durations) =>
        new SchItems(durations.Value);

    /// <summary>
    /// `Schedule` constructor that recurs for the specified durations
    /// </summary>
    /// <param name="durations">durations to apply</param>
    [Pure]
    public static Schedule TimeSeries(Lst<Duration> durations) =>
        new SchItems(durations.Value);

    /// <summary>
    /// `Schedule` constructor that recurs for the specified durations
    /// </summary>
    /// <param name="durations">durations to apply</param>
    [Pure]
    public static Schedule TimeSeries(Set<Duration> durations) =>
        new SchItems(durations.Value);

    /// <summary>
    /// `Schedule` constructor that recurs for the specified durations
    /// </summary>
    /// <param name="durations">durations to apply</param>
    [Pure]
    public static Schedule TimeSeries(HashSet<Duration> durations) =>
        new SchItems(durations.Value);
    
    /// <summary>
    /// `ScheduleTransformer` constructor which provides mapping capabilities for `Schedule` instances
    /// </summary>
    /// <param name="transform">Transformation function</param>
    /// <returns>`ScheduleTransformer`</returns>
    [Pure]
    public static ScheduleTransformer Transform(Func<Schedule, Schedule> transform) =>
        new(transform);

    /// <summary>
    /// Schedule that runs forever
    /// </summary>
    public static readonly Schedule Forever =
        SchForever.Default;
    
    /// <summary>
    /// Schedule that never runs
    /// </summary>
    public static readonly Schedule Never =
        SchNever.Default;

    /// <summary>
    /// Schedule that runs once
    /// </summary>
    public static readonly Schedule Once =
        Forever.Take(1);

    /// <summary>
    /// A schedule transformer that will enforce the first retry has no delay
    /// </summary>
    public static readonly ScheduleTransformer NoDelayOnFirst =
        Transform(s => s.Tail.Prepend(Duration.Zero));

    /// <summary>
    /// Repeats the schedule forever
    /// </summary>
    public static readonly ScheduleTransformer RepeatForever =
        Transform(s => new SchRepeatForever(s));

    /// <summary>
    /// Schedule transformer that limits the schedule to run the specified number of times
    /// </summary>
    /// <param name="times">number of times</param>
    [Pure]
    public static ScheduleTransformer recurs(int times) =>
        Transform(s => s.Take(times));

    /// <summary>
    /// Schedule that recurs continuously with the given spacing
    /// </summary>
    /// <param name="space">space</param>
    [Pure]
    public static Schedule spaced(Duration space) =>
        Forever.Map(_ => space);

    /// <summary>
    /// Schedule that recurs continuously using a linear backoff
    /// </summary>
    /// <param name="seed">seed</param>
    /// <param name="factor">optional factor to apply, default 1</param>
    [Pure]
    public static Schedule linear(Duration seed, double factor = 1) =>
        new SchLinear(seed, factor);

    /// <summary>
    /// Schedule that recurs continuously using a exponential backoff
    /// </summary>
    /// <param name="seed">seed</param>
    /// <param name="factor">optional factor to apply, default 2</param>
    [Pure]
    public static Schedule exponential(Duration seed, double factor = 2) =>
        Forever.Map((_, i) => seed * Math.Pow(factor, i));

    /// <summary>
    /// Schedule that recurs continuously using a fibonacci based backoff
    /// </summary>
    /// <param name="seed">seed</param>
    [Pure]
    public static Schedule fibonacci(Duration seed) =>
        new SchFibonacci(seed);

    internal static readonly Func<DateTime> LiveNowFn =
        () => DateTime.Now;

    /// <summary>
    /// Schedule that runs for a given duration
    /// </summary>
    /// <param name="max">max duration to run the schedule for</param>
    /// <param name="currentTimeFn">current time function</param>
    [Pure]
    public static Schedule upto(Duration max, Func<DateTime>? currentTimeFn = null) =>
        new SchUpTo(max, currentTimeFn);

    [Pure]
    internal static Duration secondsToIntervalStart(DateTime startTime, DateTime currentTime, Duration interval) =>
        interval - (currentTime - startTime).TotalMilliseconds % interval;

    /// <summary>
    /// Schedule that recurs on a fixed interval.
    ///
    /// If the action run between updates takes longer than the interval, then the
    /// action will be run immediately, but re-runs will not "pile up".
    ///
    /// 
    ///     |-----interval-----|-----interval-----|-----interval-----|
    ///     |---------action--------||action|-----|action|-----------|
    /// 
    /// </summary>
    /// <param name="interval">schedule interval</param>
    /// <param name="currentTimeFn">current time function</param>
    [Pure]
    public static Schedule fixedInterval(Duration interval, Func<DateTime>? currentTimeFn = null) =>
        new SchFixed(interval, currentTimeFn);

    ///<summary>
    /// A schedule that divides the timeline into `interval`-long windows, and sleeps
    /// until the nearest window boundary every time it recurs.
    ///
    /// For example, `Windowed(10 * seconds)` would produce a schedule as follows:
    /// 
    ///          10s        10s        10s       10s
    ///     |----------|----------|----------|----------|
    ///     |action------|sleep---|act|-sleep|action----|
    /// 
    /// </summary>
    /// <param name="interval">schedule interval</param>
    /// <param name="currentTimeFn">current time function</param>
    [Pure]
    public static Schedule windowed(Duration interval, Func<DateTime>? currentTimeFn = null) =>
        new SchWindowed(interval, currentTimeFn);

    [Pure]
    internal static int durationToIntervalStart(int intervalStart, int currentIntervalPosition, int intervalWidth)
    {
        var steps = intervalStart - currentIntervalPosition;
        return steps > 0 ? steps : steps + intervalWidth;
    }

    [Pure]
    internal static int roundBetween(int value, int min, int max) =>
        value > max
            ? max
            : value < min
                ? min
                : value;

    /// <summary>
    /// Cron-like schedule that recurs every specified `second` of each minute
    /// </summary>
    /// <param name="second">second of the minute, will be rounded to fit between 0 and 59</param>
    /// <param name="currentTimeFn">current time function</param>
    [Pure]
    public static Schedule secondOfMinute(int second, Func<DateTime>? currentTimeFn = null) =>
        new SchSecondOfMinute(second, currentTimeFn);

    /// <summary>
    /// Cron-like schedule that recurs every specified `minute` of each hour
    /// </summary>
    /// <param name="minute">minute of the hour, will be rounded to fit between 0 and 59</param>
    /// <param name="currentTimeFn">current time function</param>
    [Pure]
    public static Schedule minuteOfHour(int minute, Func<DateTime>? currentTimeFn = null) =>
        new SchMinuteOfHour(minute, currentTimeFn);

    /// <summary>
    /// Cron-like schedule that recurs every specified `hour` of each day
    /// </summary>
    /// <param name="hour">hour of the day, will be rounded to fit between 0 and 23</param>
    /// <param name="currentTimeFn">current time function</param>
    [Pure]
    public static Schedule hourOfDay(int hour, Func<DateTime>? currentTimeFn = null) =>
        new SchHourOfDay(hour, currentTimeFn);

    /// <summary>
    /// Cron-like schedule that recurs every specified `day` of each week
    /// </summary>
    /// <param name="day">day of the week</param>
    /// <param name="currentTimeFn">current time function</param>
    [Pure]
    public static Schedule dayOfWeek(DayOfWeek day, Func<DateTime>? currentTimeFn = null) =>
        new SchDayOfWeek(day, currentTimeFn);

    /// <summary>
    /// A schedule transformer that limits the returned delays to max delay
    /// </summary>
    /// <param name="max">max delay to return</param>
    [Pure]
    public static ScheduleTransformer maxDelay(Duration max) =>
        Transform(s => new SchMaxDelay(s, max));

    /// <summary>
    /// Limits the schedule to the max cumulative delay
    /// </summary>
    /// <param name="max">max delay to stop schedule at</param>
    [Pure]
    public static ScheduleTransformer maxCumulativeDelay(Duration max) => 
        Transform(s => new SchMaxCumulativeDelay(s, max));

    /// <summary>
    /// A schedule transformer that adds a random jitter to any returned delay
    /// </summary>
    /// <param name="minRandom">min random milliseconds</param>
    /// <param name="maxRandom">max random milliseconds</param>
    /// <param name="seed">optional seed</param>
    [Pure]
    public static ScheduleTransformer jitter(Duration minRandom, Duration maxRandom, Option<int> seed = default) =>
        Transform(s => new SchJitter1(s, minRandom, maxRandom, seed));

    /// <summary>
    /// A schedule transformer that adds a random jitter to any returned delay
    /// </summary>
    /// <param name="factor">jitter factor based on the returned delay</param>
    /// <param name="seed">optional seed</param>
    [Pure]
    public static ScheduleTransformer jitter(double factor = 0.5, Option<int> seed = default) =>
        Transform(s => new SchJitter2(s, factor, seed));

    /// <summary>
    /// Transforms the schedule by de-correlating each of the durations both up and down in a jittered way
    /// </summary>
    /// <remarks>
    /// Given a linear schedule starting at 100. (100, 200, 300...)
    /// Adding de-correlation to it might produce a result like this, (103.2342, 97.123, 202.3213, 197.321...)
    /// The overall schedule runs twice as long but should be less correlated when used in parallel.
    /// </remarks>
    /// <param name="factor">jitter factor based on the returned delay</param>
    /// <param name="seed">optional seed</param>
    [Pure]
    public static ScheduleTransformer decorrelate(double factor = 0.1, Option<int> seed = default) =>
        Transform(s => new SchDecorrelate(s, factor, seed));

    /// <summary>
    /// Resets the schedule after a provided cumulative max duration
    /// </summary>
    /// <param name="max">max delay to reset the schedule at</param>
    [Pure]
    public static ScheduleTransformer resetAfter(Duration max) =>
        Transform(s => new SchResetAfter(s, max));

    /// <summary>
    /// Repeats the schedule n number of times
    /// </summary>
    /// <param name="times">number of times to repeat the schedule</param>
    [Pure]
    public static ScheduleTransformer repeat(int times) =>
        Transform(s => new SchRepeat(s, times));

    /// <summary>
    /// Intersperse the provided duration(s) between each duration in the schedule
    /// </summary>
    /// <param name="duration">schedule to intersperse</param>
    [Pure]
    public static ScheduleTransformer intersperse(Schedule schedule) =>
        Transform(s => s.Bind(schedule.Prepend));

    /// <summary>
    /// Intersperse the provided duration(s) between each duration in the schedule
    /// </summary>
    /// <param name="durations">1 or more durations to intersperse</param>
    [Pure]
    public static ScheduleTransformer intersperse(params Duration[] durations) =>
        intersperse(TimeSeries(durations));
    
    [Obsolete("`Spaced` has been renamed to `spaced`")]
    public static Schedule Spaced(Duration space) => spaced(space);

    [Obsolete("`Recurs` has been renamed to `recurs`")]
    public static ScheduleTransformer Recurs(int times) => recurs(times);

    [Obsolete("`Exponential` has been renamed to `exponential`")]
    public static Schedule Exponential(Duration seed, double factor = 2) =>
        exponential(seed, factor);

    [Obsolete("`Fibonacci` has been renamed to `fibonacci`")]
    public static Schedule Fibonacci(Duration seed) =>
        fibonacci(seed);

}
