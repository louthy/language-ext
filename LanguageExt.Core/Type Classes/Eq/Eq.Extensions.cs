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
        public static bool Equals<EQ, A>(this A[] x, A[] y) where EQ : struct, Eq<A> =>
            EqArray<EQ, A>.Inst.Equals(x, y);

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
            EqOptional<EQ, MNullable<A>, A?, A>.Inst.Equals(x, y);

        /// <summary>
        /// Structural equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static bool Equals<EQ, A>(this IEnumerable<A> x, IEnumerable<A> y) where EQ : struct, Eq<A> =>
            EqEnumerable<EQ, A>.Inst.Equals(x, y);
    }
}
