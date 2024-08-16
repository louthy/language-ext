using LanguageExt;
using static LanguageExt.Prelude;
using static Streams.Console;

namespace Streams;

public static class Menu
{
    public static IO<Unit> run =>
        from _  in introduction
        from ky in readKey >> green
        from ex in ky.Key switch
                   {
                       ConsoleKey.D1 => CountForever.run,
                       ConsoleKey.D2 => CountForeverAsync.run,
                       ConsoleKey.D3 => SumOfSquares.run,
                       ConsoleKey.D4 => Grouping.run,
                       ConsoleKey.D5 => RecursionIO.run,
                       ConsoleKey.D6 => HeadsAndTails.run,
                       ConsoleKey.D7 => Folding.run,
                       _             => unitIO
                   }
        from _1 in run
        select ex;

    static IO<Unit> introduction =>
        cyan                                                          >>
        writeLine("Examples")                                         >>
        writeLine("1. Count forever")                                 >>
        writeLine("2. Count forever (async, with per-item 1s delay)") >>
        writeLine("3. Sum of squares")                                >>
        writeLine("4. Grouping test")                                 >>
        writeLine("5. IO recursion test (currently broken)")          >>
        writeLine("6. Heads and tails")                               >>
        writeLine("7. Folding")                                       >>
        writeLine("Enter a number for the example you wish to run");
}
