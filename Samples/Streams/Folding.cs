#pragma warning disable LX_StreamT

using LanguageExt;
using static Streams.Console;
using static LanguageExt.Prelude;

namespace Streams;

public static class Folding
{
    public static IO<Unit> run =>
        from x in example(100).Iter().As()
        select unit;

    static StreamT<IO, int> naturals(int n) =>
        Range(0, n).AsStream<IO>();
    
    static StreamT<IO, Unit> example(int n) =>
        from v in naturals(n).FoldUntil(0, (s, x) => s + x, (_, x) => x % 10 == 0)
        from _ in writeLine(v.ToString())
        where false
        select unit;
}
