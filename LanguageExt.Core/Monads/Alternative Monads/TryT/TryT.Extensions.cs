using System;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Either monad extensions
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
                          Fin.Succ<TryT<M, A>>(var ma) => ma.runTry,
                          Fin.Fail<TryT<M, A>>(var e)  => M.Pure(Try.Fail<A>(e)),
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
                          Fin.Succ<K<TryT<M>, A>>(var ma) => ma.As().runTry,
                          Fin.Fail<K<TryT<M>, A>>(var e)  => M.Pure(Try.Fail<A>(e)),
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

    /// <summary>
    /// Applicative apply
    /// </summary>
    [Pure]
    public static TryT<M, B> Apply<M, A, B>(this TryT<M, Func<A, B>> mf, TryT<M, A> ma)
        where M : Monad<M> =>
        new(mf.runTry.Bind(
                mf1 => ma.runTry.Bind(
                    ma1 => M.Pure(mf1.Apply(ma1)))));

    /// <summary>
    /// Applicative action
    /// </summary>
    [Pure]
    public static TryT<M, B> Action<M, A, B>(this TryT<M, A> ma, TryT<M, B> mb)
        where M : Monad<M> => 
        ma.As().Bind(_ => mb);
}
