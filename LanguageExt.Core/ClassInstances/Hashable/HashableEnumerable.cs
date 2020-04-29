using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Enumerable hashing
    /// </summary>
    public struct HashableEnumerable<HashA, A> : Hashable<IEnumerable<A>>
        where HashA : struct, Hashable<A>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(IEnumerable<A> x) =>
            hash<HashA, A>(x);
    }

    /// <summary>
    /// Enumerable hashing
    /// </summary>
    public struct HashableEnumerable<A> : Hashable<IEnumerable<A>>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(IEnumerable<A> x) =>
            default(HashableEnumerable<HashableDefault<A>, A>).GetHashCode(x);
    }
}
