using LanguageExt.Instances;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static partial class TypeClass
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public static bool equals<EQ, A>(A x, A y) where EQ : struct, Eq<A> =>
            default(EQ).Equals(x, y);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public static bool equals<EQ, A>(Option<A> x, Option<A> y) where EQ : struct, Eq<A> =>
            default(EqOpt<EQ, A>).Equals(x, y);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public static bool equals<EQ, A>(OptionUnsafe<A> x, OptionUnsafe<A> y) where EQ : struct, Eq<A> =>
            default(EqOpt<EQ, A>).Equals(x, y);
    }
}
