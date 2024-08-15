using LanguageExt;
using static LanguageExt.Prelude;

namespace Streams;

public static class Menu
{
    public static IO<Unit> run =>
        from _  in introduction
        from ky in Console.readKey >> Console.green
        from ex in ky.Key switch
                   {
                       ConsoleKey.D1 => CountForever.run,
                       ConsoleKey.D2 => CountForeverAsync.run,
                       ConsoleKey.D3 => SumOfSquares.run,
                       ConsoleKey.D4 => Grouping.run,
                       ConsoleKey.D5 => RecursionIO.run,
                       ConsoleKey.D6 => HeadsAndTails.run,
                       _             => unitIO
                   }
        from _1 in run
        select ex;

    static IO<Unit> introduction =>
        Console.cyan                                                                  >>
        Console.writeLine("1. Count forever example")                                 >>
        Console.writeLine("2. Count forever example (async, with per-item 1s delay)") >>
        Console.writeLine("3. Sum of squares example")                                >>
        Console.writeLine("4. Grouping test")                                         >>
        Console.writeLine("5. IO recursion test (currently broken)")                  >>
        Console.writeLine("6. Heads and tails example")                               >>
        Console.writeLine("Enter a number for the example you wish to run");
}
