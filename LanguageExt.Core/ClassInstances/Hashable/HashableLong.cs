using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Integer hash
    /// </summary>
    public struct HashableLong : Hashable<long>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(long x) =>
            x.GetHashCode();
    }
}
