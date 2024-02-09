using System;
using LanguageExt.HKT;

namespace LanguageExt;

/// <summary>
/// Reader ask
/// </summary>
/// <remarks>
/// This is a convenience type that is created by the Prelude `ask` function.  It avoids
/// the need for lots of generic parameters when used in `ReaderT` and `Reader` based
/// monads.
/// </remarks>
/// <param name="F">Mapping from the environment</param>
/// <typeparam name="Env">Reader environment type</typeparam>
/// <typeparam name="A">Type to map to </typeparam>
public readonly record struct Ask<Env, A>(Func<Env, A> F)
{
    /// <summary>
    /// Convert ot a `ReaderT`
    /// </summary>
    public ReaderT<Env, M, A> ToReaderT<M>() where M : Monad<M> =>
        ReaderT<Env, M, A>.Asks(F);
    
    /// <summary>
    /// Convert ot a `Reader`
    /// </summary>
    public Reader<Env, A> ToReader() =>
        Reader<Env, A>.Asks(F).As();

    /// <summary>
    /// Monadic bind with `ReaderT`
    /// </summary>
    public ReaderT<Env, M, C> SelectMany<M, B, C>(Func<A, ReaderT<Env, M, B>> bind, Func<A, B, C> project)
        where M : Monad<M> =>
        ToReaderT<M>().SelectMany(bind, project);

    /// <summary>
    /// Monadic bind with `Reader`
    /// </summary>
    public Reader<Env, C> SelectMany<B, C>(Func<A, Reader<Env, B>> bind, Func<A, B, C> project) =>
        ToReader().SelectMany(bind, project).As();
}
