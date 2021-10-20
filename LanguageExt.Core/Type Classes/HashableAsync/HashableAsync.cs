using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Attributes;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    /// <summary>
    /// HashableAsync type-class
    /// </summary>
    /// <typeparam name="A">
    /// The type for which GetHashCodeAsync is defined
    /// </typeparam>
    [Typeclass("Hashable*Async")]
    public interface HashableAsync<A> : Typeclass
    {
        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        Task<int> GetHashCodeAsync(A x);
    }
}
