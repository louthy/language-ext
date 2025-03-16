#pragma warning disable LX_StreamT

using LanguageExt;
using LanguageExt.Pipes;
using LanguageExt.Pipes.Concurrent;
using static LanguageExt.Prelude;
using static Streams.Console;

namespace Streams;

public static class CountForeverAsync
{
    public static IO<Unit> run =>
        from f in fork(example.Iter()).As()
        from k in readKey
        from r in f.Cancel 
        select unit;

    static SourceT<IO, long> naturals =>
        SourceT.lift<IO, long>(naturalsEnum());
    
    static SourceT<IO, Unit> example =>
        from v in naturals
        from _ in writeLine($"{v:N0}")
        where false
        select unit;

    static async IAsyncEnumerable<long> naturalsEnum()
    {
        for (var i = 0L; i < long.MaxValue; i++)
        {
            yield return i;
            await Task.Delay(1000);
        }
    }
}
