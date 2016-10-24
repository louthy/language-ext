using LanguageExt.TypeClasses;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Integer equality
    /// </summary>
    public struct EqShort : Eq<short>
    {
        public static readonly EqShort Inst = default(EqShort);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(short a, short b) { return a == b; }
    }
}
