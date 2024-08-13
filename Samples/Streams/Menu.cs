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
                       _             => unitIO
                   }
        from _1 in run
        select ex;

    static IO<Unit> introduction =>
        from _1 in Console.writeLine("1. Count forever sample")
        from _2 in Console.writeLine("2. Count forever (async) sample")
        from _3 in Console.writeLine("3. Sum of squares example")
        from __ in Console.writeLine("Enter a number for the example you wish to run")
        select unit;
}
