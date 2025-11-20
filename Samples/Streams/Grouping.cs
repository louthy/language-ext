using LanguageExt;
using static Streams.Console;
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
        runTestIO("test 1", test1) >>
        runTestIO("test 2", test2);

    static IO<Unit> runTestIO(string name, SourceT<IO, int> test) =>
        runTest(name, test1).Iter().As() >> emptyLine;

    static SourceT<IO, Unit> runTest(string name, SourceT<IO, int> test) =>
        from t in writeLine($"{name}")
        from r in test
        from _ in write($"{r} ")
        where false
        select unit;

    static SourceT<IO, int> test1 =
        from r in atomIO(0)
        from n in next(r) + next(r) >> (next(r) + next(r) >> next(r) + next(r))
        select n;

    static SourceT<IO, int> test2 =
        from r in atomIO(0)
        from n in (next(r) + next(r) >> next(r) + next(r)) >> next(r) + next(r)
        select n;

    static SourceT<IO, int> next(Atom<int> atom) =>
        from x in valueIO(atom)
        from _ in writeIO(atom, x + 1)
        select x;
}
