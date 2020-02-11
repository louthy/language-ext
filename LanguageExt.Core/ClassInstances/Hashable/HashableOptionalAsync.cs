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
    public struct HashableOptionalAsync<HashA, OPTION, OA, A> : HashableAsync<OA>
        where HashA  : struct, Hashable<A>
        where OPTION : struct, OptionalAsync<OA, A>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public async Task<int> GetHashCodeAsync(OA x) =>
            x.IsNull() 
                ? 0 
                : (await default(OPTION).Match(x, 
                        Some: default(HashA).GetHashCode, 
                        None: () => 0));
    }

    /// <summary>
    /// Hash of any type in the Optional type-class
    /// </summary>
    public struct HashableOptionalAsync<OPTION, OA, A> : HashableAsync<OA>
        where OPTION : struct, OptionalAsync<OA, A>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public Task<int> GetHashCodeAsync(OA x) =>
            default(HashableOptionalAsync<HashableDefault<A>, OPTION, OA, A>).GetHashCodeAsync(x);
    }

}
