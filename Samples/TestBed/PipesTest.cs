using LanguageExt;
using LanguageExt.Pipes;
using LanguageExt.Sys;
using LanguageExt.Sys.Live;
using static LanguageExt.Pipes.Proxy;
using static LanguageExt.Prelude;

namespace TestBed;

public static class PipesTestBed
{
    public static Effect<Runtime, Unit> effect => 
        repeat(producer) | consumer;

    static Producer<Runtime, int, Unit> producer =>
        from _1 in Console<Runtime>.writeLine("before")
        from _2 in yieldAll(Range(1, 10))
        from _3 in Console<Runtime>.writeLine("after")
        select unit;
    
    static Consumer<Runtime, int, Unit> consumer =>
        from i in awaiting<int>()
        from _ in Console<Runtime>.writeLine(i.ToString())
        select unit;
    
    /*
    public static Effect<Runtime, Unit> effect =>
        beforeAndAfter | consumer;

    static Producer<Runtime, int, Unit> beforeAndAfter =>
        from _1 in Console<Runtime>.writeLine("before")
        from _3 in producer(10)
        from _2 in Console<Runtime>.writeLine("after")
        select unit;
    
    static Producer<Runtime, int, Unit> producer(int x) =>
        from _ in yield(x)
        from r in x > 0 
                     ? producer(x - 1)
                     : Pure(unit) 
        select r;
 
    static Consumer<Runtime, int, Unit> consumer =>
        from i1 in awaiting<int>()
        from _x in Console<Runtime>.writeLine(i1.ToString())
        from r  in consumer
        select r;*/
}
