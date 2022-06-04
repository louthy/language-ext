using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using static LanguageExt.Prelude;

namespace LanguageExt;

using PositiveDurations = IEnumerable<PositiveDuration>;

public readonly partial struct Schedule
{
    /// <summary>
    /// Identity or noop schedule result transformer.
    /// </summary>
    public static readonly ScheduleTransformer Identity = _ => _;

    private static PositiveDurations InternalForever()
    {
        while (true) yield return PositiveDuration.Zero;
    }

    /// <summary>
    /// Schedule that runs forever.
    /// </summary>
    public static readonly Schedule Forever = InternalForever().ToSchedule();

    /// <summary>
    /// Schedule that never runs.
    /// </summary>
    public static readonly Schedule Never = Enumerable.Empty<PositiveDuration>().ToSchedule();

    /// <summary>
    /// Schedule that runs once.
    /// </summary>
    public static readonly Schedule Once = Forever.AsEnumerable().Take(1).ToSchedule();

    /// <summary>
    /// Schedule that recurs for the specified fixed durations.
    /// </summary>
    /// <param name="durations">durations to apply</param>
    [Pure]
    public static Schedule FromDurations(params PositiveDuration[] durations) => durations.AsEnumerable().ToSchedule();

    /// <summary>
    /// Schedule that recurs for the specified durations.
    /// </summary>
    /// <param name="durations">durations to apply</param>
    [Pure]
    public static Schedule FromDurations(PositiveDurations durations) => durations.ToSchedule();

    /// <summary>
    /// Schedule that recurs the specified number of times.
    /// </summary>
    /// <param name="times">number of times</param>
    [Pure]
    public static Schedule Recurs(int times) => Forever.AsEnumerable().Take(times).ToSchedule();

    /// <summary>
    /// Schedule that recurs continuously with the given spacing.
    /// </summary>
    /// <param name="space">space</param>
    [Pure]
    public static Schedule Spaced(PositiveDuration space) => Forever.AsEnumerable().Select(_ => space).ToSchedule();

    /// <summary>
    /// Schedule that will retry five times and pause 200ms between each call.
    ///
    /// Simple strategy for dealing with transient failures that are not susceptible to
    /// overload from fast retries. 
    /// </summary>
    /// <param name="retry">number of retry attempts, default 5</param>
    /// <param name="space">constant space between each retry, default 200 ms</param>
    /// <param name="fastFirst">flag to indicate the first retry is immediate, default false</param>
    [Pure]
    public static Schedule Spaced(int retry = 5, Option<PositiveDuration> space = default, bool fastFirst = false) =>
        Spaced(space.IfNone(() => 200)) & Recurs(retry) & (fastFirst ? NoDelayOnFirst : Identity);

    /// <summary>
    /// Schedule that recurs continuously using a linear backoff.
    /// </summary>
    /// <param name="seed">seed</param>
    /// <param name="factor">optional factor to apply, default 1</param>
    [Pure]
    public static Schedule Linear(PositiveDuration seed, double factor = 1)
    {
        PositiveDuration delayToAdd = seed * factor;
        var accumulator = seed;

        PositiveDurations Loop()
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
    /// Schedule that will retry five times and pause based on a linear backoff starting at 100 ms.
    ///
    /// Simple strategy for dealing with transient failures that are susceptible to
    /// overload from fast retries. In this scenario, we want to give the effect some time
    /// to stabilize before trying again.
    /// </summary>
    /// <param name="retry">number of retry attempts, default 5</param>
    /// <param name="space">initial space, default 100 ms</param>
    /// <param name="factor">linear factor to apply, default 1</param>
    /// <param name="fastFirst">flag to indicate the first retry is immediate, default false</param>
    public static Schedule Linear(
        int retry = 5, Option<PositiveDuration> space = default, double factor = 1, bool fastFirst = false) =>
        Linear(space.IfNone(() => 100), factor) & Recurs(retry) & (fastFirst ? NoDelayOnFirst : Identity);

    /// <summary>
    /// Schedule that recurs continuously using a exponential backoff.
    /// </summary>
    /// <param name="seed">seed</param>
    /// <param name="factor">optional factor to apply, default 2</param>
    [Pure]
    public static Schedule Exponential(PositiveDuration seed, double factor = 2)
    {
        var accumulator = seed;

        PositiveDurations Loop()
        {
            yield return accumulator;
            while (true)
            {
                accumulator *= factor;
                yield return accumulator;
            }
        }

        return Loop().ToSchedule();
    }

    /// <summary>
    /// Schedule that will retry five times and pauses based on a exponential backoff starting at 100 ms.
    ///
    /// Because of the exponential nature, this is best used with a low starting delay or in out-of-band
    /// communication, such as a service worker polling for information from a remote endpoint.
    /// Due to the potential for rapidly increasing times, care should be taken if an exponential retry
    /// is used in the code path for servicing a user request.
    ///
    /// If the overall amount of time that an exponential-backoff retry policy could take is a concern,
    /// consider combining it with a max cumulative delay,
    ///
    ///     Schedule.Exponential() | Schedule.MaxCumulativeDelay(45*seconds)
    /// 
    /// </summary>
    /// <param name="retry">maximum number of retries to use, default 5</param>
    /// <param name="space">initial space, default 100 ms</param>
    /// <param name="factor">linear factor to apply, default 2</param>
    /// <param name="fastFirst">flag to indicate the first retry is immediate, default false</param>
    public static Schedule Exponential(
        int retry = 5, Option<PositiveDuration> space = default, double factor = 2, bool fastFirst = false) =>
        Exponential(space.IfNone(() => 100), factor) & Recurs(retry) & (fastFirst ? NoDelayOnFirst : Identity);

    /// <summary>
    /// Schedule that recurs continuously using a fibonacci based backoff.
    /// </summary>
    /// <param name="seed">seed</param>
    [Pure]
    public static Schedule Fibonacci(PositiveDuration seed)
    {
        var last = PositiveDuration.Zero;
        var accumulator = seed;

        PositiveDurations Loop()
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

    private static readonly Func<DateTime> LiveNowFn = () => DateTime.Now;

    /// <summary>
    /// Schedule that runs for a given duration.
    /// </summary>
    /// <param name="max">max duration to run the schedule for</param>
    /// <param name="currentTimeFn">current time function</param>
    public static Schedule UpTo(PositiveDuration max, Func<DateTime> currentTimeFn = null)
    {
        var now = currentTimeFn ?? LiveNowFn;

        PositiveDurations Loop()
        {
            var startTime = now();
            while ((PositiveDuration)(startTime - now()) < max) yield return PositiveDuration.Zero;
        }

        return Loop().ToSchedule();
    }

    [Pure]
    private static PositiveDuration SecondsToIntervalStart(
        DateTime startTime, DateTime currentTime, PositiveDuration interval)
        => interval - (currentTime - startTime).TotalMilliseconds % interval;

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
    public static Schedule Fixed(PositiveDuration interval, Func<DateTime> currentTimeFn = null)
    {
        var now = currentTimeFn ?? LiveNowFn;

        PositiveDurations Loop()
        {
            var startTime = now();
            var lastRunTime = startTime;
            while (true)
            {
                var currentTime = now();
                var runningBehind = currentTime > lastRunTime + (TimeSpan)interval;
                var boundary = interval == PositiveDuration.Zero
                    ? interval
                    : SecondsToIntervalStart(startTime, currentTime, interval);
                var sleepTime = boundary == PositiveDuration.Zero ? interval : boundary;
                lastRunTime = runningBehind ? currentTime : currentTime + (TimeSpan)sleepTime;
                yield return runningBehind ? PositiveDuration.Zero : sleepTime;
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
    public static Schedule Windowed(PositiveDuration interval, Func<DateTime> currentTimeFn = null)
    {
        var now = currentTimeFn ?? LiveNowFn;

        PositiveDurations Loop()
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
    private static int RoundBetween(int value, int min, int max)
        => value > max ? max : value < min ? min : value;

    /// <summary>
    /// Cron-like schedule that recurs every specified `second` of each minute.
    /// </summary>
    /// <param name="second">second of the minute, will be rounded to fit between 0 and 59</param>
    /// <param name="currentTimeFn">current time function</param>
    public static Schedule SecondOfMinute(int second, Func<DateTime> currentTimeFn = null)
    {
        var now = currentTimeFn ?? LiveNowFn;

        PositiveDurations Loop()
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
    public static Schedule MinuteOfHour(int minute, Func<DateTime> currentTimeFn = null)
    {
        var now = currentTimeFn ?? LiveNowFn;

        PositiveDurations Loop()
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
    public static Schedule HourOfDay(int hour, Func<DateTime> currentTimeFn = null)
    {
        var now = currentTimeFn ?? LiveNowFn;

        PositiveDurations Loop()
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
    public static Schedule DayOfWeek(DayOfWeek day, Func<DateTime> currentTimeFn = null)
    {
        var now = currentTimeFn ?? LiveNowFn;

        PositiveDurations Loop()
        {
            while (true)
                yield return DurationToIntervalStart((int)day + 1, (int)now().DayOfWeek + 1, 7) * days;
        }

        return Loop().ToSchedule();
    }

    /// <summary>
    /// Generates sleep durations in an jittered manner, making sure to mitigate any correlations.
    /// For example: 117ms, 236ms, 141ms, 424ms, ...
    /// Per the formula from https://aws.amazon.com/blogs/architecture/exponential-backoff-and-jitter/.
    ///
    /// Sudden issues affecting performance, combined with a fixed-progression wait-and-retry,
    /// can lead to subsequent retries being highly correlated. For example, if there are 50 concurrent failures,
    /// and all 50 requests enter a wait-and-retry for 10ms, then all 50 requests will hit the service again in 10ms;
    /// potentially overwhelming the service again.
    /// 
    /// One way to address this is to add some randomness to the wait delay.
    /// This will cause each request to vary slightly on retry, which decorrelates the retries from each other.
    /// </summary>
    /// <remarks>
    /// Source https://github.com/Polly-Contrib/Polly.Contrib.WaitAndRetry#earlier-jitter-recommendations
    /// </remarks>
    /// <param name="minDelay">>minimum delay before each retry, default 10 milliseconds</param>
    /// <param name="maxDelay">maximum delay before each retry, default 100 milliseconds</param>
    /// <param name="retry">maximum number of retries to use, default 5</param>
    /// <param name="seed">optional seed to use</param>
    /// <param name="fastFirst">flag to indicate the first retry is immediate, default false</param>
    [Pure]
    public static Schedule AwsDecorrelated(
        Option<PositiveDuration> minDelay = default,
        Option<PositiveDuration> maxDelay = default,
        int retry = 5,
        Option<int> seed = default,
        bool fastFirst = false)
    {
        var min = minDelay.IfNone(() => 10);
        var max = maxDelay.IfNone(() => 100);

        PositiveDurations Loop()
        {
            while (true)
            {
                var ceiling = Math.Min(max, min * 3);
                yield return SingletonRandom.Uniform(min, ceiling, seed);
            }
        }

        var awsSchedule = Loop().ToSchedule();
        return awsSchedule & Recurs(retry) & (fastFirst ? NoDelayOnFirst : Identity);
    }

    /// <summary>
    /// Generates sleep durations in an exponentially backing-off, jittered manner, making sure to mitigate any correlations.
    /// For example: 850ms, 1455ms, 3060ms.
    ///
    /// Sudden issues affecting performance, combined with a fixed-progression wait-and-retry,
    /// can lead to subsequent retries being highly correlated. For example, if there are 50 concurrent failures,
    /// and all 50 requests enter a wait-and-retry for 10ms, then all 50 requests will hit the service again in 10ms;
    /// potentially overwhelming the service again.
    /// 
    /// One way to address this is to add some randomness to the wait delay.
    /// This will cause each request to vary slightly on retry, which decorrelates the retries from each other.
    /// </summary>
    /// <remarks>
    /// Source https://github.com/Polly-Contrib/Polly.Contrib.WaitAndRetry#new-jitter-recommendation
    /// </remarks>
    /// <param name="medianFirstRetryDelay">median delay to target before the first retry, call it f (= f * 2^0).
    /// Choose this value both to approximate the first delay, and to scale the remainder of the series.
    /// Subsequent retries will (over a large sample size) have a median approximating retries at time f * 2^1, f * 2^2 ... f * 2^t etc for try t.
    /// The actual amount of delay-before-retry for try t may be distributed between 0 and f * (2^(t+1) - 2^(t-1)) for t >= 2;
    /// or between 0 and f * 2^(t+1), for t is 0 or 1, default 100 milliseconds</param>
    /// <param name="retry">The maximum number of retries to use, default 5</param>
    /// <param name="seed">An optional seed to use</param>
    /// <param name="fastFirst">flag to indicate the first retry is immediate, default false</param>
    public static Schedule PollyDecorrelated(
        Option<PositiveDuration> medianFirstRetryDelay = default,
        int retry = 5,
        Option<int> seed = default,
        bool fastFirst = false)
    {
        var mrd = medianFirstRetryDelay.IfNone(() => 100);

        // A factor used within the formula to help smooth the first calculated delay.
        const double pFactor = 4.0;

        // A factor used to scale the median values of the retry times generated by the formula to be _near_ whole seconds, to aid user comprehension.
        // This factor allows the median values to fall approximately at 1, 2, 4 etc seconds, instead of 1.4, 2.8, 5.6, 11.2.
        const double rpScalingFactor = 1 / 1.4d;

        // Upper-bound to prevent overflow beyond TimeSpan.MaxValue. Potential truncation during conversion from double to long
        // (as described at https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/numeric-conversions)
        // is avoided by the arbitrary subtraction of 1000.
        var maxTimeSpanDouble = (double)TimeSpan.MaxValue.Ticks - 1000;

        PositiveDurations Loop()
        {
            var iteration = 0;
            var appliedDelay = PositiveDuration.Zero;
            while (true)
            {
                iteration += 1;
                var t = iteration + SingletonRandom.NextDouble(seed);
                var next = Math.Pow(2, t) * Math.Tanh(Math.Sqrt(pFactor * t));
                var formulaIntrinsicValue = next - appliedDelay;
                PositiveDuration delayToApply = TimeSpan.FromTicks((long)Math.Min(
                    formulaIntrinsicValue * rpScalingFactor * ((TimeSpan)mrd).Ticks,
                    maxTimeSpanDouble));
                appliedDelay += delayToApply;
                yield return delayToApply;
            }
        }

        var pollySchedule = Loop().ToSchedule();
        return pollySchedule & Recurs(retry) & (fastFirst ? NoDelayOnFirst : Identity);
    }

    /// <summary>
    /// A schedule transformer that will enforce the first retry has no delay.
    /// </summary>
    [Pure]
    public static ScheduleTransformer NoDelayOnFirst =>
        s => s.AsEnumerable().Tail().Prepend(PositiveDuration.Zero).ToSchedule();

    /// <summary>
    /// A schedule transformer that limits the returned delays to max delay.
    /// </summary>
    /// <param name="max">max delay to return</param>
    [Pure]
    public static ScheduleTransformer MaxDelay(PositiveDuration max) =>
        s => s.AsEnumerable().Select(x => x > max ? max : x).ToSchedule();

    /// <summary>
    /// Limits the schedule to the max cumulative delay.
    /// </summary>
    /// <param name="max">max delay to stop schedule at</param>
    [Pure]
    public static ScheduleTransformer MaxCumulativeDelay(PositiveDuration max) =>
        s =>
        {
            var totalAppliedDelay = PositiveDuration.Zero;

            PositiveDurations Loop()
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
    public static ScheduleTransformer Jitter(
        PositiveDuration minRandom, PositiveDuration maxRandom, Option<int> seed = default) =>
        s => s.AsEnumerable()
            .Select<PositiveDuration, PositiveDuration>(x => x + SingletonRandom.Uniform(minRandom, maxRandom, seed))
            .ToSchedule();

    /// <summary>
    /// A schedule transformer that adds a random jitter to any returned delay.
    /// </summary>
    /// <param name="factor">jitter factor based on the returned delay</param>
    /// <param name="seed">optional seed</param>
    [Pure]
    public static ScheduleTransformer Jitter(double factor = 0.5, Option<int> seed = default) =>
        s => s.AsEnumerable()
            .Select<PositiveDuration, PositiveDuration>(x => x + SingletonRandom.Uniform(0, x * factor, seed))
            .ToSchedule();

    /// <summary>
    /// Resets the schedule after a provided cumulative max duration.
    /// </summary>
    /// <param name="max">max delay to reset the schedule at</param>
    [Pure]
    public static ScheduleTransformer ResetAfter(PositiveDuration max) =>
        s =>
        {
            var iteratedSchedule = (s | MaxCumulativeDelay(max)).AsEnumerable().ToSeq();

            PositiveDurations Loop()
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

            PositiveDurations Loop()
            {
                for (var i = 0; i < times; i++)
                    foreach (var duration in iteratedSchedule)
                        yield return duration;
            }

            return Loop().ToSchedule();
        };
}
