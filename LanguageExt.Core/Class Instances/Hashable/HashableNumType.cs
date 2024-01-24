using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Hash of any type in the NumType trait
/// </summary>
public struct HashableNumType<NUMTYPE, NUM, A> : Hashable<NumType<NUMTYPE, NUM, A>>
    where NUM     : Num<A>
    where NUMTYPE : NumType<NUMTYPE, NUM, A>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(NumType<NUMTYPE, NUM, A> x) =>
        x.IsNull() ? 0 : x.GetHashCode();
}

/// <summary>
/// Hash of any type in the NumType trait
/// </summary>
public struct HashableNumType<NUMTYPE, NUM, A, PRED> : Hashable<NumType<NUMTYPE, NUM, A, PRED>>
    where PRED    : Pred<A>
    where NUM     : Num<A>
    where NUMTYPE : NumType<NUMTYPE, NUM, A, PRED>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(NumType<NUMTYPE, NUM, A, PRED> x) =>
        x.IsNull() ? 0 : x.GetHashCode();
}
