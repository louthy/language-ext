using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Hash of any type in the Optional trait
/// </summary>
public struct HashableOptional<HashA, OptionA, OA, A> : Hashable<OA>
    where HashA   : Hashable<A>
    where OptionA : Optional<OA, A>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(OA x) =>
        OptionA.Match(x, HashA.GetHashCode, 0);
}

/// <summary>
/// Hash of any type in the Optional trait
/// </summary>
public struct HashableOptional<OptionA, OA, A> : Hashable<OA>
    where OptionA : Optional<OA, A>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(OA x) =>
        HashableOptional<HashableDefault<A>, OptionA, OA, A>.GetHashCode(x);
}
