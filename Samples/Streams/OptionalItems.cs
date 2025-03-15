#pragma warning disable LX_StreamT

using LanguageExt;
using LanguageExt.Pipes;
using LanguageExt.Pipes.Concurrent;
using static LanguageExt.Prelude;

namespace Streams;

public static class OptionalItems
{
    public static IO<Unit> run =>
        from _0 in Console.writeLine("starting")
        from _1 in example(100).Iter()
        from _2 in Console.writeLine("done")
        select unit;

    static SourceT<OptionT<IO>, Unit> example1(int n) =>
        from x in SourceT.liftM(asyncStream(n))
        from _ in Console.write($"{x} ")
        where false
        select unit;    

    static SourceT<IO, Unit> example(int n) =>
        from x in asyncStream(n).SomeSource()
        from _ in Console.write($"{x} ")
        where false
        select unit;

    static bool isAllowed(int x) =>
        (x & 1) == 1;
    
    static async IAsyncEnumerable<OptionT<IO, int>> asyncStream(int n) 
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
