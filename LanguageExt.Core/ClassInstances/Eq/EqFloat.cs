using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Floating point equality
    /// </summary>
    public struct EqFloat : Eq<float>
    {
        public static readonly EqFloat Inst = default(EqFloat);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(float a, float b) { return a == b; }
    }
}
