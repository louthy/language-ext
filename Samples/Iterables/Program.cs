using LanguageExt;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

Examples.runSquares();

public class Examples
{
    public static void runSquares()
    {
        var mx = sumOfSquares(5).Run();

        var mr1 = mx.Run(EnvIO.New())
                   .IgnoreF();

        var mr2 = mx.Run(EnvIO.New())
                    .IgnoreF();
        
    }

    static IterableT<M, long> squares<M>(int n)
        where M : Monad<M> =>
        IterableT<M>.lift(Range(0, (long)n).Select(v => v * v).Where(v => v <= n));

    public static IterableT<IO, (long X, long Y)> sumOfSquares(int n) =>
        from x in squares<IO>(n)
        from y in squares<IO>(n)
        from _1 in Console.writeLine((x, y).ToString())
        where x + y == n
        from _2 in Console.writeLine("Sum of squares!")
        select (x, y);
}

// Simple IO wrappers of Console 
public static class Console
{
    public static IO<Unit> writeLine(string line) =>
        lift(() => System.Console.WriteLine(line));

    public static IO<ConsoleKeyInfo> readKey =>
        lift(System.Console.ReadKey);
}
