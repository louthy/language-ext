using LanguageExt.Traits;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// TBoolBool class instance.  Implements
/// 
///     Eq〈(bool, bool)〉
///     Ord〈(bool, bool)〉
///     Bool〈(bool, bool)〉
/// </summary>
public struct TBoolBool : Ord<(bool, bool)>, Bool<(bool, bool)>
{
    /// <summary>
    /// Returns the result of the logical AND operation between `a` and `b`
    /// </summary>
    /// <returns>The result of the logical AND operation between `a` and `b`</returns>
    [Pure]
    public static (bool, bool) And((bool, bool) a, (bool, bool) b) =>
        (a.Item1 && b.Item1,
         a.Item2 && b.Item2);

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
    public static int Compare((bool, bool) x, (bool, bool) y) =>
        x.CompareTo(y);

    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool Equals((bool, bool) x, (bool, bool) y) =>
        x.Item1 == y.Item1 && x.Item2 == y.Item2;

    /// <summary>
    /// Returns False
    /// </summary>
    /// <returns>False</returns>
    [Pure]
    public static (bool, bool) False() =>
        (false, false);

    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int GetHashCode((bool, bool) x) =>
        x.GetHashCode();

    /// <summary>
    /// Returns the result of the logical NOT operation on `a`
    /// </summary>
    /// <returns>The result of the logical NOT operation on `a`</returns>
    [Pure]
    public static (bool, bool) Not((bool, bool) a) =>
        (!a.Item1, !a.Item2);

    /// <summary>
    /// Returns the result of the logical OR operation between `a` and `b`
    /// </summary>
    /// <returns>The result of the logical OR operation between `a` and `b`</returns>
    [Pure]
    public static (bool, bool) Or((bool, bool) a, (bool, bool) b) =>
        (a.Item1 || b.Item1,
         a.Item2 || b.Item2);

    /// <summary>
    /// Returns True
    /// </summary>
    /// <returns>True</returns>
    [Pure]
    public static (bool, bool) True() =>
        (true, true);

    /// <summary>
    /// Returns the result of the logical exclusive-OR operation between `a` and `b`
    /// </summary>
    /// <returns>The result of the logical exclusive-OR operation between `a` and `b`</returns>
    [Pure]
    public static (bool, bool) XOr((bool, bool) a, (bool, bool) b) =>
        (a.Item1 ^ b.Item1,
         a.Item2 ^ b.Item2);

    /// <summary>
    /// Logical implication
    /// </summary>
    /// <returns>If `a` is true that implies `b`, else `true`</returns>
    [Pure]
    public static (bool, bool) Implies((bool, bool) a, (bool, bool) b) =>
        (!a.Item1 || b.Item1,
         !a.Item2 || b.Item2);

    /// <summary>
    /// Logical bi-conditional.  Both `a` and `b` must be `true`, or both `a` and `b` must
    /// be false.
    /// </summary>
    /// <returns>`true` if `a == b`, `false` otherwise</returns>
    [Pure]
    public static (bool, bool) BiCondition((bool, bool) a, (bool, bool) b) =>
        (a.Item1 == b.Item1, a.Item2 == b.Item2);
}
