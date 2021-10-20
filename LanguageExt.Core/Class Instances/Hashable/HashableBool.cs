using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Boolean hash
    /// </summary>
    public struct HashableBool : Hashable<bool>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(bool x) =>
            x.GetHashCode();

        [Pure]
        public Task<int> GetHashCodeAsync(bool x) =>
            GetHashCode(x).AsTask();
    }
}
