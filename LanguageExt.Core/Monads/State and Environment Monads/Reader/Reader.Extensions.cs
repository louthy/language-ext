using System;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Reader monad extensions
/// </summary>
public static class ReaderExt
{
     public static Reader<Env, A> As<Env, A>(this K<ReaderT<Env, Identity>, A> ma) =>
        (Reader<Env, A>)ma;
    
    public static ReaderT<Env, M, A> As<Env, M, A>(this K<ReaderT<Env, M>, A> ma)
        where M : Monad<M> =>
        (ReaderT<Env, M, A>)ma;
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Reader<Env, A> Flatten<Env, A>(this Reader<Env, Reader<Env, A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Impure iteration of the bound value in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public static Reader<Env, A> Do<Env, A>(this Reader<Env, A> ma, Action<A> f) =>
        ma.Map(a => { f(a); return a; });

    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static ReaderT<Env, M, A> Flatten<Env, M, A>(this ReaderT<Env, M, ReaderT<Env, M, A>> mma)
        where M : Monad<M> =>
        mma.Bind(identity);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public static ReaderT<Env, M, C> SelectMany<Env, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, K<ReaderT<Env, M>, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M> =>
        ReaderT<Env, M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public static ReaderT<Env, M, C> SelectMany<Env, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, ReaderT<Env, M, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M> =>
        ReaderT<Env, M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public static ReaderT<Env, M, C> SelectMany<Env, M, A, B, C>(
        this ReaderT<Env, M, A> ma, 
        Func<A, IO<B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M> =>
        ma.SelectMany(x => M.LiftIO(bind(x)), project);
}
