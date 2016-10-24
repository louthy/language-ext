using LanguageExt.Instances;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class TypeClass
    {
        /// <summary>
        /// An associative binary operation
        /// </summary>
        /// <param name="x">The left hand side of the operation</param>
        /// <param name="y">The right hand side of the operation</param>
        /// <returns>The result of the operation</returns>
        [Pure]
        public static A append<SEMI, A>(A x, A y) where SEMI : struct, Semigroup<A> =>
            default(SEMI).Append(x, y);

        /// <summary>
        /// An associative binary operation
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs + rhs</returns>
        [Pure]
        public static Either<L, R> append<SEMI, L, R>(Either<L, R> lhs, Either<L, R> rhs) where SEMI : struct, Semigroup<R> =>
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
        public static EitherUnsafe<L, R> append<SEMI, L, R>(EitherUnsafe<L, R> lhs, EitherUnsafe<L, R> rhs) where SEMI : struct, Semigroup<R> =>
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
        public static HashMap<K, V> append<K, V>(HashMap<K, V> x, HashMap<K, V> y) =>
            MHashMap<K, V>.Inst.Append(x, y);

        /// <summary>
        /// An associative binary operation
        /// </summary>
        /// <param name="x">The left hand side of the operation</param>
        /// <param name="y">The right hand side of the operation</param>
        /// <returns>The result of the operation</returns>
        [Pure]
        public static HashSet<A> append<A>(HashSet<A> x, HashSet<A> y) =>
            MHashSet<A>.Inst.Append(x, y);

        /// <summary>
        /// An associative binary operation
        /// </summary>
        /// <param name="x">The left hand side of the operation</param>
        /// <param name="y">The right hand side of the operation</param>
        /// <returns>The result of the operation</returns>
        [Pure]
        public static Lst<A> append<A>(Lst<A> x, Lst<A> y) =>
            MLst<A>.Inst.Append(x, y);

        /// <summary>
        /// An associative binary operation
        /// </summary>
        /// <param name="x">The left hand side of the operation</param>
        /// <param name="y">The right hand side of the operation</param>
        /// <returns>The result of the operation</returns>
        [Pure]
        public static Map<K, V> append<K, V>(Map<K, V> x, Map<K, V> y) =>
            MMap<K, V>.Inst.Append(x, y);

        /// <summary>
        /// An associative binary operation
        /// </summary>
        /// <param name="x">The left hand side of the operation</param>
        /// <param name="y">The right hand side of the operation</param>
        /// <returns>The result of the operation</returns>
        [Pure]
        public static NEWTYPE append<NEWTYPE, SEMI, A>(NewType<NEWTYPE, A> x, NewType<NEWTYPE, A> y)
            where NEWTYPE : NewType<NEWTYPE, A>
            where SEMI    : struct, Semigroup<A> =>
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
        public static NEWTYPE append<NEWTYPE, SEMI, ORD, A>(NewType<NEWTYPE, SEMI, ORD, A> x, NewType<NEWTYPE, SEMI, ORD, A> y)
            where NEWTYPE : NewType<NEWTYPE, SEMI, ORD, A>
            where ORD     : struct, Ord<A>
            where SEMI    : struct, Semigroup<A> =>
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
        public static NEWTYPE append<NEWTYPE, NUM, A>(NewType<NEWTYPE, NUM, A> x, NewType<NEWTYPE, NUM, A> y)
            where NEWTYPE : NewType<NEWTYPE, NUM, A>
            where NUM     : struct, Num<A> =>
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
        public static Option<A> append<SEMI, A>(Option<A> x, Option<A> y) where SEMI : struct, Semigroup<A> =>
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
        public static OptionUnsafe<A> append<SEMI, A>(OptionUnsafe<A> x, OptionUnsafe<A> y) where SEMI : struct, Semigroup<A> =>
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
        public static IEnumerable<A> append<SEMI, A>(IEnumerable<A> x, IEnumerable<A> y) where SEMI : struct, Semigroup<A>
        {
            foreach (var a in x)
                foreach (var b in y)
                    yield return default(SEMI).Append(a, b);
        }

        /// <summary>
        /// An associative binary operation
        /// </summary>
        /// <param name="x">The left hand side of the operation</param>
        /// <param name="y">The right hand side of the operation</param>
        /// <returns>The result of the operation</returns>
        [Pure]
        public static Try<A> append<SEMI, A>(Try<A> x, Try<A> y) where SEMI : struct, Semigroup<A> =>
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
        public static TryOption<A> append<SEMI, A>(TryOption<A> x, TryOption<A> y) where SEMI : struct, Semigroup<A> =>
            from a in x
            from b in y
            select default(SEMI).Append(a, b);
    }
}
