/*
using LanguageExt;
using LanguageExt.Streaming;
using static Streams.Console;
using static LanguageExt.Prelude;

namespace Streams;

public static class HeadsAndTails
{
    static readonly StreamT<IO, int> collection = 
        from item in Iterable(1, 2, 3, 4, 5, 6, 7, 8, 9, 10).AsStream<IO, int>()
        from col  in colour >> 
                     red
        from _    in writeLine($"\tEvaluated item: {item}") >> 
                     setForegroundColour(col)
        select item;

    public static IO<Unit> run =>
        headExample >>
        tailExample.Run();

    static IO<Unit> headExample =>
        from _ in writeLine("Head example")
        from h in collection.Head()
        from x in writeLine($"\tHead is: {h}")
        select unit;

    static StreamT<IO, Unit> tailExample =>
        from _ in writeLine("Tail example")
        from x in collection.Tail()
        from y in writeLine($"\tYielded: {x}")
        where false
        select unit;
    
}
*/
