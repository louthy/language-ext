using System;
using LanguageExt;
using LanguageExt.Pipes;
using LanguageExt.Sys;
using LanguageExt.Sys.IO;
using LanguageExt.Sys.Live;
using static LanguageExt.Pipes.Proxy;
using static LanguageExt.Prelude;

namespace TestBed;

public static class PipesTestBed
{
    public static Effect<Runtime, Unit> effect1 =>
        Producer.repeatM(Time<Runtime>.nowUTC) | writeLine<DateTime>(); 
        
    static Consumer<Runtime, X, Unit> writeLine<X>() =>
        from x in awaiting<X>()
        from _ in Console<Runtime>.writeLine($"{x}")
        from r in writeLine<X>()
        select r;
    
    public static Effect<Runtime, Unit> effect => 
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
}
