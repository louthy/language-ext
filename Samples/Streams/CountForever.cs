#pragma warning disable LX_StreamT

using LanguageExt;
using LanguageExt.Pipes;
using static LanguageExt.Prelude;
using static Streams.Console;

namespace Streams;

public static class CountForever
{
    public static IO<Unit> run =>
        from f in fork(example.RunAsync())
        from k in readKey
        from r in f.Cancel 
        select unit;

    static StreamT<IO, long> naturals =>
        StreamT.lift<IO, long>(Range(0, long.MaxValue));
    
    static StreamT<IO, Unit> example =>
        from v in naturals
        where v % 10000 == 0
        from _ in writeLine($"{v:N0}")
        where false
        select unit;
}
