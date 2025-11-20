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
    /// Match Right and return a context.  You must follow this with .Left(...) to complete the match
    /// </summary>
    /// <param name="right">Action to invoke if the Either is in a Right state</param>
    /// <returns>Context that must have Left() called upon it.</returns>
    [Pure]
    public static EitherUnitContext<L, R> Right<L, R>(this K<Either<L>, R> ma, Action<R> right) =>
        new (ma.As(), right);

    /// <summary>
    /// Match Right and return a context.  You must follow this with .Left(...) to complete the match
    /// </summary>
    /// <param name="right">Action to invoke if the Either is in a Right state</param>
    /// <returns>Context that must have Left() called upon it.</returns>
    [Pure]
    public static EitherContext<L, R, R2> Right<L, R, R2>(this K<Either<L>, R> ma, Func<R, R2> right) =>
        new (ma.As(), right);
 
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
        ma.As().Bind(x => pred(x) ? Either.Right<L, A>(x) : Either.Left<L, A>(L.Empty));

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
            Either<L, R>.Right => Pure(ma.RightValue),
            Either<L, R>.Left  => Fail(ma.LeftValue),
            _                  => throw new BottomException()
        };

    /// <summary>
    /// Convert to an Eff
    /// </summary>
    /// <returns>Eff monad</returns>
    [Pure]
    public static Eff<R> ToEff<R>(this Either<Error, R> ma) =>
        ma switch
        {
            Either<Error, R>.Right => Pure(ma.RightValue),
            Either<Error, R>.Left  => Fail(ma.LeftValue),
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
            Either<Exception, R>.Right => Pure(ma.RightValue),
            Either<Exception, R>.Left  => Fail<Error>(ma.LeftValue),
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
            Either<string, R>.Right => Pure(ma.RightValue),
            Either<string, R>.Left  => Fail(Error.New(ma.LeftValue)),
            _                       => throw new BottomException()
        };
}
