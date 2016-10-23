using LanguageExt.Instances;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace LanguageExt
{
    public static partial class TypeClassExtensions
    {
        /// <summary>
        /// Returns true if x is greater than y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is greater than y
        [Pure]
        public static bool GreaterThan<A>(this Ord<A> ord, A x, A y) =>
            ord.Compare(x, y) > 0;

        /// <summary>
        /// Returns true if x is greater than or equal to y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is greater than or equal to y
        [Pure]
        public static bool GreaterOrEq<A>(this Ord<A> ord, A x, A y) =>
            ord.Compare(x,y) >= 0;

        /// <summary>
        /// Returns true if x is less than y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is less than y
        [Pure]
        public static bool LessThan<A>(this Ord<A> ord, A x, A y) =>
            ord.Compare(x, y) < 0;

        /// <summary>
        /// Returns true if x is less than or equal to y
        /// </summary>
        /// <param name="x">The first item to compare</param>
        /// <param name="y">The second item to compare</param>
        /// <returns>True if x is less than or equal to y
        [Pure]
        public static bool LessOrEq<A>(this Ord<A> ord, A x, A y) =>
            ord.Compare(x, y) <= 0;

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
        public static int Compare<ORD, A>(this Option<A> x, Option<A> y) where ORD : struct, Ord<A> =>
            default(OrdOpt<ORD, MOption<A>, Option<A>, A>).Compare(x, y);

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
        public static int Compare<ORD, A>(this OptionUnsafe<A> x, OptionUnsafe<A> y) where ORD : struct, Ord<A> =>
            default(OrdOpt<ORD, MOptionUnsafe<A>, OptionUnsafe<A>, A>).Compare(x, y);

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
        public static int Compare<ORD, A>(A? x, A? y)
            where ORD : struct, Ord<A>
            where A   : struct =>
            default(OrdOpt<ORD, MNullable<A>, A?, A>).Compare(x, y);

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
        public static int Compare<ORDA, ORDB, A, B>(this Either<A, B> x, Either<A, B> y)
            where ORDA : struct, Ord<A>
            where ORDB : struct, Ord<B> =>
            default(OrdChoice<ORDA, ORDB, MEither<A, B>, Either<A, B>, A, B>).Compare(x, y);

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
        public static int Compare<ORDA, ORDB, A, B>(this EitherUnsafe<A, B> x, EitherUnsafe<A, B> y)
            where ORDA : struct, Ord<A>
            where ORDB : struct, Ord<B> =>
            default(OrdChoice<ORDA, ORDB, MEitherUnsafe<A, B>, EitherUnsafe<A, B>, A, B>).Compare(x, y);

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
        public static int Compare<ORD, A>(this A[] x, A[] y)
            where ORD : struct, Ord<A> =>
            default(OrdArray<ORD, A>).Compare(x, y);

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
        public static int Compare<ORD, A>(this Lst<A> x, Lst<A> y)
            where ORD : struct, Ord<A> =>
            default(OrdLst<ORD, A>).Compare(x, y);

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
        public static int Compare<NEWTYPE, ORD, T>(this NewType<NEWTYPE, T> x, NEWTYPE y)
            where ORD : struct, Ord<T>
            where NEWTYPE : NewType<NEWTYPE, T> =>
            default(OrdNewType<NEWTYPE, ORD, T>).Compare(x, y);

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
        public static int Compare<NEWTYPE, NUM, T>(this NewType<NEWTYPE, NUM, T> x, NEWTYPE y)
            where NUM : struct, Num<T>
            where NEWTYPE : NewType<NEWTYPE, NUM, T> =>
            default(OrdNewTypeNum<NEWTYPE, NUM, T>).Compare(x, y);

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
        public static int Compare<NEWTYPE, SEMI, ORD, T>(this NewType<NEWTYPE, SEMI, ORD, T> x, NEWTYPE y)
            where SEMI : struct, Semigroup<T>
            where ORD : struct, Ord<T>
            where NEWTYPE : NewType<NEWTYPE, SEMI, ORD, T> =>
            default(OrdNewType<NEWTYPE, SEMI, ORD, T>).Compare(x, y);

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
        public static int Compare<ORD, A>(this TryOption<A> x, TryOption<A> y) where ORD : struct, Ord<A> =>
            compare<ORD, A>(x, y);

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
        public static int Compare<ORD, A>(this Try<A> x, Try<A> y) where ORD : struct, Ord<A> =>
            compare<ORD, A>(x, y);
    }
}
