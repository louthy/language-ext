using LanguageExt;
using static LanguageExt.Prelude;

namespace Streams;

// Simple IO wrappers of Console 
public static class Console
{
    public static readonly IO<Unit> emptyLine =
        lift(System.Console.WriteLine);

    public static IO<Unit> writeLine(string line) =>
        lift(() => System.Console.WriteLine(line));

    public static IO<Unit> write(string text) =>
        lift(() => System.Console.Write(text));

    public static IO<string> readLine =>
        lift(() => System.Console.ReadLine() ?? "");

    public static IO<ConsoleKeyInfo> readKey =>
        IO.lift(System.Console.ReadKey) >> writeLine("");
}
