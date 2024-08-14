using LanguageExt;
using static LanguageExt.Prelude;

namespace Streams;

/// <summary>
/// Based on 'Grouping effects' from 'ListT done right'
///
/// https://wiki.haskell.org/ListT_done_right
/// </summary>
public static class Grouping
{
    public static IO<Unit> run =>
        from _1 in runTestIO("test 1", test1)
        from _2 in runTestIO("test 2", test2)
        select unit;

    static IO<Unit> runTestIO(string name, StreamT<IO, int> test) =>
        from _1 in runTest(name, test1).Iter().As()
        from _2 in Console.writeLine("\n")
        select unit;

    static StreamT<IO, Unit> runTest(string name, StreamT<IO, int> test) =>
        from t in Console.writeLine($"{name}")
        from r in test
        from _ in Console.write($"{r} ")
        where false
        select unit;

    static StreamT<IO, int> test1 =
        from r in atomIO(0)
        from n in next(r) + next(r) >> (next(r) + next(r) >> next(r) + next(r))
        select n;

    static StreamT<IO, int> test2 =
        from r in atomIO(0)
        from n in (next(r) + next(r) >> next(r) + next(r)) >> next(r) + next(r)
        select n;

    static StreamT<IO, int> next(Atom<int> atom) =>
        from x in valueIO(atom)
        from _ in writeIO(atom, x + 1)
        select x;

    static StreamT<IO, int> next1(Atom<int> atom) =>
        atom.SwapIO(x => x + 1);
    
}
