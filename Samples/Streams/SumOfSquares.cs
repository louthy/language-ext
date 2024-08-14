using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Traits;
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
        from _ in Console.writeLine("Enter a number to find the sum of squares")
        from s in Console.readLine
        from n in parseInt(s).Match(Some: IO.pure, None: IO.fail<int>(Errors.Cancelled))
        from x in example(n).Iter().As()
        select unit;

    static StreamT<M, long> squares<M>(int n)
        where M : Monad<M> =>
        StreamT<M>.lift(Range(0, (long)n).Select(v => v * v).Where(v => v <= n));

    static StreamT<IO, (long X, long Y)> example(int n) =>
        from x in squares<IO>(n)
        from y in squares<IO>(n)
        from _1 in Console.writeLine((x, y).ToString())
        where x + y == n
        from _2 in Console.writeLine("Sum of squares!")
        select (x, y);
}
