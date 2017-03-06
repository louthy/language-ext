using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
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
        public A Numerator;

        /// <summary>
        /// The denominator of the ratio, in non-reduced form.
        /// </summary>
        public A Denominator;

        public Ratio(A num, A den)
        {
            Numerator = num;
            Denominator = den;
        }
    }
}
