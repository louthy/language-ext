using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Identity equality
    /// </summary>
    public struct EqIdentity<A> : Eq<Identity<A>>
    {
        public static readonly EqIdentity<A> Inst = default(EqIdentity<A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(Identity<A> a, Identity<A> b)  => 
            a == b;

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Identity<A> x) =>
            default(HashableIdentity<A>).GetHashCode(x);
  
        [Pure]
        public Task<bool> EqualsAsync(Identity<A> x, Identity<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(Identity<A> x) => 
            GetHashCode(x).AsTask();      
    }
}
