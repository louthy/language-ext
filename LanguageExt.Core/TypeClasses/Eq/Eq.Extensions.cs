using System.Collections.Generic;
using LanguageExt.Instances;
using LanguageExt.TypeClasses;

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
        public static bool Equals<EQ, A>(this A x, A y) where EQ : struct, Eq<A> =>
            default(EQ).Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public static bool Equals<EQ, A>(this Option<A> x, Option<A> y) where EQ : struct, Eq<A> =>
            EqOpt<EQ, MOption<A>, Option<A>, A>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public static bool Equals<EQ, A>(this OptionUnsafe<A> x, OptionUnsafe<A> y) where EQ : struct, Eq<A> =>
            EqOpt<EQ, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
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
        public static bool Equals<EQ, A>(this Try<A> x, Try<A> y) where EQ : struct, Eq<A> =>
            EqTry<EQ, A>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public static bool Equals<EQ, A>(this TryOption<A> x, TryOption<A> y) where EQ : struct, Eq<A> =>
            EqTryOpt<EQ, A>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public static bool Equals<EQ, A>(this Lst<A> x, Lst<A> y) where EQ : struct, Eq<A> =>
            EqLst<EQ, A>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public static bool Equals<EQ, A>(this IEnumerable<A> x, IEnumerable<A> y) where EQ : struct, Eq<A> =>
            EqSeq<EQ, A>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public static bool Equals<EQA, EQB, A, B>(this Either<A, B> x, Either<A, B> y)
            where EQA : struct, Eq<A>
            where EQB : struct, Eq<B> =>
            EqChoice<EQA, EQB, MEither<A, B>, Either<A, B>, A, B>.Inst.Equals(x, y);
    }
}
