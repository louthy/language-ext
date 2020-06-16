using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Guid hash
    /// </summary>
    public struct HashableGuid : Hashable<Guid>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Guid x) =>
            x.GetHashCode();

        [Pure]
        public Task<int> GetHashCodeAsync(Guid x) =>
            GetHashCode(x).AsTask();
    }
}
