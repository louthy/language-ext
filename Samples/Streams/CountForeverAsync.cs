using LanguageExt;
using static LanguageExt.Prelude;

namespace Streams;

public static class CountForeverAsync
{
    public static IO<Unit> run =>
        from f in forkIO(example.Iter())
        from k in Console.readKey
        from r in f.Cancel 
        select unit;

    static StreamT<IO, long> naturals =>
        StreamT<IO>.lift(naturalsEnum());
    
    static StreamT<IO, Unit> example =>
        from v in naturals
        from _ in Console.writeLine($"{v:N0}")
        where false
        select unit;

    static async IAsyncEnumerable<long> naturalsEnum()
    {
        for (var i = 0L; i < long.MaxValue; i++)
        {
            yield return i;
            await Task.Delay(100);
        }
    }
}
