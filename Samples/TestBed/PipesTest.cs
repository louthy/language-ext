using System;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Pipes;
using LanguageExt.Sys;
using LanguageExt.Sys.IO;
using LanguageExt.Sys.Live;
using static LanguageExt.Pipes.Proxy;
using static LanguageExt.Prelude;

namespace TestBed;

public static class PipesTestBed
{
    static Producer<Runtime, int, Unit> numbers(int n, int failOn) =>
        from _ in yield(n)
        from t in Time<Runtime>.sleepFor(1000 * ms)
        from x in failOn == n 
                    ? FailEff<Runtime, Unit>($"failed on {n}") 
                    : unitEff
        from r in n < 0 
                    ? Pure(unit) 
                    : numbers(n - 1, failOn)
        select r;
    
    public static Effect<Runtime, Unit> effect => 
        Producer.merge(numbers(10, 5), numbers(20, 0)) | writeLine<int>();
    
    static Consumer<Runtime, X, Unit> writeLine<X>() =>
        from x in awaiting<X>()
        from _ in Console<Runtime>.writeLine($"{x}")
        from r in writeLine<X>()
        select r;
    
    public static Effect<Runtime, Unit> effect1 => 
        repeat(producer) | doubleIt | consumer;

    static Producer<Runtime, int, Unit> producer =>
        from _1 in Console<Runtime>.writeLine("before")
        from _2 in yieldAll(Range(1, 5))
        from _3 in Console<Runtime>.writeLine("after")
        select unit;

     static Pipe<Runtime, int, int, Unit> doubleIt =>
        from x in awaiting<int>()
        from _ in yield(x * 2)
        select unit;
    
    static Consumer<Runtime, int, Unit> consumer =>
        from i in awaiting<int>()
        from _ in Console<Runtime>.writeLine(i.ToString())
        from r in consumer
        select r;
    
    // TOOD: Resolve recursion issue`
    public static Effect<Runtime, Unit> effect2 =>
        Producer.repeatM(Time<Runtime>.nowUTC)
      | Pipe.mapM<Runtime, DateTime>(dt => 
            from _1 in Console<Runtime>.setColor(ConsoleColor.Green)
            from _2 in Console<Runtime>.writeLine(dt.ToLongTimeString())
            from _3 in Console<Runtime>.resetColor()
            select dt)
      | writeLine<DateTime>();
    
}
