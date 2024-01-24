using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Compare the equality of any type in the Choice trait.  Taking into
/// account the ordering of both possible bound values.
/// </summary>
public struct OrdChoiceUnsafe<OrdA, OrdB, ChoiceAB, CH, A, B> : Ord<CH>
    where ChoiceAB : ChoiceUnsafe<CH, A, B>
    where OrdA : Ord<A?>
    where OrdB : Ord<B?>
{
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
    public static int Compare(CH x, CH y) =>
        ChoiceAB.MatchUnsafe(x,
                             Left: a =>
                                 ChoiceAB.MatchUnsafe(y,
                                                      Left: b => compare<OrdA, A?>(a, b),
                                                      Right: _ => 1),
                             Right: a =>
                                 ChoiceAB.MatchUnsafe(y,
                                                      Left: _ => -1,
                                                      Right: b => compare<OrdB, B?>(a, b)));

    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(CH x, CH y) =>
        ChoiceAB.MatchUnsafe(x,
                             Left: a =>
                                 ChoiceAB.MatchUnsafe(y,
                                                      Left: b => equals<OrdA, A?>(a, b),
                                                      Right: _ => false),
                             Right: a =>
                                 ChoiceAB.MatchUnsafe(y,
                                                      Left: _ => false,
                                                      Right: b => equals<OrdB, B?>(a, b)));


    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode(CH x) =>
        ChoiceAB.MatchUnsafe(x,
                             Left: a => a?.GetHashCode() ?? 0,
                             Right: b => b?.GetHashCode() ?? 0);
}

/// <summary>
/// Compare the equality of any type in the Choice trait.  Taking into
/// account only the 'success bound value' of B.
/// </summary>
public struct OrdChoiceUnsafe<OrdB, ChoiceAB, CH, A, B> : Ord<CH>
    where ChoiceAB : ChoiceUnsafe<CH, A, B>
    where OrdB     : Ord<B?>
{
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
    public static int Compare(CH x, CH y) =>
        OrdChoiceUnsafe<OrdDefault<A?>, OrdB, ChoiceAB, CH, A, B>.Compare(x, y);

    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(CH x, CH y) =>
        OrdChoiceUnsafe<OrdDefault<A?>, OrdB, ChoiceAB, CH, A, B>.Equals(x, y);

    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode(CH x) =>
        OrdChoiceUnsafe<OrdDefault<A?>, OrdB, ChoiceAB, CH, A, B>.GetHashCode(x);
}

/// <summary>
/// Compare the equality of any type in the Choice trait.  Taking into
/// account only the 'success bound value' of B.
/// </summary>
public struct OrdChoiceUnsafe<ChoiceAB, CH, A, B> : Ord<CH>
    where ChoiceAB : ChoiceUnsafe<CH, A, B>
{
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
    public static int Compare(CH x, CH y) =>
        OrdChoiceUnsafe<OrdDefault<A?>, OrdDefault<B?>, ChoiceAB, CH, A, B>.Compare(x, y);

    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(CH x, CH y) =>
        OrdChoiceUnsafe<OrdDefault<A?>, OrdDefault<B?>, ChoiceAB, CH, A, B>.Equals(x, y);

    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode(CH x) =>
        OrdChoiceUnsafe<OrdDefault<A?>, OrdDefault<B?>, ChoiceAB, CH, A, B>.GetHashCode(x);
}
