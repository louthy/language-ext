using LanguageExt.Instances;
using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static int compare<NEWTYPE, ORD, T>(NEWTYPE x, NEWTYPE y)
            where ORD : struct, Ord<T>
            where NEWTYPE : NewType<NEWTYPE, T> =>
            OrdNewType<NEWTYPE, ORD, T>.Inst.Compare(x, y);

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
        public static int compare<NEWTYPE, NUM, T>(NEWTYPE x, NewType<NEWTYPE, NUM, T> y)
            where NUM : struct, Num<T>
            where NEWTYPE : NewType<NEWTYPE, NUM, T> =>
            OrdNewTypeNum<NEWTYPE, NUM, T>.Inst.Compare(x, y);

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
        public static int compare<NEWTYPE, SEMI, ORD, T>(NEWTYPE x, NewType<NEWTYPE, SEMI, ORD, T> y)
            where SEMI : struct, Semigroup<T>
            where ORD : struct, Ord<T>
            where NEWTYPE : NewType<NEWTYPE, SEMI, ORD, T> =>
            OrdNewType<NEWTYPE, SEMI, ORD, T>.Inst.Compare(x, y);

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
