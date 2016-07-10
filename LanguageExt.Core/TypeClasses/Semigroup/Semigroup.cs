using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClasses
{
    public interface Semigroup<A>
    {
        /// <summary>
        /// An associative binary operation.
        /// </summary>
        /// <param name="x">The first operand to the operation</param>
        /// <param name="y">The second operand to the operation</param>
        /// <returns>The result of the operation</returns>
        A Append(A x, A y);
    }
}
