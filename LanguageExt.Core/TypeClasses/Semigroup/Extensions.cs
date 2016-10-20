using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using System.Collections.Generic;

namespace LanguageExt
{
    public static partial class TypeClassExtensions
    {
        /// <summary>
        /// An associative binary operation
        /// </summary>
        /// <param name="x">The left hand side of the operation</param>
        /// <param name="y">The right hand side of the operation</param>
        /// <returns>The result of the operation</returns>
        [Pure]
        public static A Append<SEMI, A>(this A x, A y) where SEMI : struct, Semigroup<A> =>
            default(SEMI).Append(x, y);

        /// <summary>
        /// An associative binary operation
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs + rhs</returns>
        [Pure]
        public static Either<L, R> Append<SEMI, L, R>(this Either<L, R> lhs, Either<L, R> rhs) where SEMI : struct, Semigroup<R> =>
            from x in lhs
            from y in rhs
            select default(SEMI).Append(x, y);

        /// <summary>
        /// An associative binary operation
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs + rhs</returns>
        [Pure]
        public static EitherUnsafe<L, R> Append<SEMI, L, R>(this EitherUnsafe<L, R> lhs, EitherUnsafe<L, R> rhs) where SEMI : struct, Semigroup<R> =>
            from x in lhs
            from y in rhs
            select default(SEMI).Append(x, y);

        /// <summary>
        /// An associative binary operation
        /// </summary>
        /// <param name="x">The left hand side of the operation</param>
        /// <param name="y">The right hand side of the operation</param>
        /// <returns>The result of the operation</returns>
        [Pure]
        public static HMap<K, V> Append<K, V>(this HMap<K, V> x, HMap<K, V> y) =>
            default(HMap<K, V>).Append(x, y);

        /// <summary>
        /// An associative binary operation
        /// </summary>
        /// <param name="x">The left hand side of the operation</param>
        /// <param name="y">The right hand side of the operation</param>
        /// <returns>The result of the operation</returns>
        [Pure]
        public static HSet<A> Append<A>(this HSet<A> x, HSet<A> y) =>
            default(HSet<A>).Append(x, y);

        /// <summary>
        /// An associative binary operation
        /// </summary>
        /// <param name="x">The left hand side of the operation</param>
        /// <param name="y">The right hand side of the operation</param>
        /// <returns>The result of the operation</returns>
        [Pure]
        public static Lst<A> Append<A>(this Lst<A> x, Lst<A> y) =>
            default(Lst<A>).Append(x, y);

        /// <summary>
        /// An associative binary operation
        /// </summary>
        /// <param name="x">The left hand side of the operation</param>
        /// <param name="y">The right hand side of the operation</param>
        /// <returns>The result of the operation</returns>
        [Pure]
        public static Map<K, V> Append<K, V>(this Map<K, V> x, Map<K, V> y) =>
            default(Map<K, V>).Append(x, y);

        /// <summary>
        /// An associative binary operation
        /// </summary>
        /// <param name="x">The left hand side of the operation</param>
        /// <param name="y">The right hand side of the operation</param>
        /// <returns>The result of the operation</returns>
        [Pure]
        public static NEWTYPE Append<NEWTYPE, SEMI, A>(this NewType<NEWTYPE, A> x, NewType<NEWTYPE, A> y)
            where NEWTYPE : NewType<NEWTYPE, A>
            where SEMI : struct, Semigroup<A> =>
            from a in x
            from b in y
            select default(SEMI).Append(a, b);

        /// <summary>
        /// An associative binary operation
        /// </summary>
        /// <param name="x">The left hand side of the operation</param>
        /// <param name="y">The right hand side of the operation</param>
        /// <returns>The result of the operation</returns>
        [Pure]
        public static NEWTYPE Append<NEWTYPE, SEMI, ORD, A>(this NewType<NEWTYPE, SEMI, ORD, A> x, NewType<NEWTYPE, SEMI, ORD, A> y)
            where NEWTYPE : NewType<NEWTYPE, SEMI, ORD, A>
            where ORD : struct, Ord<A>
            where SEMI : struct, Semigroup<A> =>
            from a in x
            from b in y
            select default(SEMI).Append(a, b);

        /// <summary>
        /// An associative binary operation
        /// </summary>
        /// <param name="x">The left hand side of the operation</param>
        /// <param name="y">The right hand side of the operation</param>
        /// <returns>The result of the operation</returns>
        [Pure]
        public static NEWTYPE Append<NEWTYPE, NUM, A>(this NewType<NEWTYPE, NUM, A> x, NewType<NEWTYPE, NUM, A> y)
            where NEWTYPE : NewType<NEWTYPE, NUM, A>
            where NUM : struct, Num<A> =>
            from a in x
            from b in y
            select default(NUM).Append(a, b);

        /// <summary>
        /// An associative binary operation
        /// </summary>
        /// <param name="x">The left hand side of the operation</param>
        /// <param name="y">The right hand side of the operation</param>
        /// <returns>The result of the operation</returns>
        [Pure]
        public static Option<A> Append<SEMI, A>(this Option<A> x, Option<A> y) where SEMI : struct, Semigroup<A> =>
            from a in x
            from b in y
            select default(SEMI).Append(a, b);

        /// <summary>
        /// An associative binary operation
        /// </summary>
        /// <param name="x">The left hand side of the operation</param>
        /// <param name="y">The right hand side of the operation</param>
        /// <returns>The result of the operation</returns>
        [Pure]
        public static OptionUnsafe<A> Append<SEMI, A>(this OptionUnsafe<A> x, OptionUnsafe<A> y) where SEMI : struct, Semigroup<A> =>
            from a in x
            from b in y
            select default(SEMI).Append(a, b);

        /// <summary>
        /// An associative binary operation
        /// </summary>
        /// <param name="x">The left hand side of the operation</param>
        /// <param name="y">The right hand side of the operation</param>
        /// <returns>The result of the operation</returns>
        [Pure]
        public static Seq<A> Append<SEMI, A>(this Seq<A> x, Seq<A> y) where SEMI : struct, Semigroup<A> =>
            from a in x
            from b in y
            select default(SEMI).Append(a, b);

        /// <summary>
        /// An associative binary operation
        /// </summary>
        /// <param name="x">The left hand side of the operation</param>
        /// <param name="y">The right hand side of the operation</param>
        /// <returns>The result of the operation</returns>
        [Pure]
        public static Try<A> Append<SEMI, A>(this Try<A> x, Try<A> y) where SEMI : struct, Semigroup<A> =>
            from a in x
            from b in y
            select default(SEMI).Append(a, b);

        /// <summary>
        /// An associative binary operation
        /// </summary>
        /// <param name="x">The left hand side of the operation</param>
        /// <param name="y">The right hand side of the operation</param>
        /// <returns>The result of the operation</returns>
        [Pure]
        public static TryOption<A> Append<SEMI, A>(this TryOption<A> x, TryOption<A> y) where SEMI : struct, Semigroup<A> =>
            from a in x
            from b in y
            select default(SEMI).Append(a, b);

        /// <summary>
        /// Concatenate two enumerables (Concat in LINQ)
        /// </summary>
        /// <typeparam name="A">Enumerable item type</typeparam>
        /// <param name="lhs">First enumerable</param>
        /// <param name="rhs">Second enumerable</param>
        /// <returns>Concatenated enumerable</returns>
        [Pure]
        public static IEnumerable<A> Append<A>(this IEnumerable<A> lhs, IEnumerable<A> rhs) =>
            List.append(lhs, rhs);
    }
}
