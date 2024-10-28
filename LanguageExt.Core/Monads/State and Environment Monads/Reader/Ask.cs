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
    /// Use a `Readable` trait to convert to an `M<A>`
    /// </summary>
    public K<M, A> ToReadable<M>()
        where M : Readable<M, Env> =>
        Readable.asks<M, Env, A>(F);

    /// <summary>
    /// Convert to a `Reader`
    /// </summary>
    public Reader<Env, A> ToReader() =>
        ToReadable<Reader<Env>>().As();
    
    /// <summary>
    /// Convert to a `ReaderT`
    /// </summary>
    public ReaderT<Env, M, A> ToReaderT<M>()
        where M : Monad<M>, Choice<M> =>
        ToReadable<ReaderT<Env, M>>().As();
    
    /// <summary>
    /// Convert to a `RWS`
    /// </summary>
    //public RWS<Env, W, S, A> ToRWS() =>
    //    ToReadable<RWS<Env, W, S>>().As();
    
    /// <summary>
    /// Convert to a `RWST`
    /// </summary>
    public RWST<Env, W, S, M, A> ToRWST<W, S, M>()
        where W : Monoid<W>
        where M : Monad<M>, Choice<M> =>
        ToReadable<RWST<Env, W, S, M>>().As();
    
    /// <summary>
    /// Monadic bind with any `Reader`
    /// </summary>
    public K<M, C> SelectMany<M, B, C>(Func<A, K<M, B>> bind, Func<A, B, C> project)
        where M : Monad<M>, Choice<M>, Readable<M, Env> =>
        M.Bind(M.Asks(F), x => M.Map(y => project(x, y), bind(x)));
    
    /// <summary>
    /// Monadic bind with `ReaderT`
    /// </summary>
    public ReaderT<Env, M, C> SelectMany<M, B, C>(Func<A, ReaderT<Env, M, B>> bind, Func<A, B, C> project)
        where M : Monad<M>, Choice<M> =>
        ToReaderT<M>().SelectMany(bind, project);
    
    /// <summary>
    /// Monadic bind with `ReaderT`
    /// </summary>
    public ReaderT<Env, M, C> SelectMany<M, B, C>(Func<A, K<ReaderT<Env, M>, B>> bind, Func<A, B, C> project)
        where M : Monad<M>, Choice<M> =>
        ToReaderT<M>().SelectMany(bind, project);

    /// <summary>
    /// Monadic bind with `Reader`
    /// </summary>
    public Reader<Env, C> SelectMany<B, C>(Func<A, Reader<Env, B>> bind, Func<A, B, C> project) =>
         ToReader().SelectMany(bind, project).As();

    /// <summary>
    /// Monadic bind with `Reader`
    /// </summary>
    public Reader<Env, C> SelectMany<B, C>(Func<A, K<Reader<Env>, B>> bind, Func<A, B, C> project) =>
        ToReader().SelectMany(bind, project).As();

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public RWST<Env, W, S, M, C> SelectMany<W, S, M, B, C>(
        Func<A, RWST<Env, W, S, M, B>> bind, 
        Func<A, B, C> project)
        where W : Monoid<W>
        where M : Monad<M>, Choice<M> =>
        RWST<Env, W, S, M, A>.Asks(F).SelectMany(bind, project);    

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public RWST<Env, W, S, M, C> SelectMany<W, S, M, B, C>(
        Func<A, K<RWST<Env, W, S, M>, B>> bind, 
        Func<A, B, C> project)
        where W : Monoid<W>
        where M : Monad<M>, Choice<M> =>
        RWST<Env, W, S, M, A>.Asks(F).SelectMany(bind, project);      
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
        where M : Monad<M>, Choice<M>, Readable<M, Env> =>
        M.Bind(ma, a => M.Map(b => project(a, b), M.Asks(bind(a).F)));
}
