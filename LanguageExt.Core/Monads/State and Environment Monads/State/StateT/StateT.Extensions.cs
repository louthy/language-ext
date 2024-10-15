using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// State monad extensions
/// </summary>
public static partial class StateTExtensions
{
    public static StateT<S, M, A> As<S, M, A>(this K<StateT<S, M>, A> ma)
        where M : Monad<M>, SemigroupK<M> =>
        (StateT<S, M, A>)ma;

    /// <summary>
    /// Run the state monad 
    /// </summary>
    /// <param name="state">Initial state</param>
    /// <returns>Bound monad</returns>
    public static K<M, (A Value, S State)> Run<S, M, A>(this K<StateT<S, M>, A> ma, S state) 
        where M : Monad<M>, SemigroupK<M> =>
        ((StateT<S, M, A>)ma).runState(state);
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static StateT<S, M, A> Flatten<S, M, A>(this StateT<S, M, StateT<S, M, A>> mma)
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
    public static StateT<S, M, C> SelectMany<S, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, K<StateT<S, M>, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M>, SemigroupK<M> =>
        StateT<S, M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public static StateT<S, M, C> SelectMany<S, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, StateT<S, M, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M>, SemigroupK<M> =>
        StateT<S, M, A>.Lift(ma).SelectMany(bind, project);
}
