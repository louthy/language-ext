using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Set<T> equality
    /// </summary>
    public struct EqSet<EQ, A> : Eq<Set<A>> where EQ : struct, Eq<A>
    {
        public static readonly EqSet<EQ, A> Inst = default(EqSet<EQ, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(Set<A> x, Set<A> y)
        {
            if (x.Count != y.Count) return false;

            var enumx = x.GetEnumerator();
            var enumy = y.GetEnumerator();
            for (int i = 0; i < x.Count; i++)
            {
                enumx.MoveNext();
                enumy.MoveNext();
                if (!default(EQ).Equals(enumx.Current, enumy.Current)) return false;
            }
            return true;
        }


        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Set<A> x) =>
            x.GetHashCode();
    }
}
