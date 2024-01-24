using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Either type hashing
/// </summary>
public struct HashableEitherUnsafe<HashableL, HashableR, L, R> : Hashable<EitherUnsafe<L, R>>
    where HashableL : Hashable<L?>
    where HashableR : Hashable<R?>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(EitherUnsafe<L, R> x) =>
        x.State switch
        {
            EitherStatus.IsRight => HashableR.GetHashCode(x.RightValue),
            EitherStatus.IsLeft  => HashableL.GetHashCode(x.LeftValue),
            _                    => 0
        };
}
    
/// <summary>
/// Either type hashing
/// </summary>
public struct HashableEitherUnsafe<L, R> : Hashable<EitherUnsafe<L, R>>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure] 
    public static int GetHashCode(EitherUnsafe<L, R> x) =>
        HashableEitherUnsafe<HashableDefault<L?>, HashableDefault<R?>, L, R>.GetHashCode(x);
}
