using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Compare the equality of any type in the Either trait
/// </summary>
public struct EqChoiceUnsafe<EQA, EQB, CHOICE, CH, A, B> : Eq<CH>
    where CHOICE : ChoiceUnsafe<CH, A, B>
    where EQA : Eq<A?>
    where EQB : Eq<B?>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(CH x, CH y) =>
        CHOICE.MatchUnsafe(x,
                           Left: a =>
                               CHOICE.MatchUnsafe(y,
                                                  Left: b => equals<EQA, A?>(a, b),
                                                  Right: _ => false),
                           Right: a =>
                               CHOICE.MatchUnsafe(y,
                                                  Left: _ => false,
                                                  Right: b => equals<EQB, B?>(a, b)),
                           Bottom: () => CHOICE.IsBottom(y));

    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(CH x) =>
        HashableChoiceUnsafe<EQA, EQB, CHOICE, CH, A, B>.GetHashCode(x);
}

/// <summary>
/// Compare the equality of any type in the Either trait
/// </summary>
public struct EqChoiceUnsafe<EQB, CHOICE, CH, A, B> : Eq<CH>
    where CHOICE : ChoiceUnsafe<CH, A, B>
    where EQB : Eq<B?>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static  bool Equals(CH x, CH y) =>
        EqChoiceUnsafe<EqDefault<A?>, EQB, CHOICE, CH, A, B>.Equals(x, y);

    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static  int GetHashCode(CH x) =>
        HashableChoiceUnsafe<EQB, CHOICE, CH, A, B>.GetHashCode(x);
}

/// <summary>
/// Compare the equality of any type in the Either trait
/// </summary>
public struct EqChoiceUnsafe<CHOICE, CH, A, B> : Eq<CH>
    where CHOICE : ChoiceUnsafe<CH, A, B>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static  bool Equals(CH x, CH y) =>
        EqChoiceUnsafe<EqDefault<A?>, EqDefault<B?>, CHOICE, CH, A, B>.Equals(x, y);

    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static  int GetHashCode(CH x) =>
        HashableChoiceUnsafe<CHOICE, CH, A, B>.GetHashCode(x);
}
