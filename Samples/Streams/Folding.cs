using LanguageExt;
using LanguageExt.Pipes.Concurrent;
using static Streams.Console;
using static LanguageExt.Prelude;

namespace Streams;

public static class Folding
{
    public static IO<Unit> run =>
        example(1000).Iter().As();

    static SourceT<IO, int> naturals(int n) =>
        Range(0, n).AsSourceT<IO, int>();
    
    static SourceT<IO, Unit> example(int n) =>
        from v in naturals(n).FoldUntil((s, x) => s + x, (s, _) => s % 4 == 0, 0)
        where v % 10 == 0
        from _ in writeLine(v)
        select unit;
}
