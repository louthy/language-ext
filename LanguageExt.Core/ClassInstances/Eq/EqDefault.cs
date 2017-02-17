using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Uses the standard .NET EqualityComparer<A>.Default.Equals(a,b) method to
    /// provide equality testing.
    /// </summary>
    public struct EqDefault<A> : Eq<A>
    {
        public static readonly EqDefault<A> Inst = default(EqDefault<A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(A a, A b)
        {
            if (isnull(a)) return b == null;
            if (isnull(b)) return false;
            if (ReferenceEquals(a, b)) return true;
            return EqualityComparer<A>.Default.Equals(a, b);
        }

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(A x) =>
            x.IsNull() ? 0 : x.GetHashCode();
    }
}
