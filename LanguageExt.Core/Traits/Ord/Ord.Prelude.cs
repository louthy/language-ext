using LanguageExt.ClassInstances;
using LanguageExt.Traits;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Returns true if x is greater than y
    /// </summary>
    /// <param name="x">The first item to compare</param>
    /// <param name="y">The second item to compare</param>
    /// <returns>True if x is greater than y</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool greaterThan<ORD, A>(A x, A y) where ORD : Ord<A> =>
        ORD.Compare(x, y) > 0;

    /// <summary>
    /// Returns true if x is greater than or equal to y
    /// </summary>
    /// <param name="x">The first item to compare</param>
    /// <param name="y">The second item to compare</param>
    /// <returns>True if x is greater than or equal to y</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool greaterOrEq<ORD, A>(A x, A y) where ORD : Ord<A> =>
        ORD.Compare(x, y) >= 0;

    /// <summary>
    /// Returns true if x is less than y
    /// </summary>
    /// <param name="x">The first item to compare</param>
    /// <param name="y">The second item to compare</param>
    /// <returns>True if x is less than y</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool lessThan<ORD, A>(A x, A y) where ORD : Ord<A> =>
        ORD.Compare(x, y) < 0;

    /// <summary>
    /// Returns true if x is less than or equal to y
    /// </summary>
    /// <param name="x">The first item to compare</param>
    /// <param name="y">The second item to compare</param>
    /// <returns>True if x is less than or equal to y</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool lessOrEq<ORD, A>(A x, A y) where ORD : Ord<A> =>
        ORD.Compare(x, y) <= 0;

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
    public static int compare<ORD, A>(A x, A y) where ORD : Ord<A> =>
        ORD.Compare(x, y);

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
    public static int compare<ORD, A>(Option<A> x, Option<A> y) where ORD : Ord<A> =>
        x.CompareTo<ORD>(y);

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
    public static int compare<ORDA, ORDB, A, B>(Either<A, B> x, Either<A, B> y)
        where ORDA : Ord<A>
        where ORDB : Ord<B> =>
        x.CompareTo<ORDA, ORDB>(y);

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
    public static int compare<ORDA, ORDB, A, B>(Validation<A, B> x, Validation<A, B> y)
        where A : Monoid<A>
        where ORDA : Ord<A>, Eq<A>
        where ORDB : Ord<B> =>
        x.CompareTo<ORDA, ORDB>(y);

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
    public static int compare<ORD, A>(A[] x, A[] y)
        where ORD : Ord<A> =>
        OrdArray<ORD, A>.Compare(x, y);

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
    public static int compare<ORD, A>(Lst<A> x, Lst<A> y)
        where ORD : Ord<A> =>
        OrdLst<ORD, A>.Compare(x, y);

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
    public static int compare<NEWTYPE, ORD, T, PRED>(NEWTYPE x, NEWTYPE y)
        where ORD : Ord<T>
        where PRED : Pred<T>
        where NEWTYPE : NewType<NEWTYPE, T, PRED, ORD> =>
        OrdNewType<NEWTYPE, ORD, T, PRED>.Compare(x, y);

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
    public static int compare<NEWTYPE, NUM, T, PRED>(NEWTYPE x, NumType<NEWTYPE, NUM, T, PRED> y)
        where NUM : Num<T>
        where PRED : Pred<T>
        where NEWTYPE : NumType<NEWTYPE, NUM, T, PRED> =>
        OrdNumType<NEWTYPE, NUM, T, PRED>.Compare(x, y);

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
    public static int compare<ORD, L, R>(Either<L, R> x, Either<L, R> y) where ORD : Ord<R> =>
        x.CompareTo<ORD>(y);
}
