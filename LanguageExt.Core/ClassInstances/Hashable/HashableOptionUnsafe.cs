using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Option type hash
    /// </summary>
    public struct HashableOptionUnsafe<A> : Hashable<OptionUnsafe<A>>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        public int GetHashCode(OptionUnsafe<A> x) =>
            default(HashableOptionalUnsafe<MOptionUnsafe<A>, OptionUnsafe<A>, A>).GetHashCode(x);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public Task<int> GetHashCodeAsync(OptionUnsafe<A> x) =>
            GetHashCode(x).AsTask();
    }
}
