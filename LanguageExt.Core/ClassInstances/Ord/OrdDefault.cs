using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Uses the standard .NET  Comparer<A>.Default.Compare(a,b) method to
    /// provide equality testing.
    /// </summary>
    public struct OrdDefault<A> : Ord<A>
    {
        public static readonly OrdDefault<A> Inst = default(OrdDefault<A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public int Compare(A x, A y) =>
            Comparer<A>.Default.Compare(x, y);

        [Pure]
        public bool Equals(A x, A y) =>
            default(EqDefault<A>).Equals(x, y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(A x) =>
            x.IsNull() ? 0 : x.GetHashCode();
    }
}
