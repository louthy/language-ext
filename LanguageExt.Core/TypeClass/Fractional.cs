using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClass
{
    /// <summary>
    /// Fractional number type-class
    /// </summary>
    /// <typeparam name="A">The type for which fractional 
    /// operations are being defined.</typeparam>
    public interface Fractional<A> : Num<A>
    {
        /// <summary>
        /// Fractionally divides x by y
        /// </summary>
        /// <param name="x">The dividend to be divided by y</param>
        /// <param name="y">The divisor to divide x</param>
        /// <returns>The result of fractionally dividing x by y</returns>
        A Div(A x, A y);

        /// <summary>
        /// Generates a fractional value from an integer ratio.
        /// </summary>
        /// <param name="x">The ratio to convert</param>
        /// <returns>The equivalent of x in the implementing type.</returns>
        A FromRational(Ratio<int> x);
    }
}
