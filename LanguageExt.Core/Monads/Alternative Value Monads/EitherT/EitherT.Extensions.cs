using System;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Either monad extensions
/// </summary>
public static class EitherTExt
{
    public static EitherT<L, M, A> As<L, M, A>(this K<EitherT<L, M>, A> ma)
        where M : Monad<M> =>
        (EitherT<L, M, A>)ma;

    /// <summary>
    /// Get the outer task and wrap it up in a new IO within the EitherT IO
    /// </summary>
    public static EitherT<L, IO, A> Flatten<L, A>(this Task<EitherT<L, IO, A>> tma) =>
        EitherT<L, IO, EitherT<L, IO, A>>
           .Lift(IO.liftAsync(async () => await tma.ConfigureAwait(false)))
           .Flatten();

    /// <summary>
    /// Lift the task
    /// </summary>
    public static EitherT<L, IO, A> ToIO<L, A>(this Task<Either<L, A>> ma) =>
        liftIO(ma);
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static EitherT<L, M, A> Flatten<L, M, A>(this EitherT<L, M, EitherT<L, M, A>> mma)
        where M : Monad<M> =>
        mma.Bind(identity);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    [Pure]
    public static EitherT<L, M, C> SelectMany<L, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, K<EitherT<L, M>, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M> =>
        EitherT<L, M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    [Pure]
    public static EitherT<L, M, C> SelectMany<L, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, EitherT<L, M, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M> =>
        EitherT<L, M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Applicative apply
    /// </summary>
    [Pure]
    public static EitherT<L, M, B> Apply<L, M, A, B>(this EitherT<L, M, Func<A, B>> mf, EitherT<L, M, A> ma) 
        where M : Monad<M> => 
        mf.As().Bind(ma.As().Map);

    /// <summary>
    /// Applicative action
    /// </summary>
    [Pure]
    public static EitherT<L, M, B> Action<L, M, A, B>(this EitherT<L, M, A> ma, EitherT<L, M, B> mb)
        where M : Monad<M> => 
        ma.As().Bind(_ => mb);
}
