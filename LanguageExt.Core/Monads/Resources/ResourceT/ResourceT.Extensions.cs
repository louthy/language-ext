using System;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Reader monad extensions
/// </summary>
public static partial class ResourceT
{
    public static ResourceT<M, A> As<M, A>(this K<ResourceT<M>, A> ma)
        where M : Monad<M>, Alternative<M> =>
        (ResourceT<M, A>)ma;

    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static ResourceT<M, A> Flatten<M, A>(this ResourceT<M, ResourceT<M, A>> mma)
        where M : Monad<M>, Alternative<M> =>
        mma.Bind(x => x);

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
        where M : Monad<M>, Alternative<M> =>
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
        where M : Monad<M>, Alternative<M> =>
        ResourceT<M, A>.Lift(ma).SelectMany(bind, project);
}
