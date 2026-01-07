using static LanguageExt.Prelude;
using LanguageExt;

namespace TestBed;

public static class FoldStepTests
{
    public static void Run()
    {
        var xs = Range(1, 1000000000).AsIterable();
        var rs = +xs.FoldM((_, x) => x % 1000000 == 0 ? writeLine(x) : IO.pure(unit), unit);
        rs.Run();
    }

    static IO<Unit> writeLine<A>(A value) =>
        IO.lift(() => System.Console.WriteLine(value));
}
