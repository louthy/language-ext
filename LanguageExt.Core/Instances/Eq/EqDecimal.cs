using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Floating point equality
    /// </summary>
    public struct EqDecimal : Eq<decimal>
    {
        public static readonly EqDecimal Inst = default(EqDecimal);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(decimal a, decimal b) { return a == b; }
    }
}
