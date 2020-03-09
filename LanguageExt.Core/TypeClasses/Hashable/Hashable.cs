using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Attributes;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    /// <summary>
    /// Hashable type-class
    /// </summary>
    /// <typeparam name="A">
    /// The type for which GetHashCode is defined
    /// </typeparam>
    [Typeclass("Hashable*")]
    public interface Hashable<A> : HashableAsync<A>, Typeclass
    {
        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        int GetHashCode(A x);
    }
}
