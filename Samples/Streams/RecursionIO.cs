using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace Streams;

public static class RecursionIO
{
    public static IO<Unit> run =>
        from _ in Console.writeLine("Enter a number to count from")
        from s in Console.readLine
        from n in parseInt(s).Match(Some: IO.pure, None: IO.fail<int>(Errors.Cancelled))
        from r in recurse(n) >>
                  Console.emptyLine
        select r;

    public static IO<Unit> recurse(int n) =>
        from _ in Console.write($"{n} ")
        from r in when(n > 0, recurse(n - 1))
        select r;
}
