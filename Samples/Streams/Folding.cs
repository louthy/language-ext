#pragma warning disable LX_StreamT

using LanguageExt;
using LanguageExt.Pipes;
using static Streams.Console;
using static LanguageExt.Prelude;

namespace Streams;

public static class Folding
{
    public static IO<Unit> run =>
        example(100).Iter().As();

    static StreamT<IO, int> naturals(int n) =>
        Range(0, n).AsStream<IO, int>();
    
    static StreamT<IO, Unit> example(int n) =>
        from v in naturals(n).FoldUntil(0, (s, x) => s + x, (_, x) => x % 10 == 0)
        from _ in writeLine(v.ToString())
        where false
        select unit;
}
