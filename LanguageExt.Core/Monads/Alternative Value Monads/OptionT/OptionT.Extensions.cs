using System;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.HKT;

namespace LanguageExt;

/// <summary>
/// Option monad extensions
/// </summary>
public static class OptionTExt
{
    public static OptionT<M, A> As<M, A>(this K<OptionT<M>, A> ma)
        where M : MonadIO<M> =>
        (OptionT<M, A>)ma;
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static OptionT<M, A> Flatten<M, A>(this OptionT<M, OptionT<M, A>> mma)
        where M : MonadIO<M> =>
        mma.Bind(identity);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`OptionT`</returns>
    public static OptionT<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, 
        Func<A, K<OptionT<M>, B>> bind, 
        Func<A, B, C> project)
        where M : MonadIO<M> =>
        OptionT<M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`OptionT`</returns>
    public static OptionT<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, 
        Func<A, OptionT<M, B>> bind, 
        Func<A, B, C> project)
        where M : MonadIO<M> =>
        OptionT<M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`OptionT`</returns>
    public static OptionT<M, C> SelectMany<M, A, B, C>(
        this OptionT<M, A> ma, 
        Func<A, IO<B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M>, MonadIO<M> =>
        ma.SelectMany(x => M.LiftIO(bind(x)), project);
}
