using LanguageExt.Traits;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Compare the equality and ordering of any type in the Optional
/// trait
/// </summary>
public struct OrdOptional<OrdA, OPTION, OA, A> : Ord<OA>
    where OrdA   : Ord<A>
    where OPTION : Optional<OA, A>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(OA x, OA y) =>
        EqOptional<OrdA, OPTION, OA, A>.Equals(x, y);

    /// <summary>
    /// Compare two values
    /// </summary>
    /// <param name="x">Left hand side of the compare operation</param>
    /// <param name="y">Right hand side of the compare operation</param>
    /// <returns>
    /// if x greater than y : 1
    /// 
    /// if x less than y    : -1
    /// 
    /// if x equals y       : 0
    /// </returns>
    [Pure]
    public static int Compare(OA mx, OA my)
    {
        var xIsSome = OPTION.IsSome(mx);
        var yIsSome = OPTION.IsSome(my);
        var xIsNone = !xIsSome;
        var yIsNone = !yIsSome;

        if (xIsNone && yIsNone) return 0;
        if (xIsSome && yIsNone) return 1;
        if (xIsNone && yIsSome) return -1;

        return OPTION.Match(mx,
                            Some: a =>
                                OPTION.Match(my,
                                             Some: b => compare<OrdA, A>(a, b),
                                             None: () => 0),
                            None: () => 0);
    }

    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode(OA x) =>
        x?.GetHashCode() ?? 0;
}

/// <summary>
/// Compare the equality and ordering of any type in the Optional
/// trait
/// </summary>
public struct OrdOptional<OPTION, OA, A> : Ord<OA>
    where OPTION : Optional<OA, A>
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals(OA x, OA y) =>
        OrdOptional<OrdDefault<A>, OPTION, OA, A>.Equals(x, y);

    /// <summary>
    /// Compare two values
    /// </summary>
    /// <param name="x">Left hand side of the compare operation</param>
    /// <param name="y">Right hand side of the compare operation</param>
    /// <returns>
    /// if x greater than y : 1
    /// 
    /// if x less than y    : -1
    /// 
    /// if x equals y       : 0
    /// </returns>
    [Pure]
    public static int Compare(OA mx, OA my) =>
        OrdOptional<OrdDefault<A>, OPTION, OA, A>.Compare(mx, my);

    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode(OA x) =>
        OrdOptional<OrdDefault<A>, OPTION, OA, A>.GetHashCode(x);
}
