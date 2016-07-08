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
        /// An associative binary operation
        /// </summary>
        /// <param name="x">The left hand side of the operation</param>
        /// <param name="y">The right hand side of the operation</param>
        /// <returns>The result of the operation</returns>
        public static A append<SEMI, A>(A x, A y) where SEMI : struct, Semigroup<A> =>
            default(SEMI).Append(x, y);
    }
}
