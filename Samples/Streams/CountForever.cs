#pragma warning disable LX_StreamT

using LanguageExt;
using LanguageExt.Pipes;
using LanguageExt.Pipes.Concurrent;
using static LanguageExt.Prelude;
using static Streams.Console;

namespace Streams;

public static class CountForever
{
    public static IO<Unit> run =>
        from f in fork(example.Iter()).As()
        from k in readKey
        from r in f.Cancel 
        select unit;

    static SourceT<IO, long> naturals =>
        SourceT.lift<IO, long>(Range(0, long.MaxValue));
    
    static SourceT<IO, Unit> example =>
        from v in naturals
        where v % 10000 == 0
        from _ in writeLine($"{v:N0}")
        where false
        select unit;
}
