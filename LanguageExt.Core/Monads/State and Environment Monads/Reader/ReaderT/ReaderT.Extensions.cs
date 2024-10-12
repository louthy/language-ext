using System;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Reader monad extensions
/// </summary>
public static partial class ReaderTExtensions
{
     // public static Reader<Env, A> As<Env, A>(this K<ReaderT<Env, Identity>, A> ma) =>
     //    (Reader<Env, A>)ma;
    
    public static ReaderT<Env, M, A> As<Env, M, A>(this K<ReaderT<Env, M>, A> ma)
        where M : Monad<M>, SemiAlternative<M> =>
        (ReaderT<Env, M, A>)ma;
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static ReaderT<Env, M, A> Flatten<Env, M, A>(this ReaderT<Env, M, ReaderT<Env, M, A>> mma)
        where M : Monad<M>, SemiAlternative<M> =>
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
        where M : Monad<M>, SemiAlternative<M> =>
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
        where M : Monad<M>, SemiAlternative<M> =>
        ReaderT<Env, M, A>.Lift(ma).SelectMany(bind, project);
}
