using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Que<A> equality
    /// </summary>
    public struct EqQueue<A> : Eq<Que<A>>
    {
        public static readonly EqQueue<A> Inst = default(EqQueue<A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(Que<A> x, Que<A> y)
        {
            if (x == null) return y == null;
            if (y == null) return false;
            if (x.Count != y.Count) return false;
            return x == y;
        }

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Que<A> x) =>
            x.IsNull() ? 0 : x.GetHashCode();
    }
}
