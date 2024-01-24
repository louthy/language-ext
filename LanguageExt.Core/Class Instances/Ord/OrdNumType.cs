using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Compare the equality and ordering of any type in the NumType
/// trait
/// </summary>
public struct OrdNumType<NUMTYPE, NUM, A> : Ord<NumType<NUMTYPE, NUM, A>>
    where NUM : Num<A>
    where NUMTYPE : NumType<NUMTYPE, NUM, A>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(NumType<NUMTYPE, NUM, A> x, NumType<NUMTYPE, NUM, A> y) =>
        EqNumType<NUMTYPE, NUM, A>.Equals(x, y);

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
    public static int Compare(NumType<NUMTYPE, NUM, A> mx, NumType<NUMTYPE, NUM, A> my)
    {
        if (ReferenceEquals(mx, my)) return 0;
        if (ReferenceEquals(mx, null)) return -1;
        if (ReferenceEquals(my, null)) return 1;
        return NUM.Compare((A)mx, (A)my);
    }

    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode(NumType<NUMTYPE, NUM, A> x) =>
        x.IsNull() ? 0 : x.GetHashCode();
}

/// <summary>
/// Compare the equality and ordering of any type in the NumType
/// trait
/// </summary>
public struct OrdNumType<NUMTYPE, NUM, A, PRED> : Ord<NumType<NUMTYPE, NUM, A, PRED>>
    where NUM     : Num<A>
    where PRED    : Pred<A>
    where NUMTYPE : NumType<NUMTYPE, NUM, A, PRED>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(NumType<NUMTYPE, NUM, A, PRED> x, NumType<NUMTYPE, NUM, A, PRED> y) =>
        EqNumType<NUMTYPE, NUM, A, PRED>.Equals(x, y);

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
    public static int Compare(NumType<NUMTYPE, NUM, A, PRED> mx, NumType<NUMTYPE, NUM, A, PRED> my)
    {
        if (ReferenceEquals(mx, my)) return 0;
        if (ReferenceEquals(mx, null)) return -1;
        if (ReferenceEquals(my, null)) return 1;
        return NUM.Compare((A)mx, (A)my);
    }

    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode(NumType<NUMTYPE, NUM, A, PRED> x) =>
        x.IsNull() ? 0 : x.GetHashCode();
}
