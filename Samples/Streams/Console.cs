using LanguageExt;
using static LanguageExt.Prelude;

namespace Streams;

// Simple IO wrappers of Console 
public static class Console
{
    public static readonly IO<Unit> emptyLine =
        lift(System.Console.WriteLine);

    public static IO<Unit> setForegroundColour(ConsoleColor colour) =>
        lift(() => System.Console.ForegroundColor = colour).Map(_ => unit);

    public static IO<ConsoleColor> colour =>
        lift(() => System.Console.ForegroundColor);
    
    public static readonly IO<Unit> red =
        setForegroundColour(ConsoleColor.Red);

    public static readonly IO<Unit> green =
        setForegroundColour(ConsoleColor.Green);

    public static readonly IO<Unit> yellow =
        setForegroundColour(ConsoleColor.Yellow);

    public static readonly IO<Unit> cyan =
        setForegroundColour(ConsoleColor.Cyan);

    public static IO<Unit> writeLine(string line) =>
        lift(() => System.Console.WriteLine(line));

    public static IO<Unit> write(string text) =>
        lift(() => System.Console.Write(text));

    public static IO<string> readLine =>
        lift(() => System.Console.ReadLine() ?? "");

    public static IO<ConsoleKeyInfo> readKey =>
        IO.lift(System.Console.ReadKey) >> writeLine("");
}
