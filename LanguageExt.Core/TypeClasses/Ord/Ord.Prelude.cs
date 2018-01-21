using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances.Pred;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class TypeClass
    {
        /// <summary>
        /// Returns true if x is greater than y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is greater than y</returns>
        [Pure]
        public static bool greaterThan<ORD, A>(A x, A y) where ORD : struct, Ord<A> =>
            default(ORD).Compare(x, y) >0;

        /// <summary>
        /// Returns true if x is greater than or equal to y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is greater than or equal to y</returns>
        [Pure]
        public static bool greaterOrEq<ORD, A>(A x, A y) where ORD : struct, Ord<A> =>
            default(ORD).Compare(x, y) >= 0;

        /// <summary>
        /// Returns true if x is less than y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is less than y</returns>
        [Pure]
        public static bool lessThan<ORD, A>(A x, A y) where ORD : struct, Ord<A> =>
            default(ORD).Compare(x, y) < 0;

        /// <summary>
        /// Returns true if x is less than or equal to y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is less than or equal to y</returns>
        [Pure]
        public static bool lessOrEq<ORD, A>(A x, A y) where ORD : struct, Ord<A> =>
            default(ORD).Compare(x, y) <= 0;

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
        public static int compare<ORD, A>(A x, A y) where ORD : struct, Ord<A> =>
            default(ORD).Compare(x, y);

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
        public static int compare<ORD, A>(Option<A> x, Option<A> y) where ORD : struct, Ord<A> =>
            OrdOpt<ORD, MOption<A>, Option<A>, A>.Inst.Compare(x, y);

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
        public static int compare<ORD, A>(OptionUnsafe<A> x, OptionUnsafe<A> y) where ORD : struct, Ord<A> =>
            OrdOpt<ORD, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Inst.Compare(x, y);

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
        public static int compare<ORDA, ORDB, A, B>(Either<A, B> x, Either<A, B> y)
            where ORDA : struct, Ord<A>
            where ORDB : struct, Ord<B> =>
            OrdChoice<ORDA, ORDB, MEither<A, B>, Either<A, B>, A, B>.Inst.Compare(x, y);

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
        public static int compare<ORDA, ORDB, A, B>(EitherUnsafe<A, B> x, EitherUnsafe<A, B> y)
            where ORDA : struct, Ord<A>
            where ORDB : struct, Ord<B> =>
            OrdChoice<ORDA, ORDB, MEitherUnsafe<A, B>, EitherUnsafe<A, B>, A, B>.Inst.Compare(x, y);

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
        public static int compare<MonoidA, ORDA, ORDB, A, B>(Validation<MonoidA, A, B> x, Validation<MonoidA, A, B> y)
            where MonoidA : struct, Monoid<A>, Eq<A>
            where ORDA : struct, Ord<A>
            where ORDB : struct, Ord<B> =>
            OrdChoice<ORDA, ORDB, FoldValidation<MonoidA, A, B>, Validation<MonoidA, A, B>, A, B>.Inst.Compare(x, y);

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
        public static int compare<ORD, A>(A[] x, A[] y)
            where ORD : struct, Ord<A> =>
            OrdArray<ORD, A>.Inst.Compare(x, y);

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
        public static int compare<ORD, A>(Lst<A> x, Lst<A> y)
            where ORD : struct, Ord<A> =>
            OrdLst<ORD, A>.Inst.Compare(x, y);

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
        public static int compare<NEWTYPE, ORD, T, PRED>(NEWTYPE x, NEWTYPE y)
            where ORD : struct, Ord<T>
            where PRED : struct, Pred<T>
            where NEWTYPE : NewType<NEWTYPE, T, PRED> =>
            OrdNewType<NEWTYPE, ORD, T, PRED>.Inst.Compare(x, y);

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
        public static int compare<NEWTYPE, NUM, T, PRED>(NEWTYPE x, NumType<NEWTYPE, NUM, T, PRED> y)
            where NUM : struct, Num<T>
            where PRED : struct, Pred<T>
            where NEWTYPE : NumType<NEWTYPE, NUM, T, PRED> =>
            OrdNumType<NEWTYPE, NUM, T, PRED>.Inst.Compare(x, y);

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
        public static int compare<ORD, A>(TryOption<A> x, TryOption<A> y) where ORD : struct, Ord<A> =>
            OrdOpt<ORD, MTryOption<A>, TryOption<A>, A>.Inst.Compare(x, y);

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
        public static int compare<ORD, A>(Try<A> x, Try<A> y) where ORD : struct, Ord<A> =>
            OrdOpt<ORD, MTry<A>, Try<A>, A>.Inst.Compare(x, y);

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
        public static int compare<ORD, L, R>(Either<L, R> x, Either<L, R> y) where ORD : struct, Ord<R> =>
            OrdChoice<ORD, MEither<L, R>, Either<L, R>, L, R>.Inst.Compare(x, y);

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
        public static int compare<ORD, L, R>(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) where ORD : struct, Ord<R> =>
            OrdChoice<ORD, MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>.Inst.Compare(x, y);

    }
}
