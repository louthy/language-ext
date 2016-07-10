using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Uses the standard .NET EqualityComparer<A>.Default.Equals(a,b) method to
    /// provide equality testing.
    /// </summary>
    public struct EqDefault<A> : Eq<A>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(A a, A b)
        {
            if (isnull(a)) return b == null;
            if (isnull(b)) return false;
            if (ReferenceEquals(a, b)) return true;
            return EqualityComparer<A>.Default.Equals(a, b);
        }
    }
}
