using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Hash of any type in the Either trait
/// </summary>
public struct HashableChoiceUnsafe<HashA, HashB, CHOICE, CH, A, B> : Hashable<CH>
    where CHOICE : ChoiceUnsafe<CH, A, B>
    where HashA : Hashable<A?>
    where HashB : Hashable<B?>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(CH x) =>
        CHOICE.MatchUnsafe(x, Left: HashA.GetHashCode, Right: HashB.GetHashCode);
}

/// <summary>
/// Hash of any type in the Either trait
/// </summary>
public struct HashableChoiceUnsafe<HashB, CHOICE, CH, A, B> : Hashable<CH>
    where CHOICE : ChoiceUnsafe<CH, A, B>
    where HashB : Hashable<B?>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(CH x) =>
        HashableChoiceUnsafe<EqDefault<A?>, HashB, CHOICE, CH, A, B>.GetHashCode(x);
}

/// <summary>
/// Hash of any type in the Either trait
/// </summary>
public struct HashableChoiceUnsafe<CHOICE, CH, A, B> : Hashable<CH>
    where CHOICE : ChoiceUnsafe<CH, A, B>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(CH x) =>
        HashableChoiceUnsafe<HashableDefault<A?>, HashableDefault<B?>, CHOICE, CH, A, B>.GetHashCode(x);
}
