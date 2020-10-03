using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Globalization;
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

    /// <summary>
    /// Char equality (ordinal, ignore case)
    /// </summary>
    public struct EqCharOrdinalIgnoreCase : Eq<char>
    {
        public static readonly EqCharOrdinalIgnoreCase Inst = default(EqCharOrdinalIgnoreCase);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="a">The left hand side of the equality operation</param>
        /// <param name="b">The right hand side of the equality operation</param>
        /// <returns>True if a and b are equal</returns>
        [Pure]
        public bool Equals(char a, char b)
        {
            if (a >= 'a' && a <= 'z') a = (char)(a - 0x20);
            if (b >= 'a' && b <= 'z') b = (char)(b - 0x20);
            return a == b;
        }

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(char x) =>
            default(HashableCharOrdinalIgnoreCase).GetHashCode(x);

        [Pure]
        public Task<bool> EqualsAsync(char x, char y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(char x) =>
            GetHashCode(x).AsTask();
    }
}
