using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// HashSet hash
    /// </summary>
    public struct HashableHashSet<HashA, A> : Hashable<HashSet<A>> where HashA : struct, Hashable<A>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(HashSet<A> x) =>
            hash<HashA, A>(x);

        [Pure]
        public Task<int> GetHashCodeAsync(HashSet<A> x) =>
            GetHashCode(x).AsTask();
    }

    /// <summary>
    /// HashSet hash
    /// </summary>
    public struct HashableHashSet<A> : Hashable<HashSet<A>>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(HashSet<A> x) =>
            default(HashableHashSet<HashableDefault<A>, A>).GetHashCode(x);

        [Pure]
        public Task<int> GetHashCodeAsync(HashSet<A> x) =>
            GetHashCode(x).AsTask();
    }
}
