#pragma warning disable LX_StreamT

using LanguageExt;
using LanguageExt.Pipes;
using LanguageExt.Pipes.Concurrent;
using static Streams.Console;
using static LanguageExt.Prelude;

namespace Streams;

public static class Folding
{
    public static IO<Unit> run =>
        example(100).Iter().As();

    static SourceT<IO, int> naturals(int n) =>
        Range(0, n).AsSourceT<IO, int>();
    
    static SourceT<IO, Unit> example(int n) =>
        from v in naturals(n).FoldUntil((s, x) => s + x, (_, x) => x % 10 == 0, 0)
        from _ in writeLine(v)
        where false
        select unit;
}
