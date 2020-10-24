using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Char hash
    /// </summary>
    public struct HashableChar : Hashable<char>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(char x) =>
            x.GetHashCode();

        [Pure]
        public Task<int> GetHashCodeAsync(char x) =>
            GetHashCode(x).AsTask();
    }

    /// <summary>
    /// Char hash (ordinal, ignore case)
    /// </summary>
    public struct HashableCharOrdinalIgnoreCase : Hashable<char>
    {
        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(char x) =>
            (x >= 'a' && x <= 'z') ? x - 0x20 : x;

        [Pure]
        public Task<int> GetHashCodeAsync(char x) =>
            GetHashCode(x).AsTask();
    }
}
