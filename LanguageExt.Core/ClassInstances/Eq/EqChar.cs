using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Char equality
    /// </summary>
    public struct EqChar : Eq<char>
    {
        public static readonly EqChar Inst = default(EqChar);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(char a, char b) =>
            a == b;


        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(char x) =>
            default(HashableChar).GetHashCode(x);

        [Pure]
        public Task<bool> EqualsAsync(char x, char y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(char x) => 
            GetHashCode(x).AsTask();
    }
}
