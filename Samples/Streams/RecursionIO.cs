using LanguageExt;
using LanguageExt.Common;
using static Streams.Console;
using static LanguageExt.Prelude;

namespace Streams;

public static class RecursionIO
{
    public static IO<Unit> run =>
        from _ in writeLine("Enter a number to count from")
        from s in readLine
        from n in parseInt<IO>(s) | IO.fail<int>("expected a number!")
        from r in recurse(n) >>
                  emptyLine
        select r;

    public static IO<Unit> recurse(int n) =>
        from _ in write($"{n} ")
        from r in when(n > 0, recurse(n - 1))
        select r;
}
