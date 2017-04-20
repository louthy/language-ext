using System.Collections.Generic;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class TypeClass
    {
        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static bool Equals<EQ, A>(this A x, A y) where EQ : struct, Eq<A> =>
            default(EQ).Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static bool Equals<EQ, A>(this Arr<A> x, Arr<A> y) where EQ : struct, Eq<A> =>
            EqArr<EQ, A>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static bool Equals<EQ, A>(this HashSet<A> x, HashSet<A> y) where EQ : struct, Eq<A> =>
            EqHashSet<EQ, A>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static bool Equals<EQ, A>(this Que<A> x, Que<A> y) where EQ : struct, Eq<A> =>
            EqQue<EQ, A>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static bool Equals<EQ, A>(this Stck<A> x, Stck<A> y) where EQ : struct, Eq<A> =>
            EqStck<EQ, A>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static bool Equals<EQ, A>(this A[] x, A[] y) where EQ : struct, Eq<A> =>
            EqArray<EQ, A>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static bool Equals<EQ, A>(this Option<A> x, Option<A> y) where EQ : struct, Eq<A> =>
            EqOpt<EQ, MOption<A>, Option<A>, A>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static bool Equals<EQ, A>(this OptionUnsafe<A> x, OptionUnsafe<A> y) where EQ : struct, Eq<A> =>
            EqOpt<EQ, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static bool Equals<EQ, A>(this A? x, A? y) 
            where EQ : struct, Eq<A>
            where A  : struct =>
            EqOpt<EQ, MNullable<A>, A?, A>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static bool Equals<EQ, A>(this Try<A> x, Try<A> y) where EQ : struct, Eq<A> =>
            EqTry<EQ, A>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static bool Equals<EQ, A>(this TryOption<A> x, TryOption<A> y) where EQ : struct, Eq<A> =>
            EqTryOpt<EQ, A>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static bool Equals<EQ, A>(this Lst<A> x, Lst<A> y) where EQ : struct, Eq<A> =>
            EqLst<EQ, A>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static bool Equals<EQ, A>(this IEnumerable<A> x, IEnumerable<A> y) where EQ : struct, Eq<A> =>
            EqEnumerable<EQ, A>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static bool Equals<EQ, A>(this Seq<A> x, Seq<A> y) where EQ : struct, Eq<A> =>
            EqSeq<EQ, A>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static bool Equals<EQ, L, R>(this Either<L, R> x, Either<L, R> y) where EQ : struct, Eq<R> =>
            EqChoice<EQ, MEither<L, R>, Either<L, R>, L, R>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static bool Equals<EQ, L, R>(this EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) where EQ : struct, Eq<R> =>
            EqChoice<EQ, MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static bool Equals<EQA, EQB, A, B>(this Either<A, B> x, Either<A, B> y)
            where EQA : struct, Eq<A>
            where EQB : struct, Eq<B> =>
            EqChoice<EQA, EQB, MEither<A, B>, Either<A, B>, A, B>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static bool Equals<EQA, EQB, A, B>(this EitherUnsafe<A, B> x, EitherUnsafe<A, B> y)
            where EQA : struct, Eq<A>
            where EQB : struct, Eq<B> =>
            EqChoice<EQA, EQB, MEitherUnsafe<A, B>, EitherUnsafe<A, B>, A, B>.Inst.Equals(x, y);
    }
}
