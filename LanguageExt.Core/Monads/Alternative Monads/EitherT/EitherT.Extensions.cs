using System;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;
using NSE = System.NotSupportedException;

namespace LanguageExt;

/// <summary>
/// EitherT monad-transformer extensions
/// </summary>
public static partial class EitherTExtensions
{
    public static EitherT<L, M, R> As<L, M, R>(this K<EitherT<L, M>, R> ma)
        where M : Monad<M> =>
        (EitherT<L, M, R>)ma;
 
    public static EitherT<L, M, R> As2<L, M, R>(this K<EitherT<M>, L, R> ma) 
        where M : Monad<M> =>
        (EitherT<L, M, R>)ma;

    public static FinT<M, R> ToFin<M, R>(this K<EitherT<Error, M>, R> ma) 
        where M : Monad<M> =>
        new(ma.As().runEither.Map(ma => ma.ToFin()));

    /// <summary>
    /// Runs the EitherT exposing the outer monad with an inner wrapped `Either`
    /// </summary>
    public static K<M, Either<L, A>> Run<L, M, A>(this K<EitherT<L, M>, A> ma)
        where M : Monad<M> =>
        ma.As().runEither;

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, B> Bind<L, M, A, B>(this K<EitherT<L, M>, A> ma, Func<A, IO<B>> f) 
        where M : MonadIO<M> =>
        ma.As().Bind(a => EitherT.liftIO<L, M, B>(f(a)));
    
    /// <summary>
    /// Get the outer task and wrap it up in a new IO within the EitherT IO
    /// </summary>
    public static EitherT<L, IO, A> Flatten<L, A>(this Task<EitherT<L, IO, A>> tma) =>
        EitherT
           .lift<L, IO, EitherT<L, IO, A>>(IO.liftAsync(async () => await tma.ConfigureAwait(false)))
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
    /// Filtering based on predicate.  
    /// </summary>
    /// <remarks>>
    /// If the predicate returns false, then `Left(L.Empty)` is yielded and therefore `L` must be a monoid.  
    /// </remarks>
    [Pure]
    public static EitherT<L, M, A> Where<L, M, A>(this K<EitherT<L, M>, A> ma, Func<A, bool> pred)
        where L : Monoid<L>
        where M : Monad<M> =>
        ma.Filter(pred);

    /// <summary>
    /// Filtering based on predicate.  
    /// </summary>
    /// <remarks>>
    /// If the predicate returns false, then `Left(L.Empty)` is yielded and therefore `L` must be a monoid.  
    /// </remarks>
    [Pure]
    public static EitherT<L, M, A> Filter<L, M, A>(this K<EitherT<L, M>, A> ma, Func<A, bool> pred)
        where L : Monoid<L>
        where M : Monad<M> =>
        ma.As().Bind(x => pred(x) ? EitherT.Right<L, M, A>(x) : EitherT.Left<L, M, A>(L.Empty));    
    
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
        EitherT.lift<L, M, A>(ma).SelectMany(bind, project);

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
        EitherT.lift<L, M, A>(ma).SelectMany(bind, project);

    /// <summary>
    /// Partitions a foldable of `EitherT` into two sequences.
    /// 
    /// All the `Left` elements are extracted, in order, to the first component of the output.
    /// Similarly, the `Right` elements are extracted to the second component of the output.
    /// </summary>
    /// <returns>A pair containing the sequences of partitioned values</returns>
    [Pure]
    public static K<M, (Seq<L> Lefts, Seq<R> Rights)> Partition<F, L, M, R>(this K<F, EitherT<L, M, R>> self)
        where F : Foldable<F>
        where M : Monad<M> =>
        self.Fold(M.Pure((Left: Seq<L>.Empty, Right: Seq<R>.Empty)),
                  (ms, ma) =>
                      ms.Bind(s => ma.Run().Map(a => a switch
                                                     {
                                                         Either<L, R>.Right (var r) => (s.Left, s.Right.Add(r)),
                                                         Either<L, R>.Left (var l)  => (s.Left.Add(l), s.Right),
                                                         _                          => throw new NSE()
                                                     })));

    /// <summary>
    /// Partitions a foldable of `EitherT` into two lists and returns the `Left` items only.
    /// </summary>
    /// <returns>A sequence of partitioned items</returns>
    [Pure]
    public static K<M, Seq<L>> Lefts<F, L, M, R>(this K<F, EitherT<L, M, R>> self)
        where F : Foldable<F>
        where M : Monad<M> =>
        self.Fold(M.Pure(Seq<L>.Empty),
                  (ms, ma) =>
                      ms.Bind(s => ma.Run().Map(a => a switch
                                                     {
                                                         Either<L, R>.Left (var l)  => s.Add(l),
                                                         _                          => throw new NSE()
                                                     })));

    /// <summary>
    /// Partitions a foldable of `EitherT` into two lists and returns the `Right` items only.
    /// </summary>
    /// <returns>A sequence of partitioned items</returns>
    [Pure]
    public static K<M, Seq<R>> Rights<F, L, M, R>(this K<F, EitherT<L, M, R>> self)
        where F : Foldable<F>
        where M : Monad<M> =>
        self.Fold(M.Pure(Seq<R>.Empty),
                  (ms, ma) =>
                      ms.Bind(s => ma.Run().Map(a => a switch
                                                     {
                                                         Either<L, R>.Right (var r) => s.Add(r),
                                                         _                          => throw new NSE()
                                                     })));
}
