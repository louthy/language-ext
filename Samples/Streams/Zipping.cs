#pragma warning disable LX_StreamT

using LanguageExt;
using LanguageExt.Pipes;
using static Streams.Console;
using static LanguageExt.Prelude;

namespace Streams;

public static class Zipping
{
    public static IO<Unit> run =>
        from x in example(10).RunAsync().As()
        select unit;

    static StreamT<IO, Unit> example(int n) =>
        from v in evens(n).Zip(odds(n))
        from _ in writeLine(v)
        where false
        select unit;

    static StreamT<IO, int> evens(int n) =>
        from x in Range(0, n).AsStream<IO, int>()
        where isEven(x)
        select x;

    static StreamT<IO, int> odds(int n) =>
        from x in Range(0, n).AsStream<IO, int>()
        where isOdd(x)
        select x;
    
    static bool isOdd(int n) =>
        (n & 1) == 1;

    static bool isEven(int n) =>
        !isOdd(n);
}
