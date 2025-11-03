using System;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// `TryT` monad extensions
/// </summary>
public static partial class TryTExtensions
{
    public static TryT<M, A> As<M, A>(this K<TryT<M>, A> ma)
        where M : Monad<M> =>
        (TryT<M, A>)ma;

    /// <summary>
    /// Run the transformer
    /// </summary>
    /// <remarks>
    /// This is where the exceptions are caught
    /// </remarks>
    public static K<M, Fin<A>> Run<M, A>(this K<TryT<M>, A> ma) 
        where M : Monad<M> =>
        ma.As().runTry.Map(t => t.Run());

    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static TryT<M, A> Flatten<M, A>(this K<TryT<M>, TryT<M, A>> mma)
        where M : Monad<M> =>
        new(mma.As().runTry.Bind(
                ta => ta.Run() switch
                      {
                          Fin<TryT<M, A>>.Succ(var ma) => ma.runTry,
                          Fin<TryT<M, A>>.Fail(var e)  => M.Pure(Try.Fail<A>(e)),
                          _                            => throw new NotSupportedException()
                      }));

    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static TryT<M, A> Flatten<M, A>(this K<TryT<M>, K<TryT<M>, A>> mma)
        where M : Monad<M> =>
        new(mma.As().runTry.Bind(
                ta => ta.Run() switch
                      {
                          Fin<K<TryT<M>, A>>.Succ(var ma) => ma.As().runTry,
                          Fin<K<TryT<M>, A>>.Fail(var e)  => M.Pure(Try.Fail<A>(e)),
                          _                               => throw new NotSupportedException()
                      }));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    [Pure]
    public static TryT<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, 
        Func<A, K<TryT<M>, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M> =>
        TryT<M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    [Pure]
    public static TryT<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, 
        Func<A, TryT<M, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M> =>
        TryT<M, A>.Lift(ma).SelectMany(bind, project);
}
