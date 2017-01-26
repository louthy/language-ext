using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Floating point equality
    /// </summary>
    public struct EqDouble : Eq<double>
    {
        public static readonly EqDouble Inst = default(EqDouble);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(double a, double b) { return a == b; }
    }
}
