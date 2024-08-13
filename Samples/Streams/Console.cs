using LanguageExt;
using static LanguageExt.Prelude;

namespace Streams;

// Simple IO wrappers of Console 
public static class Console
{
    public static IO<Unit> writeLine(string line) =>
        lift(() => System.Console.WriteLine(line));

    public static IO<string> readLine =>
        lift(() => System.Console.ReadLine() ?? "");

    public static IO<ConsoleKeyInfo> readKey =>
        from k in lift(System.Console.ReadKey)
        from _ in writeLine("")
        select k;
}
