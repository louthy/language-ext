using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Set<T> hashing
    /// </summary>
    public struct HashableSet<HashA, A> : Hashable<Set<A>> where HashA : struct, Hashable<A>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Set<A> x) =>
            Prelude.hash<HashA, A>(x);
    }

    /// <summary>
    /// Set<T> hashing
    /// </summary>
    public struct HashableSet<A> : Hashable<Set<A>>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Set<A> x) =>
            default(HashableSet<HashableDefault<A>, A>).GetHashCode(x);
    }
}
