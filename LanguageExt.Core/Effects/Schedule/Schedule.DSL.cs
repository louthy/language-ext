using System;
using static LanguageExt.UnitsOfMeasure;

namespace LanguageExt;

/// <summary>
/// Time series of durations
/// </summary>
record SchItems(Iterator<Duration> Items) : Schedule
{
    public override IteratorIO<Duration> Run() =>
        Items.AsIteratorIO();
}

/// <summary>
/// Time series of durations
/// </summary>
record SchItems2(IteratorIO<Duration> Items) : Schedule
{
    public override IteratorIO<Duration> Run() =>
        Items;
}

/// <summary>
/// Functor map
/// </summary>
record SchMap(Schedule Schedule, Func<Duration, Duration> F) : Schedule 
{
    public override IteratorIO<Duration> Run() =>
        Schedule.Run().Map(F);
}

/// <summary>
/// Functor map
/// </summary>
record SchMapIndex(Schedule Schedule, Func<Duration, long, Duration> F) : Schedule 
{
    public override IteratorIO<Duration> Run() =>
        Schedule.Run().Map(F);
}

/// <summary>
/// Filter
/// </summary>
record SchFilter(Schedule Schedule, Func<Duration, bool> Pred) : Schedule 
{
    public override IteratorIO<Duration> Run() =>
        Schedule.Run().Filter(Pred);
}

/// <summary>
/// Functor bind
/// </summary>
record SchBind(Schedule Schedule, Func<Duration, Schedule> BindF) : Schedule
{
    public override IteratorIO<Duration> Run() =>
        Schedule.Run().Bind(x => BindF(x).Run());
}    

/// <summary>
/// Functor bind and project
/// </summary>
record SchBind2(Schedule Schedule, Func<Duration, Schedule> BindF, Func<Duration, Duration, Duration> Project) : Schedule
{
    public override IteratorIO<Duration> Run() =>
        Schedule.Run().Bind(x => BindF(x).Run().Map(y => Project(x, y)));
}

/// <summary>
/// Tail of sequence
/// </summary>
record SchTail(Schedule Schedule) : Schedule
{
    public override IteratorIO<Duration> Run() =>
        Schedule.Run().Tail();
}    

/// <summary>
/// Skip items in sequence
/// </summary>
record SchSkip(Schedule Schedule, int Count) : Schedule
{
    public override IteratorIO<Duration> Run() =>
        Schedule.Run().Skip(Count);
}    

/// <summary>
/// Take items in sequence
/// </summary>
record SchTake(Schedule Schedule, int Count) : Schedule
{
    public override IteratorIO<Duration> Run() =>
        Schedule.Run().Take(Count);
}

/// <summary>
/// Append in sequence
/// </summary>
record SchCombine(Schedule Left, Schedule Right) : Schedule
{
    public override IteratorIO<Duration> Run() =>
        Left.Run() + Right.Run();
}    

/// <summary>
/// Interleave items in sequence
/// </summary>
record SchInterleave(Schedule Left, Schedule Right) : Schedule
{
    public override IteratorIO<Duration> Run() =>
        Left.Run()
            .Zip(Right.Run(), static (d1, d2) => IteratorIO.forward(d1, d2))
            .Flatten();
}

/// <summary>
/// Union sequence
/// </summary>
record SchUnion(Schedule Left, Schedule Right) : Schedule
{
    public override IteratorIO<Duration> Run() =>
        Left.Run().Union(Right.Run(), join: (x, y) => Math.Min(x, y));
}

/// <summary>
/// Intersect sequence
/// </summary>
record SchIntersect(Schedule Left, Schedule Right) : Schedule
{
    public override IteratorIO<Duration> Run() =>
        Left.Run()
            .Zip(Right.Run())
            .Map(static t => (Duration)Math.Max(t.First, t.Second));
}    

/// <summary>
/// Cons an item onto sequence
/// </summary>
record SchCons(Duration Left, Schedule Right) : Schedule
{
    public override IteratorIO<Duration> Run() =>
        IteratorIO.cons(Left, Right.Run());
}

record SchRepeatForever(Schedule Schedule) : Schedule
{
    public override IteratorIO<Duration> Run() =>
        Schedule.Run().Repeat();
}

record SchLinear(Duration Seed, double Factor) : Schedule
{
    public override IteratorIO<Duration> Run()
    {
        return go(Seed, Seed * Factor);

        IteratorIO<Duration> go(Duration acc, Duration delay) =>
            IteratorIO.cons(acc, () => go(acc + delay, delay));
    }
}

record SchFibonacci(Duration Seed) : Schedule
{
    public override IteratorIO<Duration> Run()
    {
        var last = Duration.Zero;
        return go(Seed, last);

        IteratorIO<Duration> go(Duration acc, Duration last) =>
            IteratorIO.cons(acc, () => go(acc + last, acc) );
    }
}

record SchForever : Schedule
{
    public static readonly Schedule Default = new SchForever();

    public override IteratorIO<Duration> Run() => 
        IteratorIO.forever(Duration.Zero);
}

record SchNever : Schedule
{
    public static readonly Schedule Default = new SchNever();

    public override IteratorIO<Duration> Run() =>
        IteratorIO.empty<Duration>();
}

record SchUpTo(Duration Max, Func<DateTime>? CurrentTimeFn = null) : Schedule
{
    public override IteratorIO<Duration> Run()
    {
        var now = IteratorIO.liftIO(IO.lift(CurrentTimeFn ?? LiveNowFn));

        return now >> go;

        IteratorIO<Duration> go(DateTime startTime) =>
            now >> (n => n - startTime < Max
                             ? IteratorIO.empty<Duration>()
                             : IteratorIO.cons(Duration.Zero, () => go(startTime)));
    }
}

record SchFixed(Duration Interval, Func<DateTime>? CurrentTimeFn = null) : Schedule
{
    public override IteratorIO<Duration> Run()
    {
        var now = IteratorIO.liftIO(IO.lift(CurrentTimeFn ?? LiveNowFn));
        
        return now >> (n => go(n, n));

        IteratorIO<Duration> go(DateTime startTime, DateTime lastRunTime) =>
            from currentTime in now

            let runningBehind = currentTime > lastRunTime + (TimeSpan)Interval

            let boundary = Interval == Duration.Zero
                               ? Interval
                               : secondsToIntervalStart(startTime, currentTime, Interval)

            let sleepTime = boundary == Duration.Zero
                                ? Interval
                                : boundary

            let lastRunTime1 = runningBehind
                                   ? currentTime
                                   : currentTime + (TimeSpan)sleepTime

            from d in IteratorIO.cons(runningBehind ? Duration.Zero : sleepTime, () => go(startTime, lastRunTime1))
            
            select d;
    }
}

record SchWindowed(Duration Interval, Func<DateTime>? CurrentTimeFn = null) : Schedule
{
    public override IteratorIO<Duration> Run()
    {
        var now = IteratorIO.liftIO(IO.lift(CurrentTimeFn ?? LiveNowFn));
        return now >> go;

        IteratorIO<Duration> go(DateTime startTime) =>
            now >> (ct => IteratorIO.cons(secondsToIntervalStart(startTime, ct, Interval), () => go(startTime)));
    }
}

record SchSecondOfMinute(int Second, Func<DateTime>? CurrentTimeFn = null) : Schedule
{
    public override IteratorIO<Duration> Run()
    {
        var now = IteratorIO.liftIO(IO.lift(CurrentTimeFn ?? LiveNowFn));
        return go();

        IteratorIO<Duration> go() =>
            now >> 
            (n => IteratorIO.cons(durationToIntervalStart(roundBetween(Second, 0, 59), n.Second, 60) * seconds, go));
    }
}

record SchMinuteOfHour(int Minute, Func<DateTime>? CurrentTimeFn = null) : Schedule
{
    public override IteratorIO<Duration> Run()
    {
        var now = IteratorIO.liftIO(IO.lift(CurrentTimeFn ?? LiveNowFn));
        return go();

        IteratorIO<Duration> go() =>
            now >> 
            (n => IteratorIO.cons(durationToIntervalStart(roundBetween(Minute, 0, 59), n.Minute, 60) * minutes, go));
    }
}

record SchHourOfDay(int Hour, Func<DateTime>? CurrentTimeFn = null) : Schedule
{
    public override IteratorIO<Duration> Run()
    {
        var now = IteratorIO.liftIO(IO.lift(CurrentTimeFn ?? LiveNowFn));
        return go();

        IteratorIO<Duration> go() =>
            now >> 
            (n => IteratorIO.cons(durationToIntervalStart(roundBetween(Hour, 0, 23), n.Hour, 24) * hours, go));
    }
}

record SchDayOfWeek(DayOfWeek Day, Func<DateTime>? CurrentTimeFn = null) : Schedule
{
    public override IteratorIO<Duration> Run()
    {
        var now = IteratorIO.liftIO(IO.lift(CurrentTimeFn ?? LiveNowFn));
        return go();

        IteratorIO<Duration> go() =>
            now >> 
            (n => IteratorIO.cons(durationToIntervalStart((int)Day + 1, (int)n.DayOfWeek + 1, 7) * days, go));
    }
}

record SchMaxDelay(Schedule Schedule, Duration Max) : Schedule
{
    public override IteratorIO<Duration> Run() =>
        Schedule.Run().Map(x => x > Max ? Max : x);
}

record SchMaxCumulativeDelay(Schedule Schedule, Duration Max) : Schedule
{
    public override IteratorIO<Duration> Run()
    {
        return go(Schedule.Run(), Duration.Zero);
        IteratorIO<Duration> go(IteratorIO<Duration> schedule, Duration total) =>
            IteratorIO.liftIO(
                schedule.NextIO()
                        .Map(head => head switch
                                     {
                                         (Exist<Duration> (var d), var ds) =>
                                             total >= Max
                                                 ? IteratorIO.empty<Duration>()
                                                 : IteratorIO.cons(d, () => go(ds, total + d)),

                                         _ => IteratorIO.empty<Duration>()
                                     }));
    }
}

record SchJitter1(Schedule Schedule, Duration MinRandom, Duration MaxRandom, Option<int> Seed) : Schedule
{
    public override IteratorIO<Duration> Run() =>
        Schedule.Run().Map(x => (Duration)(x + SingletonRandom.Uniform(MinRandom, MaxRandom, Seed)));
}

record SchJitter2(Schedule Schedule, double Factor, Option<int> Seed) : Schedule
{
    public override IteratorIO<Duration> Run() =>
        Schedule.Run().Map(x => (Duration)(x + SingletonRandom.Uniform(0, x * Factor, Seed)));
}

record SchDecorrelate(Schedule Schedule, double Factor, Option<int> Seed) : Schedule
{
    public override IteratorIO<Duration> Run() =>
        Schedule.Run() >> (ms => IteratorIO.forward<Duration>(
                               ms + SingletonRandom.Uniform(0, ms * Factor, Seed),
                               ms - SingletonRandom.Uniform(0, ms * Factor, Seed)));
}

record SchResetAfter(Schedule Schedule, Duration Max) : Schedule
{
    public override IteratorIO<Duration> Run() =>
        (Schedule | maxCumulativeDelay(Max) | RepeatForever).Run();
}

record SchRepeat(Schedule Schedule, int Times) : Schedule
{
    public override IteratorIO<Duration> Run()
    {
        return go(Times);
        IteratorIO<Duration> go(int times) =>
            times <= 0
                ? IteratorIO.empty<Duration>()
                : Schedule.Run() + (() => go(times - 1));
    }
}
