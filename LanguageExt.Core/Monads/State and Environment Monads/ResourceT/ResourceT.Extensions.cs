using System;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.HKT;

namespace LanguageExt;

/// <summary>
/// Reader monad extensions
/// </summary>
public static class ResourceT
{
    public static ResourceT<M, A> As<M, A>(this K<ResourceT<M>, A> ma)
        where M : MonadIO<M> =>
        (ResourceT<M, A>)ma;

    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static ResourceT<M, A> Flatten<M, A>(this ResourceT<M, ResourceT<M, A>> mma)
        where M : MonadIO<M> =>
        mma.Bind(identity);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public static ResourceT<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, 
        Func<A, K<ResourceT<M>, B>> bind, 
        Func<A, B, C> project)
        where M : MonadIO<M> =>
        ResourceT<M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public static ResourceT<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, 
        Func<A, ResourceT<M, B>> bind, 
        Func<A, B, C> project)
        where M : MonadIO<M> =>
        ResourceT<M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public static ResourceT<M, C> SelectMany<M, A, B, C>(
        this ResourceT<M, A> ma, 
        Func<A, IO<B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M>, MonadIO<M> =>
        ma.SelectMany(x => M.LiftIO(bind(x)), project);
}
