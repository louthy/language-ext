using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Floating point hash
    /// </summary>
    public struct HashableDecimal : Hashable<decimal>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(decimal x) =>
            x.GetHashCode();

        [Pure]
        public Task<int> GetHashCodeAsync(decimal x) =>
            GetHashCode(x).AsTask();
    }
}
