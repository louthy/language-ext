#nullable enable
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt;

public static partial class TypeClass
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
        OrdOptional<ORD, MOption<A>, Option<A>, A>.Compare(x, y);

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
    public static int compare<ORD, A>(OptionUnsafe<A> x, OptionUnsafe<A> y) where ORD : Ord<A> =>
        OrdOptionalUnsafe<ORD, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Compare(x, y);

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
        OrdChoice<ORDA, ORDB, MEither<A, B>, Either<A, B>, A, B>.Compare(x, y);

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
    public static int compare<ORDA, ORDB, A, B>(EitherUnsafe<A, B> x, EitherUnsafe<A, B> y)
        where ORDA : Ord<A>
        where ORDB : Ord<B> =>
        OrdChoiceUnsafe<ORDA, ORDB, MEitherUnsafe<A, B>, EitherUnsafe<A, B>, A, B>.Compare(x, y);

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
    public static int compare<MonoidA, ORDA, ORDB, A, B>(Validation<MonoidA, A, B> x, Validation<MonoidA, A, B> y)
        where MonoidA : Monoid<A>, Eq<A>
        where ORDA : Ord<A>
        where ORDB : Ord<B> =>
        OrdChoice<ORDA, ORDB, FoldValidation<MonoidA, A, B>, Validation<MonoidA, A, B>, A, B>.Compare(x, y);

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
    public static int compare<ORD, A>(TryOption<A> x, TryOption<A> y) where ORD : Ord<A> =>
        OrdOptional<ORD, MTryOption<A>, TryOption<A>, A>.Compare(x, y);

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
    public static int compare<ORD, A>(Try<A> x, Try<A> y) where ORD : Ord<A> =>
        OrdOptional<ORD, MTry<A>, Try<A>, A>.Compare(x, y);

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
        OrdChoice<ORD, MEither<L, R>, Either<L, R>, L, R>.Compare(x, y);

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
    public static int compare<ORD, L, R>(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) where ORD : Ord<R> =>
        OrdChoiceUnsafe<ORD, MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>.Compare(x, y);

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
    /// Find the minimum value between a set of values
    /// </summary>
    /// <param name="x">First value</param>
    /// <param name="y">Second value</param>
    /// <param name="tail">Remaining values</param>
    /// <returns>When ordering the values in ascending order, this is the first of those</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static A min<OrdA, A>(A x, A y, A z, params A[] tail) where OrdA : Ord<A> =>
        MArray<A>.Fold(tail, min<OrdA, A>(x, min<OrdA, A>(y, z)), min<OrdA, A>)(Prelude.unit);

    /// <summary>
    /// Find the maximum value between a set of values
    /// </summary>
    /// <param name="x">First value</param>
    /// <param name="y">Second value</param>
    /// <param name="tail">Remaining values</param>
    /// <returns>When ordering the values in ascending order, this is the last of those</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static A max<OrdA, A>(A x, A y, A z, params A[] tail) where OrdA : Ord<A> =>
        MArray<A>.Fold(tail, max<OrdA, A>(x, max<OrdA, A>(y, z)), max<OrdA, A>)(Prelude.unit);
}
