using System;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Pipes;
using LanguageExt.Sys;
using LanguageExt.Sys.Live;
using static LanguageExt.Prelude;
using static LanguageExt.Pipes.Proxy;
using static LanguageExt.UnitsOfMeasure;

namespace TestBed;

public static class PipesTestBed
{
    static Producer<int, Eff<Runtime>, Unit> numbers(int n, int failOn) =>
        from _ in yield(n)
        from t in Time<Eff<Runtime>, Runtime>.sleepFor(1000 * ms)
        from x in failOn == n 
                    ? FailEff<Runtime, Unit>(Error.New($"failed on {n}")) 
                    : unitEff
        from r in n < 0 
                    ? Pure(unit) 
                    : numbers(n - 1, failOn)
        select r;
    
    public static Effect<Eff<Runtime>, Unit> effect => 
        Producer.merge(numbers(10, 5), numbers(20, 0)) | writeLine<int>();
    
    static Consumer<X, Eff<Runtime>, Unit> writeLine<X>() =>
        from x in awaiting<X>()
        from _ in Console<Eff<Runtime>, Runtime>.writeLine($"{x}")
        from r in writeLine<X>()
        select r;
    
    public static Effect<Eff<Runtime>, Unit> effect1 => 
        repeat(producer) | doubleIt | consumer;

    static Producer<int, Eff<Runtime>, Unit> producer =>
        from _1 in Console<Eff<Runtime>, Runtime>.writeLine("before")
        from _2 in yieldAll(Range(1, 5))
        from _3 in Console<Eff<Runtime>, Runtime>.writeLine("after")
        select unit;

     static Pipe<int, int, Eff<Runtime>, Unit> doubleIt =>
        from x in awaiting<int>()
        from _ in yield(x * 2)
        select unit;
    
    static Consumer<int, Eff<Runtime>, Unit> consumer =>
        from i in awaiting<int>()
        from _ in Console<Eff<Runtime>, Runtime>.writeLine(i.ToString())
        from r in consumer
        select r;
    
    public static Effect<Eff<Runtime>, Unit> effect2 =>
        Producer.repeatM(Time<Eff<Runtime>, Runtime>.nowUTC)
      | Pipe.mapM<DateTime, DateTime, Eff<Runtime>, Unit>(dt => 
            from _1 in Console<Eff<Runtime>, Runtime>.setColour(ConsoleColor.Green)
            from _2 in Console<Eff<Runtime>, Runtime>.writeLine(dt.ToLongTimeString())
            from _3 in Console<Eff<Runtime>, Runtime>.resetColour()
            select dt)
      | writeLine<DateTime>();
    
}
