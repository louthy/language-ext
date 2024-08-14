using LanguageExt;
using static LanguageExt.Prelude;

namespace Streams;

public static class Menu
{
    public static IO<Unit> run =>
        from _  in introduction
        from ky in Console.readKey
        from ex in ky.Key switch
                   {
                       ConsoleKey.D1 => CountForever.run,
                       ConsoleKey.D2 => CountForeverAsync.run,
                       ConsoleKey.D3 => SumOfSquares.run,
                       ConsoleKey.D4 => Grouping.run,
                       _             => unitIO
                   }
        from _1 in run
        select ex;

    static IO<Unit> introduction =>
        Console.writeLine("1. Count forever sample")                              >>
        Console.writeLine("2. Count forever sample (async, with per-item delay)") >>
        Console.writeLine("3. Sum of squares example")                            >>
        Console.writeLine("4. Grouping test")                                     >>
        Console.writeLine("Enter a number for the example you wish to run");
}
