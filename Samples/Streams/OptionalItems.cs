using LanguageExt;
using static LanguageExt.Prelude;

namespace Streams;

public static class OptionalItems
{
    public static IO<Unit> run =>
        from _1 in example(100).Iter().Run()
        from _2 in Console.writeLine("done")
        select unit;

    static StreamT<OptionT<IO>, Unit> example(int n) =>
        from x in StreamT.liftM(getOptionsAsync(n))
        from _ in Console.write($"{x} ")
        where true
        select unit;    

    static bool isAllowed(int x) =>
        x != 20;
    
    static async IAsyncEnumerable<OptionT<IO ,int>> getOptionsAsync(int n) 
    {
        foreach (var x in Range(1, n))
        {
            var option = isAllowed(x)
                             ? OptionT.lift(IO.pure(x))
                             : OptionT<IO, int>.None;

            var r = await Task.FromResult(option);
            yield return r;
        }
    }
}
