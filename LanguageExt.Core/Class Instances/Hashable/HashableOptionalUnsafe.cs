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
    public struct HashableOptionalUnsafe<HashA, OPTION, OA, A> : Hashable<OA>
        where HashA  : struct, Hashable<A>
        where OPTION : struct, OptionalUnsafe<OA, A>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(OA x) =>
            default(OPTION).MatchUnsafe(x, default(HashA).GetHashCode, 0);

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
    public struct HashableOptionalUnsafe<OPTION, OA, A> : Hashable<OA>
        where OPTION : struct, OptionalUnsafe<OA, A>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(OA x) =>
            default(HashableOptionalUnsafe<HashableDefault<A>, OPTION, OA, A>).GetHashCode(x);

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
