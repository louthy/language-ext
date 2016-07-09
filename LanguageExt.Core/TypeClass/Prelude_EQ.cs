using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClass
{
    public static partial class Prelude
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
            equals<EqOption<EQ, A>, Option<A>>(x, y);
    }
}
