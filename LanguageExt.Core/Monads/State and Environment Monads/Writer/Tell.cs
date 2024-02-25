using System;
using LanguageExt.Traits;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// State put
/// </summary>
/// <remarks>
/// This is a convenience type that is created by the Prelude `put` function.  It avoids
/// the need for lots of generic parameters when used in `WriterT` and `State` based
/// monads.
/// </remarks>
/// <param name="F">Mapping from the environment</param>
/// <typeparam name="S">State type</typeparam>
public record Tell<W>(W Value) where W : Monoid<W>
{
    /// <summary>
    /// Convert ot a `WriterT`
    /// </summary>
    public WriterT<W, M, Unit> ToWriterT<M>()
        where M : Monad<M>, SemiAlternative<M> =>
        new (() => M.Pure((unit, Value)));
    
    /// <summary>
    /// Convert ot a `WriterT`
    /// </summary>
    public Writer<W, Unit> ToWriter() =>
        new (() => (unit, Value));
    
    /// <summary>
    /// Convert ot a `State`
    /// </summary>
    //public State<S, Unit> ToState() =>
    //    State<S, Unit>.Put(Value).As();

    /// <summary>
    /// Monadic bind with `WriterT`
    /// </summary>
    public WriterT<W, M, C> SelectMany<M, B, C>(Func<Unit, WriterT<W, M, B>> bind, Func<Unit, B, C> project)
        where M : Monad<M>, SemiAlternative<M> =>
        ToWriterT<M>().SelectMany(bind, project);

    /// <summary>
    /// Monadic bind with `State`
    /// </summary>
    public Writer<W, C> SelectMany<B, C>(Func<Unit, Writer<W, B>> bind, Func<Unit, B, C> project) =>
        ToWriter().SelectMany(bind, project);
}

/// <summary>
/// State modify
/// </summary>
/// <remarks>
/// This is a convenience type that is created by the Prelude `modify` function.  It avoids
/// the need for lots of generic parameters when used in `WriterT` and `State` based
/// monads.
/// </remarks>
/// <param name="f">Mapping from the environment</param>
/// <typeparam name="S">State type</typeparam>
public readonly record struct Modify<S>(Func<S, S> f)
{
    /// <summary>
    /// Convert ot a `WriterT`
    /// </summary>
    public WriterT<S, M, Unit> ToWriterT<M>()
        where M : Monad<M>, SemiAlternative<M> =>
        WriterT<S, M, Unit>.Modify(f);
    
    /// <summary>
    /// Convert ot a `State`
    /// </summary>
    public State<S, Unit> ToState() =>
        State<S, Unit>.Modify(f);

    /// <summary>
    /// Monadic bind with `WriterT`
    /// </summary>
    public WriterT<S, M, C> SelectMany<M, B, C>(Func<Unit, WriterT<S, M, B>> bind, Func<Unit, B, C> project)
        where M : Monad<M>, SemiAlternative<M> =>
        ToWriterT<M>().SelectMany(bind, project);

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
/// the need for lots of generic parameters when used in `WriterT` and `State` based
/// monads.
/// </remarks>
/// <param name="f">Mapping from the environment</param>
/// <typeparam name="S">State type</typeparam>
public readonly record struct Gets<S, A>(Func<S, A> f)
{
    /// <summary>
    /// Convert ot a `WriterT`
    /// </summary>
    public WriterT<S, M, A> ToWriterT<M>()
        where M : Monad<M>, SemiAlternative<M> =>
        WriterT<S, M, A>.Gets(f);
    
    /// <summary>
    /// Convert ot a `State`
    /// </summary>
    public State<S, A> ToState() =>
        State<S, A>.Gets(f);

    /// <summary>
    /// Monadic bind with `WriterT`
    /// </summary>
    public WriterT<S, M, C> SelectMany<M, B, C>(Func<A, WriterT<S, M, B>> bind, Func<A, B, C> project)
        where M : Monad<M>, SemiAlternative<M> =>
        ToWriterT<M>().SelectMany(bind, project);

    /// <summary>
    /// Monadic bind with `State`
    /// </summary>
    public State<S, C> SelectMany<B, C>(Func<A, State<S, B>> bind, Func<A, B, C> project) =>
        ToState().SelectMany(bind, project);
}
