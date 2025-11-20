using System;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Pipes;
using LanguageExt.Sys;
using LanguageExt.Sys.Live;
using static LanguageExt.Prelude;
using static LanguageExt.Pipes.Producer;
using static LanguageExt.Pipes.Consumer;
using static LanguageExt.Pipes.Pipe;
using static LanguageExt.UnitsOfMeasure;

namespace TestBed;

public static class PipesTestBed
{
    static Producer<Runtime, int, Unit> numbers(int n, int failOn) =>
        from _ in yield<Runtime, int>(n)
        from t in Time<Runtime>.sleepFor(1000 * ms)
        from x in failOn == n 
                    ? FailEff<Runtime, Unit>(Error.New($"failed on {n}")) 
                    : unitEff
        from r in n < 0 
                    ? Pure(unit) 
                    : numbers(n - 1, failOn)
        select r;
    
    public static Effect<Runtime, Unit> effect => 
        merge(numbers(10, 5), numbers(20, 0)) | writeLine<int>();
    
    static Consumer<Runtime, X, Unit> writeLine<X>() =>
        from x in awaiting<Runtime, X>()
        from _ in Console<Runtime>.writeLine($"{x}")
        from r in writeLine<X>()
        select r;
    
    public static Effect<Runtime, Unit> effect1 => 
        repeat(producer) | doubleIt | consumer;

    static Producer<Runtime, int, Unit> producer =>
        from _1 in Console<Runtime>.writeLine("before")
        from _2 in yieldAll<Runtime, int>(Range(1, 5))
        from _3 in Console<Runtime>.writeLine("after")
        select unit;

     static Pipe<Runtime, int, int, Unit> doubleIt =>
        from x in awaiting<Runtime, int, int>()
        from _ in yield<Runtime, int, int>(x * 2)
        select unit;
    
    static Consumer<Runtime, int, Unit> consumer =>
        from i in awaiting<Runtime, int>()
        from _ in Console<Runtime>.writeLine(i.ToString())
        from r in consumer
        select r;
}
