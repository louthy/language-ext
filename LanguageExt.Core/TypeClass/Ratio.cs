using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClass
{
    /// <summary>
    /// A ratio between two values.
    /// </summary>
    /// <remarks>
    /// This is used in the definition of Fractional.
    /// </remarks>
    public struct Ratio<A>
    {
        /// <summary>
        /// The numerator of the ratio, in non-reduced form.
        /// </summary>
        public A num;

        /// <summary>
        /// The denominator of the ratio, in non-reduced form.
        /// </summary>
        public A den;
    }
}
