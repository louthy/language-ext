using LanguageExt;
using static LanguageExt.Prelude;

namespace Iterables;

public static class CountForeverAsync
{
    public static IO<Unit> run =>
        from f in forkIO(example.Iter()).As()
        from k in Console.readKey
        from r in f.Cancel 
        select unit;

    static IterableT<IO, long> naturals =>
        IterableT<IO>.lift(naturalsEnum());
    
    static IterableT<IO, Unit> example =>
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
