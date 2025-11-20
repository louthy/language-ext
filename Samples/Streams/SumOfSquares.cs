using LanguageExt;
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
        from n in parseLong<IO>(s) | IO.fail<long>("expected a number!")
        from x in example(n).Iter()
        select unit;

    static SourceT<M, long> squares<M>(long n)
        where M : MonadIO<M>, Alternative<M> =>
        SourceT.lift<M, long>(Range(0, n).Select(v => v * v).Where(v => v <= n));

    static SourceT<IO, (long X, long Y)> example(long n) =>
        from x in squares<IO>(n)
        from y in squares<IO>(n)
        where x + y == n
        from _ in writeLine($"Sum of squares! {(x, y)}")
        where false
        select (x, y);
}
