using System;
using LanguageExt;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace TestBed.StateStuff;

public static class StateForkIO
{
    public static StateIO<int, Unit> forkTest =>
        from p1 in showState("parent")
        from f1 in fork(countTo10("fst"))
        from f2 in fork(countTo10("snd"))
        from _  in awaitAll(f1, f2)
        from p2 in showState("parent")
        select unit;

    static StateIO<int, Unit> countTo10(string branch) =>
        from _1 in addToState
        from st in showState(branch)
        from _2 in when(st < 10, countTo10(branch))
        select unit;

    static StateIO<int, Unit> addToState =>
        Stateful.modify<StateIO<int>, int>(x => x + 1).As();
    
    static StateIO<int, int> showState(string branch) =>
        from s in Stateful.get<StateIO<int>, int>().As()
        from _ in Console.writeLine($"{branch}: {s}")
        select s;
}

public static class Console
{
    public static IO<Unit> writeLine(object? obj) =>
        IO.lift(() => System.Console.WriteLine(obj));
}

/// <summary>
/// Wrapper around `StateT IO` so we can add a `MapIO` and `ToIO` 
/// </summary>
public record StateIO<S, A>(StateT<S, IO, A> runState) : K<StateIO<S>, A>
{
    public IO<(A Value, S State)> Run(S initialState) =>
        runState.Run(initialState).As();
    
    public StateIO<S, B> Map<B>(Func<A, B> f) =>
        this.Kind().Map(f).As();
    
    public StateIO<S, B> Bind<B>(Func<A, K<StateIO<S>, B>> f) =>
        this.Kind().Bind(f).As();
    
    public StateIO<S, C> SelectMany<B, C>(Func<A, K<StateIO<S>, B>> f, Func<A, B, C> g) =>
        Bind(x => f(x).Map(y => g(x, y)));
    
    public StateIO<S, C> SelectMany<B, C>(Func<A, IO<B>> f, Func<A, B, C> g) =>
        SelectMany(x => MonadIO.liftIO<StateIO<S>, B>(f(x)), g);    
}

/// <summary>
/// StateIO extensions
/// </summary>
public static class StateIOExtensions
{
    public static StateIO<S, A> As<S, A>(this K<StateIO<S>, A> ma) =>
        (StateIO<S, A>)ma;
    
    public static IO<(A Value, S State)> Run<S, A>(this K<StateIO<S>, A> ma, S initialState) =>
        ma.As().Run(initialState);    
}

/// <summary>
/// StateIO trait implementation
/// </summary>
public class StateIO<S> : 
    Deriving.Monad<StateIO<S>, StateT<S, IO>>,
    Deriving.Choice<StateIO<S>, StateT<S, IO>>,
    Deriving.Stateful<StateIO<S>, StateT<S, IO>, S>,
    MonadUnliftIO<StateIO<S>>
{
    static K<StateT<S, IO>, A> Natural<StateIO<S>, StateT<S, IO>>.Transform<A>(K<StateIO<S>, A> fa) => 
        fa.As().runState;

    static K<StateIO<S>, A> CoNatural<StateIO<S>, StateT<S, IO>>.CoTransform<A>(K<StateT<S, IO>, A> fa) => 
        new StateIO<S, A>(fa.As());

    static K<StateIO<S>, B> Maybe.MonadUnliftIO<StateIO<S>>.MapIO<A, B>(K<StateIO<S>, A> ma, Func<IO<A>, IO<B>> f) =>
        from s in Stateful.get<StateIO<S>, S>()
        let a = Atom(s)
        from r in CoNatural.transform<StateIO<S>, StateT<S, IO>, B>(
            StateT.lift<S, IO, B>(
                ma.As().runState.Run(s).Map(p =>
                                            {
                                                a.Swap(_ => p.State);
                                                return p.Value;
                                            }).MapIO(f)))
        from _ in Stateful.put<StateIO<S>, S>(a.Value)
        select r;

    static K<StateIO<S>, IO<A>> Maybe.MonadUnliftIO<StateIO<S>>.ToIO<A>(K<StateIO<S>, A> ma) => 
        from s in Stateful.get<StateIO<S>, S>()
        let a = Atom(s)
        from r in CoNatural.transform<StateIO<S>, StateT<S, IO>, IO<A>>(
            StateT.lift<S, IO, IO<A>>(
                ma.As().runState.Run(s).Map(p =>
                                            {
                                                a.Swap(_ => p.State);
                                                return p.Value;
                                            }).ToIO()))
        from _ in Stateful.put<StateIO<S>, S>(a.Value)
        select r;

    static K<StateIO<S>, A> Maybe.MonadIO<StateIO<S>>.LiftIO<A>(IO<A> ma) => 
        new StateIO<S, A>(StateT.lift<S, IO, A>(ma));
}
