using System;
using LanguageExt.Traits;

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
    /// Convert to a `Reader`
    /// </summary>
    public Reader<Env, A> ToReader() =>
        Reader<Env, A>.Asks(F);
    
    /// <summary>
    /// Convert to a `ReaderT`
    /// </summary>
    public ReaderT<Env, M, A> ToReaderT<M>()
        where M : Monad<M>, SemigroupK<M> =>
        ReaderT<Env, M, A>.Asks(F);
    
    /// <summary>
    /// Monadic bind with any `Reader`
    /// </summary>
    public K<M, C> SelectMany<M, B, C>(Func<A, K<M, B>> bind, Func<A, B, C> project)
        where M : Monad<M>, SemigroupK<M>, Readable<M, Env> =>
        M.Bind(M.Asks(F), x => M.Map(y => project(x, y), bind(x)));
    
    /// <summary>
    /// Monadic bind with `ReaderT`
    /// </summary>
    public ReaderT<Env, M, C> SelectMany<M, B, C>(Func<A, ReaderT<Env, M, B>> bind, Func<A, B, C> project)
        where M : Monad<M>, SemigroupK<M> =>
        ToReaderT<M>().SelectMany(bind, project);

    /// <summary>
    /// Monadic bind with `Reader`
    /// </summary>
    public Reader<Env, C> SelectMany<B, C>(Func<A, Reader<Env, B>> bind, Func<A, B, C> project) =>
         ToReader().SelectMany(bind, project).As();
}

public static class AskExtensions
{
    /// <summary>
    /// Monadic bind with any `Reader`
    /// </summary>
    public static K<M, C> SelectMany<Env, M, A, B, C>(
        this K<M, A> ma,
        Func<A, Ask<Env, B>> bind,
        Func<A, B, C> project)
        where M : Monad<M>, SemigroupK<M>, Readable<M, Env> =>
        M.Bind(ma, a => M.Map(b => project(a, b), M.Asks(bind(a).F)));
}
