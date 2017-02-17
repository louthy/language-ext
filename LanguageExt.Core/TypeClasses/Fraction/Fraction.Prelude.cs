using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class TypeClass
    {
        /// <summary>
        /// Generates a fractional value from an integer ratio.
        /// </summary>
        /// <param name="x">The ratio to convert</param>
        /// <returns>The equivalent of x in the implementing type.</returns>
        [Pure]
        public static A fromRational<FRACTION, A>(Ratio<int> x) where FRACTION : struct, Fraction<A> =>
            default(FRACTION).FromRational(x);
    }
}
