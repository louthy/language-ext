using LanguageExt.Attributes;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.TypeClasses
{
    [Typeclass("Ord*Async")]
    public interface OrdAsync<A> : EqAsync<A>, Typeclass
    {
        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if x greater than y : 1
        /// 
        /// if x less than y    : -1
        /// 
        /// if x equals y       : 0
        /// </returns>
        [Pure]
        Task<int> CompareAsync(A x, A y);
    }
}
