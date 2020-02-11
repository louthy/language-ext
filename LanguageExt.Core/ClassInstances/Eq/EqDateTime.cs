using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// DateTime equality
    /// </summary>
    public struct EqDateTime : Eq<DateTime>
    {
        public static readonly EqDateTime Inst = default(EqDateTime);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(DateTime a, DateTime b)  => 
            a == b;

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(DateTime x) =>
            default(HashableDateTime).GetHashCode(x);
    }
}
