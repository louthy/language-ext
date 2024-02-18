using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// State monad extensions
/// </summary>
public static class StateExt
{
     public static State<Env, A> As<Env, A>(this K<StateT<Env, Identity>, A> ma) =>
        (State<Env, A>)ma;
    
    public static StateT<Env, M, A> As<Env, M, A>(this K<StateT<Env, M>, A> ma)
        where M : Monad<M>, Alternative<M> =>
        (StateT<Env, M, A>)ma;
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static StateT<Env, M, A> Flatten<Env, M, A>(this StateT<Env, M, StateT<Env, M, A>> mma)
        where M : Monad<M>, Alternative<M> =>
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
        where M : Monad<M>, Alternative<M> =>
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
        where M : Monad<M>, Alternative<M> =>
        StateT<Env, M, A>.Lift(ma).SelectMany(bind, project);
}
