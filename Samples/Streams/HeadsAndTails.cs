using LanguageExt;
using static LanguageExt.Prelude;

namespace Streams;

public static class HeadsAndTails
{
    static readonly StreamT<IO, int> collection = 
        from item in Iterable(1, 2, 3, 4, 5, 6, 7, 8, 9, 10).AsStream<IO>()
        from col  in Console.colour >> 
                     Console.red
        from _    in Console.writeLine($"Evaluated item: {item}") >> 
                     Console.setForegroundColour(col)
        select item;

    public static IO<Unit> run =>
        headExample >>
        tailExample.Iter();

    static IO<Unit> headExample =>
        from _ in Console.writeLine("Head example")
        from h in collection.Head
        from x in Console.writeLine($"\tHead is: {h}")
        select unit;

    static StreamT<IO, Unit> tailExample =>
        from _ in Console.writeLine("Tail example")
        from x in collection.Tail
        from y in Console.writeLine($"\tYielded: {x}")
        where false
        select unit;
    
}
