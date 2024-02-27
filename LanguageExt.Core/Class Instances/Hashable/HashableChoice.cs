using LanguageExt.Traits;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Hash of any type in the Choice trait
/// </summary>
public struct HashableChoice<HashA, HashB, CHOICE, CH, A, B> : Hashable<CH>
    where CHOICE : Choice<CH, A, B>
    where HashA  : Hashable<A>
    where HashB  : Hashable<B>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(CH x) =>
        CHOICE.Match(x, Left: HashA.GetHashCode, Right: HashB.GetHashCode);
}

/// <summary>
/// Hash of any type in the Choice trait
/// </summary>
public struct HashableChoice<HashB, CHOICE, CH, A, B> : Hashable<CH>
    where CHOICE : Choice<CH, A, B>
    where HashB : Hashable<B>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(CH x) =>
        HashableChoice<EqDefault<A>, HashB, CHOICE, CH, A, B>.GetHashCode(x);
}

/// <summary>
/// Hash of any type in the Either trait
/// </summary>
public struct HashableChoice<CHOICE, CH, A, B> : Hashable<CH>
    where CHOICE : Choice<CH, A, B>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(CH x) =>
        HashableChoice<HashableDefault<A>, HashableDefault<B>, CHOICE, CH, A, B>.GetHashCode(x);
}
