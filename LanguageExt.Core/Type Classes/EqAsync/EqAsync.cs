using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Equality type-class for asynchronous types
    /// </summary>
    /// <typeparam name="A">
    /// The type for which equality is defined
    /// </typeparam>
    [Typeclass("Eq*Async")]
    public interface EqAsync<A> : HashableAsync<A>, Typeclass
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        Task<bool> EqualsAsync(A x, A y);
    }
}
