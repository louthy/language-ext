using LanguageExt;
using static Streams.Console;
using static LanguageExt.Prelude;

namespace Streams;

public static class Folding
{
    public static IO<Unit> run =>
        from x in example(1000).Iter().As()
        select unit;

    static StreamT<IO, int> naturals =>
        Range(0, int.MaxValue).AsStream<IO>();
    
    static StreamT<IO, Unit> example(int n) =>
        from v in naturals.FoldUntil(0, (s, x) => s + x, (s, _) => s % 10 == 0)
        from _ in writeLine(v.ToString())
        where false
        select unit;
}
