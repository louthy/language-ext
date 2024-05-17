using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// State put
/// </summary>
/// <remarks>
/// This is a convenience type that is created by the Prelude `put` function.  It avoids
/// the need for lots of generic parameters when used in `StateT` and `State` based
/// monads.
/// </remarks>
/// <param name="F">Mapping from the environment</param>
/// <typeparam name="S">State type</typeparam>
public readonly record struct Put<S>(S Value)
{
    /// <summary>
    /// Convert ot a `StateT`
    /// </summary>
    public StateT<S, M, Unit> ToStateT<M>()
        where M : Monad<M>, SemiAlternative<M> =>
        StateT<S, M, Unit>.Put(Value);
    
    /// <summary>
    /// Convert ot a `StateT`
    /// </summary>
    public State<S, Unit> ToState() =>
        State<S, Unit>.Put(Value);
    
    /// <summary>
    /// Convert ot a `State`
    /// </summary>
    //public State<S, Unit> ToState() =>
    //    State<S, Unit>.Put(Value).As();

    /// <summary>
    /// Monadic bind with `StateT`
    /// </summary>
    public StateT<S, M, C> SelectMany<M, B, C>(Func<Unit, StateT<S, M, B>> bind, Func<Unit, B, C> project)
        where M : Monad<M>, SemiAlternative<M> =>
        ToStateT<M>().SelectMany(bind, project);

    /// <summary>
    /// Monadic bind with `State`
    /// </summary>
    public State<S, C> SelectMany<B, C>(Func<Unit, State<S, B>> bind, Func<Unit, B, C> project) =>
        ToState().SelectMany(bind, project);
}

/// <summary>
/// State modify
/// </summary>
/// <remarks>
/// This is a convenience type that is created by the Prelude `modify` function.  It avoids
/// the need for lots of generic parameters when used in `StateT` and `State` based
/// monads.
/// </remarks>
/// <param name="f">Mapping from the environment</param>
/// <typeparam name="S">State type</typeparam>
public readonly record struct Modify<S>(Func<S, S> f)
{
    /// <summary>
    /// Convert ot a `StateT`
    /// </summary>
    public StateT<S, M, Unit> ToStateT<M>()
        where M : Monad<M>, SemiAlternative<M> =>
        StateT<S, M, Unit>.Modify(f);
    
    /// <summary>
    /// Convert ot a `State`
    /// </summary>
    public State<S, Unit> ToState() =>
        State<S, Unit>.Modify(f);

    /// <summary>
    /// Monadic bind with `StateT`
    /// </summary>
    public StateT<S, M, C> SelectMany<M, B, C>(Func<Unit, StateT<S, M, B>> bind, Func<Unit, B, C> project)
        where M : Monad<M>, SemiAlternative<M> =>
        ToStateT<M>().SelectMany(bind, project);

    /// <summary>
    /// Monadic bind with `State`
    /// </summary>
    public State<S, C> SelectMany<B, C>(Func<Unit, State<S, B>> bind, Func<Unit, B, C> project) =>
        ToState().SelectMany(bind, project);
}


/// <summary>
/// State modify
/// </summary>
/// <remarks>
/// This is a convenience type that is created by the Prelude `modify` function.  It avoids
/// the need for lots of generic parameters when used in `StateT` and `State` based
/// monads.
/// </remarks>
/// <param name="f">Mapping from the environment</param>
/// <typeparam name="S">State type</typeparam>
public readonly record struct Gets<S, A>(Func<S, A> f)
{
    /// <summary>
    /// Convert ot a `StateT`
    /// </summary>
    public StateT<S, M, A> ToStateT<M>()
        where M : Monad<M>, SemiAlternative<M> =>
        StateT<S, M, A>.Gets(f);
    
    /// <summary>
    /// Convert ot a `State`
    /// </summary>
    public State<S, A> ToState() =>
        State<S, A>.Gets(f);

    /// <summary>
    /// Monadic bind with `StateT`
    /// </summary>
    public StateT<S, M, C> SelectMany<M, B, C>(Func<A, StateT<S, M, B>> bind, Func<A, B, C> project)
        where M : Monad<M>, SemiAlternative<M> =>
        ToStateT<M>().SelectMany(bind, project);

    /// <summary>
    /// Monadic bind with `State`
    /// </summary>
    public State<S, C> SelectMany<B, C>(Func<A, State<S, B>> bind, Func<A, B, C> project) =>
        ToState().SelectMany(bind, project);
}
