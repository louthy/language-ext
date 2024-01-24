using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Hash of any type in the Optional trait
/// </summary>
public struct HashableOptionalUnsafe<HashA, OPTION, OA, A> : Hashable<OA>
    where HashA  : Hashable<A?>
    where OPTION : OptionalUnsafe<OA, A>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(OA x) =>
        OPTION.MatchUnsafe(x, HashA.GetHashCode, 0);
}

/// <summary>
/// Hash of any type in the Optional trait
/// </summary>
public struct HashableOptionalUnsafe<OPTION, OA, A> : Hashable<OA>
    where OPTION : OptionalUnsafe<OA, A>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(OA x) =>
        HashableOptionalUnsafe<HashableDefault<A?>, OPTION, OA, A>.GetHashCode(x);
}
