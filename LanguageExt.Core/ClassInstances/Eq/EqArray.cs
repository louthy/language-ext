using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Array equality
    /// </summary>
    public struct EqArray<EqA, A> : Eq<A[]> where EqA : struct, Eq<A>
    {
        public static readonly EqArray<EqA, A> Inst = default(EqArray<EqA, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(A[] x, A[] y)
        {
            if (x == null) return y == null;
            if (y == null) return false;
            if (x.Length != y.Length) return false;

            for (var i = 0; i < x.Length; i++)
            {
                if (!equals<EqA, A>(x[i], y[i])) return false;
            }
            return true;
        }

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(A[] x) =>
            default(HashableArray<EqA, A>).GetHashCode(x);
        
        [Pure]
        public Task<bool> EqualsAsync(A[] x, A[] y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(A[] x) => 
            GetHashCode(x).AsTask();
    }

    /// <summary>
    /// Array equality
    /// </summary>
    public struct EqArray<A> : Eq<A[]>
    {
        public static readonly EqArray<A> Inst = default(EqArray<A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(A[] x, A[] y) =>
            default(EqArray<EqDefault<A>, A>).Equals(x, y);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(A[] x) =>
            default(HashableArray<HashableDefault<A>, A>).GetHashCode(x);
        
        [Pure]
        public Task<bool> EqualsAsync(A[] x, A[] y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(A[] x) => 
            GetHashCode(x).AsTask();
    }
}
