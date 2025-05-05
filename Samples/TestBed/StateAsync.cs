using LanguageExt;
using static LanguageExt.Prelude;

namespace TestBed.StateStuff;

public static class StateForkIO
{
    public static StateT<int, IO, Unit> forkTest =>
        from p1 in showState("parent")
        from f1 in fork(countTo10("fst"))
        from f2 in fork(countTo10("snd"))
        from _  in awaitAll(f1, f2)
        from p2 in showState("parent")
        select unit;

    static StateT<int, IO, Unit> countTo10(string branch) =>
        from _1 in addToState
        from st in showState(branch)
        from _2 in when(st < 10, countTo10(branch))
        select unit;

    static StateT<int, IO, Unit> addToState =>
        StateT.modify<IO, int>(x => x + 1);
    
    static StateT<int, IO, int> showState(string branch) =>
        from s in StateT.get<IO, int>()
        from _ in Console.writeLine($"{branch}: {s}")
        select s;
}

public static class Console
{
    public static IO<Unit> writeLine(object? obj) =>
        IO.lift(() => System.Console.WriteLine(obj));
}
