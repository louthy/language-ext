using System;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Option monad extensions
/// </summary>
public static class OptionTExt
{
    public static OptionT<M, A> As<M, A>(this K<OptionT<M>, A> ma)
        where M : Monad<M> =>
        (OptionT<M, A>)ma;
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static OptionT<M, A> Flatten<M, A>(this OptionT<M, OptionT<M, A>> mma)
        where M : Monad<M> =>
        mma.Bind(identity);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`OptionT`</returns>
    [Pure]
    public static OptionT<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, 
        Func<A, K<OptionT<M>, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M> =>
        OptionT<M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`OptionT`</returns>
    [Pure]
    public static OptionT<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, 
        Func<A, OptionT<M, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M> =>
        OptionT<M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Applicative apply
    /// </summary>
    [Pure]
    public static OptionT<M, B> Apply<M, A, B>(this OptionT<M, Func<A, B>> mf, OptionT<M, A> ma) 
        where M : Monad<M> => 
        mf.As().Bind(ma.As().Map);

    /// <summary>
    /// Applicative action
    /// </summary>
    [Pure]
    public static OptionT<M, B> Action<M, A, B>(this OptionT<M, A> ma, OptionT<M, B> mb)
        where M : Monad<M> => 
        ma.As().Bind(_ => mb);
}
