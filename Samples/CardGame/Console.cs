using LanguageExt;
using static LanguageExt.Prelude;

namespace CardGame;

/// <summary>
/// Simple IO wrappers of Console 
/// </summary>
public static class Console
{
    public static IO<Unit> emptyLine =>
        lift(System.Console.WriteLine);

    public static IO<Unit> writeLine(string line) =>
        lift(() => System.Console.WriteLine(line));

    public static IO<string> readLine =>
        lift(System.Console.ReadLine).Map(ln => ln ?? "");

    public static IO<ConsoleKeyInfo> readKey =>
        from k in lift(System.Console.ReadKey)
        from _ in emptyLine
        select k;
}
