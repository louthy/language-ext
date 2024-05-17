using LanguageExt.Traits;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Compare the equality and ordering of any type in the NewType
/// type-class
/// </summary>
public struct OrdNewType<NEWTYPE, ORD, A, PRED> : Ord<NewType<NEWTYPE, A, PRED, ORD>>
    where ORD     : Ord<A>
    where PRED    : Pred<A>
    where NEWTYPE : NewType<NEWTYPE, A, PRED, ORD>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(NewType<NEWTYPE, A, PRED, ORD> x, NewType<NEWTYPE, A, PRED, ORD> y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        return ORD.Equals((A)x, (A)y);
    }

    /// <summary>
    /// Compare two values
    /// </summary>
    /// <param name="x">Left hand side of the compare operation</param>
    /// <param name="y">Right hand side of the compare operation</param>
    /// <returns>
    /// if x greater than y : 1
    /// if x less than y    : -1
    /// if x equals y       : 0
    /// </returns>
    [Pure]
    public static int Compare(NewType<NEWTYPE, A, PRED, ORD> mx, NewType<NEWTYPE, A, PRED, ORD> my)
    {
        if (ReferenceEquals(mx, my)) return 0;
        if (ReferenceEquals(mx, null)) return -1;
        if (ReferenceEquals(my, null)) return 1;
        return ORD.Compare((A)mx, (A)my);
    }

    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode(NewType<NEWTYPE, A, PRED, ORD> x) =>
        ORD.GetHashCode(x.Value);
}
