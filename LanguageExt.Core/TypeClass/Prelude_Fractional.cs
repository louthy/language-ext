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
        /// Fractionally divides x by y
        /// </summary>
        /// <param name="x">The dividend to be divided by y</param>
        /// <param name="y">The divisor to divide x</param>
        /// <returns>The result of fractionally dividing x by y</returns>
        public static A div<FRACTION, A>(A x, A y) where FRACTION : struct, Fractional<A> =>
            default(FRACTION).Div(x, y);

        /// <summary>
        /// Generates a fractional value from an integer ratio.
        /// </summary>
        /// <param name="x">The ratio to convert</param>
        /// <returns>The equivalent of x in the implementing type.</returns>
        public static A fromRational<FRACTION, A>(Ratio<int> x) where FRACTION : struct, Fractional<A> =>
            default(FRACTION).FromRational(x);
    }
}
