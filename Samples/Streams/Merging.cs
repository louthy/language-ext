#pragma warning disable LX_StreamT

using LanguageExt;
using LanguageExt.Pipes;
using static Streams.Console;
using static LanguageExt.Prelude;

namespace Streams;

public static class Merging
{
    public static IO<Unit> run =>
        example(20).Iter().As() >>
        emptyLine;

    static StreamT<IO, Unit> example(int n) =>
        from v in evens(n) + odds(n)
        where false
        select unit;
    
    static StreamT<IO, int> evens(int n) =>
        from x in Range(0, n).AsStream<IO, int>()
        where isEven(x)
        from _ in magenta >> write($"{x} ")
        select x;

    static StreamT<IO, int> odds(int n) =>
        from x in Range(0, n).AsStream<IO, int>()
        where isOdd(x)
        from _ in yellow >> write($"{x} ")
        select x;
    
    static bool isOdd(int n) =>
        (n & 1) == 1;

    static bool isEven(int n) =>
        !isOdd(n);
}
