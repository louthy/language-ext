using LanguageExt.Instances;
using LanguageExt.TypeClasses;
using System.Collections.Generic;

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
        public static bool equals<EQ, A>(A x, A y) where EQ : struct, Eq<A> =>
            default(EQ).Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public static bool equals<EQ, A>(Option<A> x, Option<A> y) where EQ : struct, Eq<A> =>
            default(EqOpt<EQ, MOption<A>, Option<A>, A>).Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public static bool equals<EQ, A>(OptionUnsafe<A> x, OptionUnsafe<A> y) where EQ : struct, Eq<A> =>
            default(EqOpt<EQ, MOptionUnsafe<A>, OptionUnsafe<A>, A>).Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public static bool equals<EQ, A>(A? x, A? y)
            where EQ : struct, Eq<A>
            where A : struct =>
            default(EqOpt<EQ, MNullable<A>, A?, A>).Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public static bool equals<EQ, A>(Try<A> x, Try<A> y) where EQ : struct, Eq<A> =>
            default(EqTry<EQ, A>).Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public static bool equals<EQ, A>(TryOption<A> x, TryOption<A> y) where EQ : struct, Eq<A> =>
            default(EqTryOpt<EQ, A>).Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public static bool equals<EQ, A>(Lst<A> x, Lst<A> y) where EQ : struct, Eq<A> =>
            default(EqLst<EQ, A>).Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public static bool equals<EQ, A>(IEnumerable<A> x, IEnumerable<A> y) where EQ : struct, Eq<A> =>
            default(EqSeq<EQ, A>).Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public static bool equals<NEWTYPE, EQ, A>(NewType<NEWTYPE, A> x, NewType<NEWTYPE, A> y) 
            where EQ      : struct, Eq<A>
            where NEWTYPE : NewType<NEWTYPE, A>
            =>
            !ReferenceEquals(x, y) || ReferenceEquals(x, null) || ReferenceEquals(y, null) 
                ? false 
                : default(EQ).Equals(x.Value, y.Value);
    }
}
