#nullable enable

using System;
using System.Linq;
using System.Collections.Generic;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Time series of durations
/// </summary>
internal record SchItems(IEnumerable<Duration> Items) : Schedule
{
    public override IEnumerable<Duration> Run() =>
        Items;
}

/// <summary>
/// Functor map
/// </summary>
internal record SchMap(Schedule Schedule, Func<Duration, Duration> F) : Schedule 
{
    public override IEnumerable<Duration> Run() =>
        Schedule.Run().Map(F);
}

/// <summary>
/// Functor map
/// </summary>
internal record SchMapIndex(Schedule Schedule, Func<Duration, int, Duration> F) : Schedule 
{
    public override IEnumerable<Duration> Run() =>
        Schedule.Run().Select(F);
}

/// <summary>
/// Filter
/// </summary>
internal record SchFilter(Schedule Schedule, Func<Duration, bool> Pred) : Schedule 
{
    public override IEnumerable<Duration> Run() =>
        Schedule.Run().Filter(Pred);
}

/// <summary>
/// Functor bind
/// </summary>
internal record SchBind(Schedule Schedule, Func<Duration, Schedule> BindF) : Schedule
{
    public override IEnumerable<Duration> Run() =>
        Schedule.Run().Bind(x => BindF(x).Run());
}    

/// <summary>
/// Functor bind and project
/// </summary>
internal record SchBind2(Schedule Schedule, Func<Duration, Schedule> BindF, Func<Duration, Duration, Duration> Project) : Schedule
{
    public override IEnumerable<Duration> Run() =>
        Schedule.Run().Bind(x => BindF(x).Run().Map(y => Project(x, y)));
}

/// <summary>
/// Tail of sequence
/// </summary>
internal record SchTail(Schedule Schedule) : Schedule
{
    public override IEnumerable<Duration> Run() =>
        Schedule.Run().Tail();
}    

/// <summary>
/// Skip items in sequence
/// </summary>
internal record SchSkip(Schedule Schedule, int Count) : Schedule
{
    public override IEnumerable<Duration> Run() =>
        Schedule.Run().Skip(Count);
}    

/// <summary>
/// Take items in sequence
/// </summary>
internal record SchTake(Schedule Schedule, int Count) : Schedule
{
    public override IEnumerable<Duration> Run() =>
        Schedule.Run().Take(Count);
}

/// <summary>
/// Append in sequence
/// </summary>
internal record SchAppend(Schedule Left, Schedule Right) : Schedule
{
    public override IEnumerable<Duration> Run() =>
        Left.Run().Append(Right.Run());
}    

/// <summary>
/// Interleave items in sequence
/// </summary>
internal record SchInterleave(Schedule Left, Schedule Right) : Schedule
{
    public override IEnumerable<Duration> Run() =>
        Left.Run()
            .Zip(Right.Run(), static (d1, d2) => new[] {d1, d2})
            .SelectMany(x => x);
}

/// <summary>
/// Union sequence
/// </summary>
internal record SchUnion(Schedule Left, Schedule Right) : Schedule
{
    public override IEnumerable<Duration> Run()
    {
        using var aEnumerator = Left.Run().GetEnumerator();
        using var bEnumerator = Right.Run().GetEnumerator();

        var hasA = aEnumerator.MoveNext();
        var hasB = bEnumerator.MoveNext();

        while (hasA || hasB)
        {
            yield return hasA switch
            {
                true when hasB => Math.Min(aEnumerator.Current, bEnumerator.Current),
                true => aEnumerator.Current,
                _ => bEnumerator.Current
            };

            hasA = hasA && aEnumerator.MoveNext();
            hasB = hasB && bEnumerator.MoveNext();

        }
    }
}

/// <summary>
/// Intersect sequence
/// </summary>
internal record SchIntersect(Schedule Left, Schedule Right) : Schedule
{
    public override IEnumerable<Duration> Run() =>
        Left.Run()
            .Zip(Right.Run())
            .Select(static t => (Duration)Math.Max(t.Item1, t.Item2));
}    

/// <summary>
/// Cons an item onto sequence
/// </summary>
internal record SchCons(Duration Left, Schedule Right) : Schedule
{
    public override IEnumerable<Duration> Run()
    {
        yield return Left;
        foreach (var r in Right.Run())
        {
            yield return r;
        }
    }
}

internal record SchRepeatForever(Schedule Schedule) : Schedule
{
    public override IEnumerable<Duration> Run()
    {
        while (true)
            foreach (var x in Schedule.Run())
                yield return x;
    }
}

internal record SchLinear(Duration Seed, double Factor) : Schedule
{
    public override IEnumerable<Duration> Run()
    {
        Duration delayToAdd = Seed * Factor;
        var accumulator = Seed;

        yield return accumulator;
        while (true)
        {
            accumulator += delayToAdd;
            yield return accumulator;
        }
    }
}

internal record SchFibonacci(Duration Seed) : Schedule
{
    public override IEnumerable<Duration> Run()
    {
        var last = Duration.Zero;
        var accumulator = Seed;

        yield return accumulator;
        while (true)
        {
            var current = accumulator;
            accumulator += last;
            last = current;
            yield return accumulator;
        }
    }
}

internal record SchForever : Schedule
{
    public static readonly Schedule Default = new SchForever();

    public override IEnumerable<Duration> Run()
    {
        while(true) yield return Duration.Zero;
    }
}

internal record SchNever : Schedule
{
    public static readonly Schedule Default = new SchNever();

    public override IEnumerable<Duration> Run() =>
        Enumerable.Empty<Duration>();
}

internal record SchUpTo(Duration Max, Func<DateTime>? CurrentTimeFn = null) : Schedule
{
    public override IEnumerable<Duration> Run()
    {
        var now = CurrentTimeFn ?? LiveNowFn;
        var startTime = now();
        
        while (now() - startTime < Max) 
            yield return Duration.Zero;
    }
}

internal record SchFixed(Duration Interval, Func<DateTime>? CurrentTimeFn = null) : Schedule
{
    public override IEnumerable<Duration> Run()
    {
        var now = CurrentTimeFn ?? LiveNowFn;
        var startTime = now();
        var lastRunTime = startTime;
        while (true)
        {
            var currentTime = now();
            var runningBehind = currentTime > lastRunTime + (TimeSpan)Interval;
            
            var boundary = Interval == Duration.Zero
                ? Interval
                : secondsToIntervalStart(startTime, currentTime, Interval);
            
            var sleepTime = boundary == Duration.Zero 
                ? Interval 
                : boundary;
            
            lastRunTime = runningBehind ? currentTime : currentTime + (TimeSpan)sleepTime;
            yield return runningBehind ? Duration.Zero : sleepTime;
        }
    }
}

internal record SchWindowed(Duration Interval, Func<DateTime>? CurrentTimeFn = null) : Schedule
{
    public override IEnumerable<Duration> Run()
    {
        var now = CurrentTimeFn ?? LiveNowFn;
        var startTime = now();
        while (true)
        {
            var currentTime = now();
            yield return secondsToIntervalStart(startTime, currentTime, Interval);
        }
    }
}

internal record SchSecondOfMinute(int Second, Func<DateTime>? CurrentTimeFn = null) : Schedule
{
    public override IEnumerable<Duration> Run()
    {
        var now = CurrentTimeFn ?? LiveNowFn;
        while (true)
            yield return durationToIntervalStart(roundBetween(Second, 0, 59), now().Second, 60) * seconds;
    }
}

internal record SchMinuteOfHour(int Minute, Func<DateTime>? CurrentTimeFn = null) : Schedule
{
    public override IEnumerable<Duration> Run()
    {
        var now = CurrentTimeFn ?? LiveNowFn;
        while (true)
            yield return durationToIntervalStart(roundBetween(Minute, 0, 59), now().Minute, 60) * minutes;
    }
}

internal record SchHourOfDay(int Hour, Func<DateTime>? CurrentTimeFn = null) : Schedule
{
    public override IEnumerable<Duration> Run()
    {
        var now = CurrentTimeFn ?? LiveNowFn;
        while (true)
            yield return durationToIntervalStart(roundBetween(Hour, 0, 23), now().Hour, 24) * hours;
    }
}

internal record SchDayOfWeek(DayOfWeek Day, Func<DateTime>? CurrentTimeFn = null) : Schedule
{
    public override IEnumerable<Duration> Run()
    {
        var now = CurrentTimeFn ?? LiveNowFn;
        while (true)
            yield return durationToIntervalStart((int)Day + 1, (int)now().DayOfWeek + 1, 7) * days;
    }
}

internal record SchMaxDelay(Schedule Schedule, Duration Max) : Schedule
{
    public override IEnumerable<Duration> Run() =>
        Schedule.Run().Map(x => x > Max ? Max : x);
}

internal record SchMaxCumulativeDelay(Schedule Schedule, Duration Max) : Schedule
{
    public override IEnumerable<Duration> Run()
    {
        var totalAppliedDelay = Duration.Zero;

        foreach (var duration in Schedule.Run())
        {
            if (totalAppliedDelay >= Max) yield break;
            totalAppliedDelay += duration;
            yield return duration;
        }
    }
}

internal record SchJitter1(Schedule Schedule, Duration MinRandom, Duration MaxRandom, Option<int> Seed) : Schedule
{
    public override IEnumerable<Duration> Run() =>
        Schedule.Run().Map(x => (Duration)(x + SingletonRandom.Uniform(MinRandom, MaxRandom, Seed)));
}

internal record SchJitter2(Schedule Schedule, double Factor, Option<int> Seed) : Schedule
{
    public override IEnumerable<Duration> Run() =>
        Schedule.Run().Map(x => (Duration)(x + SingletonRandom.Uniform(0, x * Factor, Seed)));
}

internal record SchDecorrelate(Schedule Schedule, double Factor, Option<int> Seed) : Schedule
{
    public override IEnumerable<Duration> Run()
    {
        foreach(var currentMilliseconds in Schedule.Run())
        {
            var rand1 = SingletonRandom.Uniform(0, currentMilliseconds * Factor, Seed);
            var rand2 = SingletonRandom.Uniform(0, currentMilliseconds * Factor, Seed);
            yield return currentMilliseconds + rand1;
            yield return currentMilliseconds - rand2;
        }
    }
}

internal record SchResetAfter(Schedule Schedule, Duration Max) : Schedule
{
    public override IEnumerable<Duration> Run()
    {
        while (true)
            foreach (var duration in (Schedule | maxCumulativeDelay(Max)).Run())
                yield return duration;
    }
}

internal record SchRepeat(Schedule Schedule, int Times) : Schedule
{
    public override IEnumerable<Duration> Run()
    {
        for (var i = 0; i < Times; i++)
            foreach (var duration in Schedule.Run())
                yield return duration;
    }
}
