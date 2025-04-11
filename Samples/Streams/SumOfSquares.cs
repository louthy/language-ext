#pragma warning disable LX_StreamT

using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Pipes;
using LanguageExt.Pipes.Concurrent;
using LanguageExt.Traits;
using static Streams.Console;
using static LanguageExt.Prelude;

namespace Streams;

/// <summary>
/// Based on 'Sum of squares' from 'ListT done right'
///
/// https://wiki.haskell.org/ListT_done_right
/// </summary>
public static class SumOfSquares
{
    public static IO<Unit> run =>
        from _ in writeLine("Enter a number to find the sum of squares")
        from s in readLine
        from n in parseInt<IO>(s) | IO.fail<int>("expected a number!")
        from x in example(n).Iter()
        select unit;

    static SourceT<M, long> squares<M>(int n)
        where M : MonadIO<M>, Alternative<M> =>
        SourceT.lift<M, long>(Range(0, (long)n).Select(v => v * v).Where(v => v <= n));

    //static SourceT<IO, Unit> example(int n) =>
    //    squares<IO>(n)
    //       .Bind(x => writeLine($"{x}"));

    static SourceT<IO, (long X, long Y)> example(int n) =>
        from x in squares<IO>(n)
        from y in squares<IO>(n)
        from _1 in writeLine((x, y))
        where x + y == n
        from _2 in writeLine("Sum of squares!")
        where false
        select (x, y);
}
