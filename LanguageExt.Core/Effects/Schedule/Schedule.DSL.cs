using System;
using System.Linq;
using System.Collections.Generic;
using static LanguageExt.UnitsOfMeasure;

namespace LanguageExt;

/// <summary>
/// Time series of durations
/// </summary>
record SchItems(Iterator<Duration> Items) : Schedule
{
    public override Iterator<Duration> Run() =>
        Items;
}

/// <summary>
/// Functor map
/// </summary>
record SchMap(Schedule Schedule, Func<Duration, Duration> F) : Schedule 
{
    public override Iterator<Duration> Run() =>
        Schedule.Run().Map(F);
}

/// <summary>
/// Functor map
/// </summary>
record SchMapIndex(Schedule Schedule, Func<Duration, long, Duration> F) : Schedule 
{
    public override Iterator<Duration> Run() =>
        Schedule.Run().Map(F);
}

/// <summary>
/// Filter
/// </summary>
record SchFilter(Schedule Schedule, Func<Duration, bool> Pred) : Schedule 
{
    public override Iterator<Duration> Run() =>
        Schedule.Run().Filter(Pred);
}

/// <summary>
/// Functor bind
/// </summary>
record SchBind(Schedule Schedule, Func<Duration, Schedule> BindF) : Schedule
{
    public override Iterator<Duration> Run() =>
        Schedule.Run().Bind(x => BindF(x).Run());
}    

/// <summary>
/// Functor bind and project
/// </summary>
record SchBind2(Schedule Schedule, Func<Duration, Schedule> BindF, Func<Duration, Duration, Duration> Project) : Schedule
{
    public override Iterator<Duration> Run() =>
        Schedule.Run().Bind(x => BindF(x).Run().Map(y => Project(x, y)));
}

/// <summary>
/// Tail of sequence
/// </summary>
record SchTail(Schedule Schedule) : Schedule
{
    public override Iterator<Duration> Run() =>
        Schedule.Run() switch
        {
            (Exist<Duration>, var tail) => tail,
            _                           => Iterator.empty<Duration>()
        };
}    

/// <summary>
/// Skip items in sequence
/// </summary>
record SchSkip(Schedule Schedule, int Count) : Schedule
{
    public override Iterator<Duration> Run() =>
        Schedule.Run().Skip(Count);
}    

/// <summary>
/// Take items in sequence
/// </summary>
record SchTake(Schedule Schedule, int Count) : Schedule
{
    public override Iterator<Duration> Run() =>
        Schedule.Run().Take(Count);
}

/// <summary>
/// Append in sequence
/// </summary>
record SchCombine(Schedule Left, Schedule Right) : Schedule
{
    public override Iterator<Duration> Run() =>
        Left.Run().Combine(Right.Run());
}    

/// <summary>
/// Interleave items in sequence
/// </summary>
record SchInterleave(Schedule Left, Schedule Right) : Schedule
{
    public override Iterator<Duration> Run() =>
        Left.Run()
            .Zip(Right.Run(), static (d1, d2) => Iterator.forward(d1, d2))
            .Flatten();
}

/// <summary>
/// Union sequence
/// </summary>
record SchUnion(Schedule Left, Schedule Right) : Schedule
{
    public override Iterator<Duration> Run() 
    {
        // TODO: Build Union into Iterator
        return Iterator.forward(Go());
        IEnumerable<Duration> Go()
        {
            var aEnumerator = Left.Run().GetEnumerator();
            var bEnumerator = Right.Run().GetEnumerator();

            var hasA = aEnumerator.MoveNext();
            var hasB = bEnumerator.MoveNext();

            while (hasA || hasB)
            {
                yield return hasA switch
                             {
                                 true when hasB => Math.Min(aEnumerator.Current, bEnumerator.Current),
                                 true           => aEnumerator.Current,
                                 _              => bEnumerator.Current
                             };

                hasA = hasA && aEnumerator.MoveNext();
                hasB = hasB && bEnumerator.MoveNext();
            }
        }
    }
}

/// <summary>
/// Intersect sequence
/// </summary>
record SchIntersect(Schedule Left, Schedule Right) : Schedule
{
    public override Iterator<Duration> Run() =>
        Left.Run()
            .Zip(Right.Run())
            .Map(static t => (Duration)Math.Max(t.First, t.Second));
}    

/// <summary>
/// Cons an item onto sequence
/// </summary>
record SchCons(Duration Left, Schedule Right) : Schedule
{
    public override Iterator<Duration> Run() =>
        Iterator.cons(Left, Right.Run());
}

record SchRepeatForever(Schedule Schedule) : Schedule
{
    public override Iterator<Duration> Run()
    {
        // TODO: Build Repeat, RepeatUntil, ... into Iterator
        return Iterator.forward(Go());
        
        IEnumerable<Duration> Go()
        {
            while (true)
                foreach (var x in Schedule.Run())
                    yield return x;
        }
    }
}

record SchLinear(Duration Seed, double Factor) : Schedule
{
    public override Iterator<Duration> Run()
    {
        // TODO: Refactor this to be deterministic (no enumerable)
        return Iterator.forward(Go());
        IEnumerable<Duration> Go()
        {
            Duration delayToAdd  = Seed * Factor;
            var      accumulator = Seed;

            yield return accumulator;
            while (true)
            {
                accumulator += delayToAdd;
                yield return accumulator;
            }
        }
    }
}

record SchFibonacci(Duration Seed) : Schedule
{
    public override Iterator<Duration> Run()
    {
        // TODO: Refactor this to be deterministic (no enumerable)
        return Iterator.forward(Go());
        IEnumerable<Duration> Go()
        {
            var last        = Duration.Zero;
            var accumulator = Seed;

            yield return accumulator;
            while (true)
            {
                var current = accumulator;
                accumulator += last;
                last        =  current;
                yield return accumulator;
            }
        }
    }
}

record SchForever : Schedule
{
    public static readonly Schedule Default = new SchForever();

    public override Iterator<Duration> Run()
    {
        // TODO: Refactor this to be deterministic (no enumerable)
        return Iterator.forward(Go());
        IEnumerable<Duration> Go()
        {
            while(true) yield return Duration.Zero;
        }
    }
}

record SchNever : Schedule
{
    public static readonly Schedule Default = new SchNever();

    public override Iterator<Duration> Run() =>
        Iterator.empty<Duration>();
}

record SchUpTo(Duration Max, Func<DateTime>? CurrentTimeFn = null) : Schedule
{
    public override Iterator<Duration> Run()
    {
        // TODO: Refactor this to be deterministic (no enumerable)
        return Iterator.forward(Go());
        IEnumerable<Duration> Go()
        {
            var now       = CurrentTimeFn ?? LiveNowFn;
            var startTime = now();
        
            while (now() - startTime < Max) 
                yield return Duration.Zero;
        }
    }
}

record SchFixed(Duration Interval, Func<DateTime>? CurrentTimeFn = null) : Schedule
{
    public override Iterator<Duration> Run()
    {
        // TODO: Refactor this to be deterministic (no enumerable)
        return Iterator.forward(Go());
        IEnumerable<Duration> Go()
        {
            var now         = CurrentTimeFn ?? LiveNowFn;
            var startTime   = now();
            var lastRunTime = startTime;
            while (true)
            {
                var currentTime   = now();
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
}

record SchWindowed(Duration Interval, Func<DateTime>? CurrentTimeFn = null) : Schedule
{
    public override Iterator<Duration> Run()
    {
        // TODO: Refactor this to be deterministic (no enumerable)
        return Iterator.forward(Go());
        IEnumerable<Duration> Go()
        {
            var now       = CurrentTimeFn ?? LiveNowFn;
            var startTime = now();
            while (true)
            {
                var currentTime = now();
                yield return secondsToIntervalStart(startTime, currentTime, Interval);
            }
        }
    }
}

record SchSecondOfMinute(int Second, Func<DateTime>? CurrentTimeFn = null) : Schedule
{
    public override Iterator<Duration> Run()
    {
        // TODO: Refactor this to be deterministic (no enumerable)
        return Iterator.forward(Go());
        IEnumerable<Duration> Go()
        {
            var now = CurrentTimeFn ?? LiveNowFn;
            while (true)
                yield return durationToIntervalStart(roundBetween(Second, 0, 59), now().Second, 60) * seconds;
        }
    }
}

record SchMinuteOfHour(int Minute, Func<DateTime>? CurrentTimeFn = null) : Schedule
{
    public override Iterator<Duration> Run()
    {
        // TODO: Refactor this to be deterministic (no enumerable)
        return Iterator.forward(Go());
        IEnumerable<Duration> Go()
        {
            var now = CurrentTimeFn ?? LiveNowFn;
            while (true)
                yield return durationToIntervalStart(roundBetween(Minute, 0, 59), now().Minute, 60) * minutes;
        }
    }
}

record SchHourOfDay(int Hour, Func<DateTime>? CurrentTimeFn = null) : Schedule
{
    public override Iterator<Duration> Run()
    {
        // TODO: Refactor this to be deterministic (no enumerable)
        return Iterator.forward(Go());
        IEnumerable<Duration> Go()
        {
            var now = CurrentTimeFn ?? LiveNowFn;
            while (true)
                yield return durationToIntervalStart(roundBetween(Hour, 0, 23), now().Hour, 24) * hours;
        }
    }
}

record SchDayOfWeek(DayOfWeek Day, Func<DateTime>? CurrentTimeFn = null) : Schedule
{
    public override Iterator<Duration> Run()
    {
        // TODO: Refactor this to be deterministic (no enumerable)
        return Iterator.forward(Go());
        IEnumerable<Duration> Go()
        {
            var now = CurrentTimeFn ?? LiveNowFn;
            while (true)
                yield return durationToIntervalStart((int)Day + 1, (int)now().DayOfWeek + 1, 7) * days;
        }    
    }
}

record SchMaxDelay(Schedule Schedule, Duration Max) : Schedule
{
    public override Iterator<Duration> Run() =>
        Schedule.Run().Map(x => x > Max ? Max : x);
}

record SchMaxCumulativeDelay(Schedule Schedule, Duration Max) : Schedule
{
    public override Iterator<Duration> Run()
    {
        // TODO: Refactor this to be deterministic (no enumerable)
        return Iterator.forward(Go());
        IEnumerable<Duration> Go()
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
}

record SchJitter1(Schedule Schedule, Duration MinRandom, Duration MaxRandom, Option<int> Seed) : Schedule
{
    public override Iterator<Duration> Run() =>
        Schedule.Run().Map(x => (Duration)(x + SingletonRandom.Uniform(MinRandom, MaxRandom, Seed)));
}

record SchJitter2(Schedule Schedule, double Factor, Option<int> Seed) : Schedule
{
    public override Iterator<Duration> Run() =>
        Schedule.Run().Map(x => (Duration)(x + SingletonRandom.Uniform(0, x * Factor, Seed)));
}

record SchDecorrelate(Schedule Schedule, double Factor, Option<int> Seed) : Schedule
{
    public override Iterator<Duration> Run()
    {
        // TODO: Refactor this to be deterministic (no enumerable)
        return Iterator.forward(Go());
        IEnumerable<Duration> Go()
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
}

record SchResetAfter(Schedule Schedule, Duration Max) : Schedule
{
    public override Iterator<Duration> Run()
    {
        // TODO: Refactor this to be deterministic (no enumerable)
        return Iterator.forward(Go());
        IEnumerable<Duration> Go()
        {
            while (true)
                foreach (var duration in (Schedule | maxCumulativeDelay(Max)).Run())
                    yield return duration;
        }
    }
}

record SchRepeat(Schedule Schedule, int Times) : Schedule
{
    public override Iterator<Duration> Run()
    {
        // TODO: Refactor this to be deterministic (no enumerable)
        return Iterator.forward(Go());
        IEnumerable<Duration> Go()
        {
            for (var i = 0; i < Times; i++)
                foreach (var duration in Schedule.Run())
                    yield return duration;
        }
    }
}
