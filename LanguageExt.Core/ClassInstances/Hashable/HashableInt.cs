using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Integer hash
    /// </summary>
    public struct HashableInt : Hashable<int>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(int x) =>
            x.GetHashCode();

        [Pure]
        public Task<int> GetHashCodeAsync(int x) =>
            GetHashCode(x).AsTask();    
    }
}
