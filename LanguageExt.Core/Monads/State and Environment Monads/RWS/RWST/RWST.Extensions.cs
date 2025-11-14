using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class RWSTExtensions
{
    public static RWST<R, W, S, M, A> As<R, W, S, M, A>(this K<RWST<R, W, S, M>, A> ma) 
        where M : Monad<M>
        where W : Monoid<W> =>
        (RWST<R, W, S, M, A>)ma;
    
    public static K<M, (A Value, W Output, S State)> Run<R, W, S, M, A>(
        this K<RWST<R, W, S, M>, A> ma, R env, W output, S state) 
        where M : Monad<M>
        where W : Monoid<W> =>
        ma.As().runRWST((env, output, state));
    
    public static K<M, (A Value, W Output, S State)> Run<R, W, S, M, A>(
        this K<RWST<R, W, S, M>, A> ma, R env, S state) 
        where M : Monad<M>
        where W : Monoid<W> =>
        ma.As().runRWST((env, W.Empty, state));
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static RWST<R, W, S, M, A> Flatten<R, W, S, M, A>(this RWST<R, W, S, M, RWST<R, W, S, M, A>> mma)
        where W : Monoid<W>
        where M : Monad<M> =>
        mma.Bind(x => x);
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static RWST<R, W, S, M, A> Flatten<R, W, S, M, A>(this RWST<R, W, S, M, K<RWST<R, W, S, M>, A>> mma)
        where W : Monoid<W>
        where M : Monad<M> =>
        mma.Bind(x => x);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public static RWST<R, W, S, M, C> SelectMany<R, W, S, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, K<RWST<R, W, S, M>, B>> bind, 
        Func<A, B, C> project)
        where W : Monoid<W>
        where M : Monad<M> =>
        RWST<R, W, S, M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public static RWST<R, W, S, M, C> SelectMany<R, W, S, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, RWST<R, W, S, M, B>> bind, 
        Func<A, B, C> project)
        where W : Monoid<W>
        where M : Monad<M> =>
        RWST<R, W, S, M, A>.Lift(ma).SelectMany(bind, project);    
    

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public static RWST<R, W, S, M, C> SelectMany<R, W, S, M, A, B, C>(
        this K<IO, A> ma, 
        Func<A, K<RWST<R, W, S, M>, B>> bind, 
        Func<A, B, C> project)
        where W : Monoid<W>
        where M : Monad<M> =>
        RWST<R, W, S, M, A>.LiftIO(ma).SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public static RWST<R, W, S, M, C> SelectMany<R, W, S, M, A, B, C>(
        this K<IO, A> ma, 
        Func<A, RWST<R, W, S, M, B>> bind, 
        Func<A, B, C> project)
        where W : Monoid<W>
        where M : Monad<M> =>
        RWST<R, W, S, M, A>.LiftIO(ma).SelectMany(bind, project);    
}
