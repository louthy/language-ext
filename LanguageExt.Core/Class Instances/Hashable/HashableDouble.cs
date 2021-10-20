using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Floating point hash
    /// </summary>
    public struct HashableDouble : Hashable<double>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(double x) =>
            x.GetHashCode();

        [Pure]
        public Task<int> GetHashCodeAsync(double x) =>
            GetHashCode(x).AsTask();
    }
}
