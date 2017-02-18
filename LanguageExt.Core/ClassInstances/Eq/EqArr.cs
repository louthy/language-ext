using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Array equality
    /// </summary>
    public struct EqArr<EQ, A> : Eq<Arr<A>> where EQ : struct, Eq<A>
    {
        public static readonly EqArr<EQ, A> Inst = default(EqArr<EQ, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(Arr<A> x, Arr<A> y)
        {
            if (x.Count != y.Count) return false;
            var xiter = x.GetEnumerator();
            var yiter = y.GetEnumerator();
            while(xiter.MoveNext() && yiter.MoveNext())
            {
                if (!equals<EQ, A>(xiter.Current, yiter.Current)) return false;
            }
            return true;
        }

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Arr<A> x) =>
            x.GetHashCode();
    }
}
