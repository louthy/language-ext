using LanguageExt;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Hash of any type in the Either type-class
    /// </summary>
    public struct HashableChoice<HashA, HashB, CHOICE, CH, A, B> : Hashable<CH>
        where CHOICE : struct, Choice<CH, A, B>
        where HashA  : struct, Hashable<A>
        where HashB  : struct, Hashable<B>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(CH x) =>
            default(CHOICE).Match(x,
                Left: default(HashA).GetHashCode,
                Right: default(HashB).GetHashCode);

        [Pure]
        public Task<int> GetHashCodeAsync(CH x) =>
            GetHashCode(x).AsTask();
    }

    /// <summary>
    /// Hash of any type in the Either type-class
    /// </summary>
    public struct HashableChoice<HashB, CHOICE, CH, A, B> : Hashable<CH>
        where CHOICE : struct, Choice<CH, A, B>
        where HashB : struct, Hashable<B>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(CH x) =>
            default(HashableChoice<EqDefault<A>, HashB, CHOICE, CH, A, B>).GetHashCode(x);
    
        [Pure]
        public Task<int> GetHashCodeAsync(CH x) =>
            GetHashCode(x).AsTask();
    }

    /// <summary>
    /// Hash of any type in the Either type-class
    /// </summary>
    public struct HashableChoice<CHOICE, CH, A, B> : Hashable<CH>
        where CHOICE : struct, Choice<CH, A, B>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(CH x) =>
            default(HashableChoice<HashableDefault<A>, HashableDefault<B>, CHOICE, CH, A, B>).GetHashCode(x);

        [Pure]
        public Task<int> GetHashCodeAsync(CH x) =>
            GetHashCode(x).AsTask();
    }
}
