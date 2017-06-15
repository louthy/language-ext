using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Equality type-class
    /// </summary>
    /// <typeparam name="A">
    /// The type for which equality is defined
    /// </typeparam>
    [Typeclass]
    public interface Eq<A>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        bool Equals(A x, A y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        int GetHashCode(A x);
    }
}
