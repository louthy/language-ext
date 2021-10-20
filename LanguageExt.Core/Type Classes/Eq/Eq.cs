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
    [Typeclass("Eq*")]
    public interface Eq<A> : Hashable<A>, EqAsync<A>, Typeclass
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        bool Equals(A x, A y);
    }
}
