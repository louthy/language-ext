using LanguageExt.ClassInstances;
using LanguageExt.Traits;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt;

public static partial class Ord
{
    /// <summary>
    /// Returns true if x is greater than y
    /// </summary>
    /// <param name="x">The first item to compare</param>
    /// <param name="y">The second item to compare</param>
    /// <returns>True if x is greater than y</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool greaterThan<A>(A x, A y) where A : Ord<A> =>
        A.Compare(x, y) > 0;

    /// <summary>
    /// Returns true if x is greater than or equal to y
    /// </summary>
    /// <param name="x">The first item to compare</param>
    /// <param name="y">The second item to compare</param>
    /// <returns>True if x is greater than or equal to y</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool greaterOrEq<A>(A x, A y) where A : Ord<A> =>
        A.Compare(x, y) >= 0;

    /// <summary>
    /// Returns true if x is less than y
    /// </summary>
    /// <param name="x">The first item to compare</param>
    /// <param name="y">The second item to compare</param>
    /// <returns>True if x is less than y</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool lessThan<A>(A x, A y) where A : Ord<A> =>
        A.Compare(x, y) < 0;

    /// <summary>
    /// Returns true if x is less than or equal to y
    /// </summary>
    /// <param name="x">The first item to compare</param>
    /// <param name="y">The second item to compare</param>
    /// <returns>True if x is less than or equal to y</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool lessOrEq<A>(A x, A y) where A : Ord<A> =>
        A.Compare(x, y) <= 0;

    /// <summary>
    /// Compare one item to another to ascertain ordering
    /// </summary>
    /// <param name="x">The first item to compare</param>
    /// <param name="y">The second item to compare</param>
    /// <returns>
    ///  0 if x is equal to y
    /// -1 if x greater than y
    ///  1 if x less than y
    /// </returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int compare<A>(A x, A y) where A : Ord<A> =>
        A.Compare(x, y);

    /// <summary>
    /// Find the maximum value between any two values
    /// </summary>
    /// <param name="x">First value</param>
    /// <param name="y">Second value</param>
    /// <returns>When ordering the two values in ascending order, this is the last of those</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static A max<OrdA, A>(A x, A y) where OrdA : Ord<A> =>
        OrdA.Compare(x, y) > 0
            ? x
            : y;

    /// <summary>
    /// Find the minimum value between any two values
    /// </summary>
    /// <param name="x">First value</param>
    /// <param name="y">Second value</param>
    /// <returns>When ordering the two values in ascending order, this is the first of those</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static A min<OrdA, A>(A x, A y) where OrdA : Ord<A> =>
        OrdA.Compare(x, y) < 0
            ? x
            : y;

    /// <summary>
    /// Find the minimum value between a set of values
    /// </summary>
    /// <param name="x">First value</param>
    /// <param name="y">Second value</param>
    /// <param name="tail">Remaining values</param>
    /// <returns>When ordering the values in ascending order, this is the first of those</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static A min<OrdA, A>(A x, params A[] tail) where OrdA : Ord<A>
    {
        var min = x;
        foreach (var v in tail)
        {
            if (OrdA.Compare(v, x) < 0)
            {
                min = v;
            }
        }
        return min;
    }

    /// <summary>
    /// Find the maximum value between a set of values
    /// </summary>
    /// <param name="x">First value</param>
    /// <param name="y">Second value</param>
    /// <param name="tail">Remaining values</param>
    /// <returns>When ordering the values in ascending order, this is the last of those</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static A max<OrdA, A>(A x, params A[] tail) where OrdA : Ord<A>
    {
        var max = x;
        foreach (var v in tail)
        {
            if (OrdA.Compare(v, x) > 0)
            {
                max = v;
            }
        }
        return max;
    }
}
