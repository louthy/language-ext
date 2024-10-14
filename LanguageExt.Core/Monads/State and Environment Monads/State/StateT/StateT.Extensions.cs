using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// State monad extensions
/// </summary>
public static partial class StateTExtensions
{
    public static StateT<Env, M, A> As<Env, M, A>(this K<StateT<Env, M>, A> ma)
        where M : Monad<M>, SemigroupK<M> =>
        (StateT<Env, M, A>)ma;
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static StateT<Env, M, A> Flatten<Env, M, A>(this StateT<Env, M, StateT<Env, M, A>> mma)
        where M : Monad<M>, SemigroupK<M> =>
        mma.Bind(x => x);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public static StateT<Env, M, C> SelectMany<Env, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, K<StateT<Env, M>, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M>, SemigroupK<M> =>
        StateT<Env, M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public static StateT<Env, M, C> SelectMany<Env, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, StateT<Env, M, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M>, SemigroupK<M> =>
        StateT<Env, M, A>.Lift(ma).SelectMany(bind, project);
}
