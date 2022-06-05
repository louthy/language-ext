#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using static LanguageExt.Prelude;

namespace LanguageExt;

using Durations = IEnumerable<Duration>;

public readonly partial struct Schedule
{
    /// <summary>
    /// Identity or noop schedule result transformer.
    /// </summary>
    public static readonly ScheduleTransformer Identity =
        _ => _;

    [Pure]
    private static Durations InternalForever()
    {
        while (true) yield return Duration.Zero;
    }

    /// <summary>
    /// Schedule that runs forever.
    /// </summary>
    public static readonly Schedule Forever =
        InternalForever().ToSchedule();

    /// <summary>
    /// Schedule that never runs.
    /// </summary>
    public static readonly Schedule Never =
        Enumerable.Empty<Duration>().ToSchedule();

    /// <summary>
    /// Schedule that runs once.
    /// </summary>
    public static readonly Schedule Once =
        Forever.AsEnumerable().Take(1).ToSchedule();

    /// <summary>
    /// Schedule that recurs for the specified fixed durations.
    /// </summary>
    /// <param name="durations">durations to apply</param>
    [Pure]
    public static Schedule FromDurations(params Duration[] durations) =>
        durations.AsEnumerable().ToSchedule();

    /// <summary>
    /// Schedule that recurs for the specified durations.
    /// </summary>
    /// <param name="durations">durations to apply</param>
    [Pure]
    public static Schedule FromDurations(Durations durations) =>
        durations.ToSchedule();

    /// <summary>
    /// Schedule that recurs the specified number of times.
    /// </summary>
    /// <param name="times">number of times</param>
    [Pure]
    public static ScheduleTransformer Recurs(int times) =>
        s => s.Take(times).ToSchedule();

    /// <summary>
    /// Schedule that recurs continuously with the given spacing.
    /// </summary>
    /// <param name="space">space</param>
    [Pure]
    public static Schedule Spaced(Duration space) =>
        Forever.AsEnumerable().Select(_ => space).ToSchedule();

    /// <summary>
    /// Schedule that recurs continuously using a linear backoff.
    /// </summary>
    /// <param name="seed">seed</param>
    /// <param name="factor">optional factor to apply, default 1</param>
    [Pure]
    public static Schedule Linear(Duration seed, double factor = 1)
    {
        Duration delayToAdd = seed * factor;
        var accumulator = seed;

        Durations Loop()
        {
            yield return accumulator;
            while (true)
            {
                accumulator += delayToAdd;
                yield return accumulator;
            }
        }

        return Loop().ToSchedule();
    }

    /// <summary>
    /// Schedule that recurs continuously using a exponential backoff.
    /// </summary>
    /// <param name="seed">seed</param>
    /// <param name="factor">optional factor to apply, default 2</param>
    [Pure]
    public static Schedule Exponential(Duration seed, double factor = 2) =>
        Forever.AsEnumerable().Select<Duration, Duration>((_, i) => seed * Math.Pow(factor, i)).ToSchedule();

    /// <summary>
    /// Schedule that recurs continuously using a fibonacci based backoff.
    /// </summary>
    /// <param name="seed">seed</param>
    [Pure]
    public static Schedule Fibonacci(Duration seed)
    {
        var last = Duration.Zero;
        var accumulator = seed;

        Durations Loop()
        {
            yield return accumulator;
            while (true)
            {
                var current = accumulator;
                accumulator += last;
                last = current;
                yield return accumulator;
            }
        }

        return Loop().ToSchedule();
    }

    private static readonly Func<DateTime> LiveNowFn =
        () => DateTime.Now;

    /// <summary>
    /// Schedule that runs for a given duration.
    /// </summary>
    /// <param name="max">max duration to run the schedule for</param>
    /// <param name="currentTimeFn">current time function</param>
    [Pure]
    public static Schedule UpTo(Duration max, Func<DateTime>? currentTimeFn = null)
    {
        var now = currentTimeFn ?? LiveNowFn;

        Durations Loop()
        {
            var startTime = now();
            while (now() - startTime < max) yield return Duration.Zero;
        }

        return Loop().ToSchedule();
    }

    [Pure]
    private static Duration SecondsToIntervalStart(DateTime startTime, DateTime currentTime, Duration interval) =>
        interval - (currentTime - startTime).TotalMilliseconds % interval;

    /// <summary>
    /// Schedule that recurs on a fixed interval.
    ///
    /// If the action run between updates takes longer than the interval, then the
    /// action will be run immediately, but re-runs will not "pile up".
    ///
    /// <pre>
    /// |-----interval-----|-----interval-----|-----interval-----|
    /// |---------action--------||action|-----|action|-----------|
    /// </pre>
    /// </summary>
    /// <param name="interval">schedule interval</param>
    /// <param name="currentTimeFn">current time function</param>
    public static Schedule Fixed(Duration interval, Func<DateTime>? currentTimeFn = null)
    {
        var now = currentTimeFn ?? LiveNowFn;

        Durations Loop()
        {
            var startTime = now();
            var lastRunTime = startTime;
            while (true)
            {
                var currentTime = now();
                var runningBehind = currentTime > lastRunTime + (TimeSpan)interval;
                var boundary = interval == Duration.Zero
                    ? interval
                    : SecondsToIntervalStart(startTime, currentTime, interval);
                var sleepTime = boundary == Duration.Zero ? interval : boundary;
                lastRunTime = runningBehind ? currentTime : currentTime + (TimeSpan)sleepTime;
                yield return runningBehind ? Duration.Zero : sleepTime;
            }
        }

        return Loop().ToSchedule();
    }

    ///<summary>
    /// A schedule that divides the timeline to `interval`-long windows, and sleeps
    /// until the nearest window boundary every time it recurs.
    ///
    /// For example, `Windowed(10*seconds)` would produce a schedule as follows:
    /// <pre>
    ///      10s        10s        10s       10s
    /// |----------|----------|----------|----------|
    /// |action------|sleep---|act|-sleep|action----|
    /// </pre>
    /// </summary>
    /// <param name="interval">schedule interval</param>
    /// <param name="currentTimeFn">current time function</param>
    [Pure]
    public static Schedule Windowed(Duration interval, Func<DateTime>? currentTimeFn = null)
    {
        var now = currentTimeFn ?? LiveNowFn;

        Durations Loop()
        {
            var startTime = now();
            while (true)
            {
                var currentTime = now();
                yield return SecondsToIntervalStart(startTime, currentTime, interval);
            }
        }

        return Loop().ToSchedule();
    }

    [Pure]
    private static int DurationToIntervalStart(int intervalStart, int currentIntervalPosition, int intervalWidth)
    {
        var steps = intervalStart - currentIntervalPosition;
        return steps > 0 ? steps : steps + intervalWidth;
    }

    [Pure]
    private static int RoundBetween(int value, int min, int max) =>
        value > max
            ? max
            : value < min
                ? min
                : value;

    /// <summary>
    /// Cron-like schedule that recurs every specified `second` of each minute.
    /// </summary>
    /// <param name="second">second of the minute, will be rounded to fit between 0 and 59</param>
    /// <param name="currentTimeFn">current time function</param>
    [Pure]
    public static Schedule SecondOfMinute(int second, Func<DateTime>? currentTimeFn = null)
    {
        var now = currentTimeFn ?? LiveNowFn;

        Durations Loop()
        {
            while (true)
                yield return DurationToIntervalStart(RoundBetween(second, 0, 59), now().Second, 60) * seconds;
        }

        return Loop().ToSchedule();
    }

    /// <summary>
    /// Cron-like schedule that recurs every specified `minute` of each hour.
    /// </summary>
    /// <param name="minute">minute of the hour, will be rounded to fit between 0 and 59</param>
    /// <param name="currentTimeFn">current time function</param>
    [Pure]
    public static Schedule MinuteOfHour(int minute, Func<DateTime>? currentTimeFn = null)
    {
        var now = currentTimeFn ?? LiveNowFn;

        Durations Loop()
        {
            while (true)
                yield return DurationToIntervalStart(RoundBetween(minute, 0, 59), now().Minute, 60) * minutes;
        }

        return Loop().ToSchedule();
    }

    /// <summary>
    /// Cron-like schedule that recurs every specified `hour` of each day.
    /// </summary>
    /// <param name="hour">hour of the day, will be rounded to fit between 0 and 23</param>
    /// <param name="currentTimeFn">current time function</param>
    [Pure]
    public static Schedule HourOfDay(int hour, Func<DateTime>? currentTimeFn = null)
    {
        var now = currentTimeFn ?? LiveNowFn;

        Durations Loop()
        {
            while (true)
                yield return DurationToIntervalStart(RoundBetween(hour, 0, 23), now().Hour, 24) * hours;
        }

        return Loop().ToSchedule();
    }

    /// <summary>
    /// Cron-like schedule that recurs every specified `day` of each week.
    /// </summary>
    /// <param name="day">day of the week</param>
    /// <param name="currentTimeFn">current time function</param>
    [Pure]
    public static Schedule DayOfWeek(DayOfWeek day, Func<DateTime>? currentTimeFn = null)
    {
        var now = currentTimeFn ?? LiveNowFn;

        Durations Loop()
        {
            while (true)
                yield return DurationToIntervalStart((int)day + 1, (int)now().DayOfWeek + 1, 7) * days;
        }

        return Loop().ToSchedule();
    }

    /// <summary>
    /// A schedule transformer that will enforce the first retry has no delay.
    /// </summary>
    [Pure]
    public static ScheduleTransformer NoDelayOnFirst =>
        s => s.AsEnumerable().Tail().Prepend(Duration.Zero).ToSchedule();

    /// <summary>
    /// A schedule transformer that limits the returned delays to max delay.
    /// </summary>
    /// <param name="max">max delay to return</param>
    [Pure]
    public static ScheduleTransformer MaxDelay(Duration max) =>
        s => s.AsEnumerable().Select(x => x > max ? max : x).ToSchedule();

    /// <summary>
    /// Limits the schedule to the max cumulative delay.
    /// </summary>
    /// <param name="max">max delay to stop schedule at</param>
    [Pure]
    public static ScheduleTransformer MaxCumulativeDelay(Duration max) =>
        s =>
        {
            var totalAppliedDelay = Duration.Zero;

            Durations Loop()
            {
                foreach (var duration in s.AsEnumerable())
                {
                    if (totalAppliedDelay >= max) yield break;
                    totalAppliedDelay += duration;
                    yield return duration;
                }
            }

            return Loop().ToSchedule();
        };

    /// <summary>
    /// A schedule transformer that adds a random jitter to any returned delay.
    /// </summary>
    /// <param name="minRandom">min random milliseconds</param>
    /// <param name="maxRandom">max random milliseconds</param>
    /// <param name="seed">optional seed</param>
    [Pure]
    public static ScheduleTransformer Jitter(Duration minRandom, Duration maxRandom, Option<int> seed = default) =>
        s => s.AsEnumerable()
            .Select<Duration, Duration>(x => x + SingletonRandom.Uniform(minRandom, maxRandom, seed))
            .ToSchedule();

    /// <summary>
    /// A schedule transformer that adds a random jitter to any returned delay.
    /// </summary>
    /// <param name="factor">jitter factor based on the returned delay</param>
    /// <param name="seed">optional seed</param>
    [Pure]
    public static ScheduleTransformer Jitter(double factor = 0.5, Option<int> seed = default) =>
        s => s.AsEnumerable()
            .Select<Duration, Duration>(x => x + SingletonRandom.Uniform(0, x * factor, seed))
            .ToSchedule();

    /// <summary>
    /// Transforms the schedule by decorrelating each of the durations both up and down in a jittered way.
    /// </summary>
    /// <remarks>
    /// Given a linear schedule starting at 100. (100,200,300...)
    /// Adding decorrlation to it might produce a result like this, (103.2342, 97.123, 202.3213, 197.321...)
    /// The overall backoff takes twice as long but should not be correlated when occurring in parallel.
    /// </remarks>
    /// <param name="factor">jitter factor based on the returned delay</param>
    /// <param name="seed">optional seed</param>
    [Pure]
    public static ScheduleTransformer Decorrelate(double factor = 0.1, Option<int> seed = default) =>
        s =>
        {
            Durations Loop()
            {
                using var enumerator = s.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    double currentMilliseconds = enumerator.Current;
                    var rand1 = SingletonRandom.Uniform(0, currentMilliseconds * factor, seed);
                    var rand2 = SingletonRandom.Uniform(0, currentMilliseconds * factor, seed);
                    yield return currentMilliseconds + rand1;
                    yield return currentMilliseconds - rand2;
                }
            }

            return Loop().ToSchedule();
        };

    /// <summary>
    /// Resets the schedule after a provided cumulative max duration.
    /// </summary>
    /// <param name="max">max delay to reset the schedule at</param>
    [Pure]
    public static ScheduleTransformer ResetAfter(Duration max) =>
        s =>
        {
            var iteratedSchedule = (s | MaxCumulativeDelay(max)).AsEnumerable().ToSeq();

            Durations Loop()
            {
                while (true)
                    foreach (var duration in iteratedSchedule)
                        yield return duration;
            }

            return Loop().ToSchedule();
        };

    /// <summary>
    /// Repeats the schedule n number of times.
    /// </summary>
    /// <param name="times">number of times to repeat the schedule</param>
    [Pure]
    public static ScheduleTransformer Repeat(int times) =>
        s =>
        {
            var iteratedSchedule = s.ToSeq();

            Durations Loop()
            {
                for (var i = 0; i < times; i++)
                    foreach (var duration in iteratedSchedule)
                        yield return duration;
            }

            return Loop().ToSchedule();
        };

    /// <summary>
    /// Intersperse the provided duration(s) between each duration in the schedule.
    /// </summary>
    /// <param name="duration">schedule to intersperse</param>
    [Pure]
    public static ScheduleTransformer Intersperse(Schedule schedule) =>
        s => s.AsEnumerable().SelectMany(schedule.Prepend).ToSchedule();

    /// <summary>
    /// Intersperse the provided duration(s) between each duration in the schedule.
    /// </summary>
    /// <param name="durations">1 or more durations to intersperse</param>
    [Pure]
    public static ScheduleTransformer Intersperse(params Duration[] durations) =>
        Intersperse(FromDurations(durations));
}
