using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Numerics;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Integer equality
    /// </summary>
    public struct EqBigInt : Eq<bigint>
    {
        public static readonly EqBigInt Inst = default(EqBigInt);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(bigint a, bigint b) =>
            a == b;


        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(bigint x) =>
            default(HashableBigInt).GetHashCode(x);
    }
}
