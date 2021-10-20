using LanguageExt;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Hash of any type in the Optional type-class
    /// </summary>
    public struct HashableOptional<HashA, OptionA, OA, A> : Hashable<OA>
        where HashA   : struct, Hashable<A>
        where OptionA : struct, Optional<OA, A>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(OA x) =>
            default(OptionA).Match(x, default(HashA).GetHashCode, 0);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public Task<int> GetHashCodeAsync(OA x) =>
            GetHashCode(x).AsTask();
    }

    /// <summary>
    /// Hash of any type in the Optional type-class
    /// </summary>
    public struct HashableOptional<OptionA, OA, A> : Hashable<OA>
        where OptionA : struct, Optional<OA, A>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(OA x) =>
            default(HashableOptional<HashableDefault<A>, OptionA, OA, A>).GetHashCode(x);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public Task<int> GetHashCodeAsync(OA x) =>
            GetHashCode(x).AsTask();
    }
}
