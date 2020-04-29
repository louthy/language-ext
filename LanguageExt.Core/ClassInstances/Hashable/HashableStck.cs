using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Stack hashing
    /// </summary>
    public struct HashableStck<HashA, A> : Hashable<Stck<A>> where HashA : struct, Hashable<A>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Stck<A> x) =>
            Prelude.hash<HashA, A>(x);
    }

    /// <summary>
    /// Stack hashing
    /// </summary>
    public struct HashableStck<A> : Hashable<Stck<A>>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Stck<A> x) =>
            default(HashableStck<HashableDefault<A>, A>).GetHashCode(x);
    }
}
