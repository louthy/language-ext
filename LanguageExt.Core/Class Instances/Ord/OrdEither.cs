using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Either type hashing
/// </summary>
public struct OrdEither<OrdL, OrdR, L, R> : Ord<Either<L, R>>
    where OrdL : Ord<L>
    where OrdR : Ord<R>
{
    /// <summary>
    /// Ordering test
    /// </summary>
    [Pure]
    public static int Compare(Either<L, R> x, Either<L, R> y) =>
        x.State switch
        {
            EitherStatus.IsRight => y.State switch
                                    {
                                        EitherStatus.IsRight => OrdR.Compare(x.RightValue, y.RightValue),
                                        EitherStatus.IsLeft  => 1,
                                        _                    => 1
                                    },
            EitherStatus.IsLeft => y.State switch
                                   {
                                       EitherStatus.IsLeft  => OrdL.Compare(x.LeftValue, y.LeftValue),
                                       EitherStatus.IsRight => -1,
                                       _                    => 1
                                   },
            EitherStatus.IsBottom => y.State == EitherStatus.IsBottom ? 0 : -1,
            _                     => 0
        };

    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    public static bool Equals(Either<L, R> x, Either<L, R> y) =>
        EqEither<OrdL, OrdR, L, R>.Equals(x, y);

    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(Either<L, R> x) => 
        HashableEither<OrdL, OrdR, L, R>.GetHashCode(x);
}
    
/// <summary>
/// Either type hashing
/// </summary>
public struct OrdEither<L, R> : Ord<Either<L, R>>
{
    /// <summary>
    /// Ordering test
    /// </summary>
    [Pure]
    public static int Compare(Either<L, R> x, Either<L, R> y) => 
        OrdEither<OrdDefault<L>, OrdDefault<R>, L, R>.Compare(x, y);
 
    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    public static bool Equals(Either<L, R> x, Either<L, R> y) => 
        EqEither<EqDefault<L>, EqDefault<R>, L, R>.Equals(x, y);
        
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure] 
    public static int GetHashCode(Either<L, R> x) =>
        HashableEither<HashableDefault<L>, HashableDefault<R>, L, R>.GetHashCode(x);
}
