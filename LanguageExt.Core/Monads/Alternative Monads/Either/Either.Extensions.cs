using System;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using NSE = System.NotSupportedException;
using LanguageExt.Traits;
using LanguageExt.Common;

namespace LanguageExt;

/// <summary>
/// Extension methods for Either
/// </summary>
public static partial class EitherExtensions
{
    public static Either<L, R> As<L, R>(this K<Either<L>, R> ma) =>
        (Either<L, R>)ma;
 
    public static Either<L, R> As2<L, R>(this K<Either, L, R> ma) =>
        (Either<L, R>)ma;
 
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Either<L, R> Flatten<L, R>(this K<Either<L>, Either<L, R>> ma) =>
        ma.As().Bind(x => x);
 
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Either<L, R> Flatten<L, R>(this K<Either<L>, K<Either<L>, R>> ma) =>
        ma.As().Bind(x => x);

    /// <summary>
    /// Filtering based on predicate.  
    /// </summary>
    /// <remarks>>
    /// If the predicate returns false, then `Left(L.Empty)` is yielded and therefore `L` must be a monoid.  
    /// </remarks>
    [Pure]
    public static Either<L, A> Where<L, A>(this K<Either<L>, A> ma, Func<A, bool> pred)
        where L : Monoid<L> =>
        ma.Filter(pred);

    /// <summary>
    /// Filtering based on predicate.  
    /// </summary>
    /// <remarks>>
    /// If the predicate returns false, then `Left(L.Empty)` is yielded and therefore `L` must be a monoid.  
    /// </remarks>
    [Pure]
    public static Either<L, A> Filter<L, A>(this K<Either<L>, A> ma, Func<A, bool> pred)
        where L : Monoid<L> =>
        ma.As().Bind(x => pred(x) ? Either<L, A>.Right(x) : Either<L, A>.Left(L.Empty));

    /*
    /// <summary>
    /// Partitions a foldable of `Either` into two sequences.
    /// 
    /// All the `Left` elements are extracted, in order, to the first component of the output.
    /// Similarly, the `Right` elements are extracted to the second component of the output.
    /// </summary>
    /// <returns>A pair containing the sequences of partitioned values</returns>
    [Pure]
    public static (Seq<L> Lefts, Seq<R> Rights) Partition<F, L, R>(this K<F, Either<L, R>> self)
        where F : Foldable<F> =>
        self.Fold((Left: Seq<L>.Empty, Right: Seq<R>.Empty),
                  (s, ma) =>
                      ma switch
                      {
                          Either.Right<L, R> (var r) => (s.Left, s.Right.Add(r)),
                          Either.Left<L, R> (var l)  => (s.Left.Add(l), s.Right),
                          _                          => throw new NSE()
                      });

    /// <summary>
    /// Partitions a foldable of `Either` into two lists and returns the `Left` items only.
    /// </summary>
    /// <returns>A sequence of partitioned items</returns>
    [Pure]
    public static Seq<L> Lefts<F, L, R>(this K<F, Either<L, R>> self)
        where F : Foldable<F> =>
        self.Fold(Seq<L>.Empty,
                  (s, ma) =>
                      ma switch
                      {
                          Either.Left<L, R> (var l) => s.Add(l),
                          _                         => throw new NSE()
                      });

    /// <summary>
    /// Partitions a foldable of `Either` into two lists and returns the `Right` items only.
    /// </summary>
    /// <returns>A sequence of partitioned items</returns>
    [Pure]
    public static Seq<R> Rights<F, L, R>(this K<F, Either<L, R>> self)
        where F : Foldable<F> =>
        self.Fold(Seq<R>.Empty,
                  (s, ma) =>
                      ma switch
                      {
                          Either.Right<L, R> (var r) => s.Add(r),
                          _                          => throw new NSE()
                      });
                      */
    
    [Pure]
    public static Validation<L, R> ToValidation<L, R>(this Either<L, R> ma)
        where L : Monoid<L> =>
        ma switch
        {
            Either.Right<L, R>       => Pure(ma.RightValue),
            Either.Left<L, R> => Fail(ma.LeftValue),
            _                 => throw new BottomException()
        };

    /// <summary>
    /// Convert to an Eff
    /// </summary>
    /// <returns>Eff monad</returns>
    [Pure]
    public static Eff<R> ToEff<R>(this Either<Error, R> ma) =>
        ma switch
        {
            Either.Right<Error, R> => Pure(ma.RightValue),
            Either.Left<Error, R>  => Fail(ma.LeftValue),
            _                      => throw new BottomException()
        };

    /// <summary>
    /// Convert to an Eff
    /// </summary>
    /// <returns>Eff monad</returns>
    [Pure]
    public static Eff<R> ToEff<R>(this Either<Exception, R> ma) =>
        ma switch
        {
            Either.Right<Exception, R> => Pure(ma.RightValue),
            Either.Left<Exception, R>  => Fail<Error>(ma.LeftValue),
            _                          => throw new BottomException()
        };

    /// <summary>
    /// Convert to an Eff
    /// </summary>
    /// <returns>Eff monad</returns>
    [Pure]
    public static Eff<R> ToEff<R>(this Either<string, R> ma) =>
        ma switch
        {
            Either.Right<string, R> => Pure(ma.RightValue),
            Either.Left<string, R>  => Fail(Error.New(ma.LeftValue)),
            _                       => throw new BottomException()
        };
}
