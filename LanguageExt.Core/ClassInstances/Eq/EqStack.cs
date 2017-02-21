using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Que<A> equality
    /// </summary>
    public struct EqStack<A> : Eq<Stck<A>>
    {
        public static readonly EqStack<A> Inst = default(EqStack<A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(Stck<A> x, Stck<A> y)
        {
            if (x.Count != y.Count) return false;
            return x == y;
        }

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Stck<A> x) =>
            x.IsNull() ? 0 : x.GetHashCode();
    }
}
