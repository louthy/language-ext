using LanguageExt;
using static LanguageExt.Prelude;

namespace CardGame;

/// <summary>
/// Simple IO wrappers of Console 
/// </summary>
public static class Console
{
    public static IO<Unit> writeLine() =>
        lift(System.Console.WriteLine);

    public static IO<Unit> writeLine(string line) =>
        lift(() => System.Console.WriteLine(line));

    public static IO<ConsoleKeyInfo> readKey =>
        lift(System.Console.ReadKey);
}
