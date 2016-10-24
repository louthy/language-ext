using System.Collections.Generic;
using LanguageExt.TypeClasses;

namespace LanguageExt.Instances
{
    /// <summary>
    /// String point equality
    /// </summary>
    public struct EqString : Eq<string>
    {
        public static readonly EqString Inst = default(EqString);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(string a, string b) { return a == b; }
    }
}
