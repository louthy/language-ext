using LanguageExt;
using LanguageExt.Pipes;
using LanguageExt.Sys;
using LanguageExt.Sys.Live;
using static LanguageExt.Pipes.Proxy;
using static LanguageExt.Prelude;

namespace TestBed;

public static class PipesTestBed
{
    /*
    public static Effect<Runtime, Unit> effect =>
        beforeAndAfter.For(bodyRun).ToEffect();

    static Effect<Runtime, Unit> bodyRun(Unit _) =>
        producer | consumer;
    
    static Producer<Runtime, Unit, Unit> beforeAndAfter =>
        from _1 in Console<Runtime>.writeLine("before")
        from _ in yield(unit)
        from _3 in Console<Runtime>.writeLine("after")
        select unit;
        */
    
    public static Effect<Runtime, Unit> effect =>
        producer(10) | consumer;

    static Consumer<Runtime, int, Unit> consumer =>
        from i1 in Consumer.awaiting<Runtime, int>()
        from _x in Consumer.lift<Runtime, int, Unit>(Console<Runtime>.writeLine(i1.ToString()))
        select unit;

    static Producer<Runtime, int, Unit> producer(int x) =>
        from _1 in Producer.lift<Runtime, int, Unit>(Console<Runtime>.writeLine("before"))
        from _  in Producer.yield<Runtime, int>(x)
        from nx in x > 0 
                      ? producer(x - 1)
                      : Producer.Pure<Runtime, int, Unit>(unit) 
        from _3 in Producer.lift<Runtime, int, Unit>(Console<Runtime>.writeLine("after"))
        select unit;
}
