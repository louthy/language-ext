using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Array equality
    /// </summary>
    public struct EqArr<EqA, A> : Eq<Arr<A>> where EqA : struct, Eq<A>
    {
        public static readonly EqArr<EqA, A> Inst = default(EqArr<EqA, A>);

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
                if (!equals<EqA, A>(xiter.Current, yiter.Current)) return false;
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
            default(HashableArr<A>).GetHashCode(x);
    }

    /// <summary>
    /// Array equality
    /// </summary>
    public struct EqArr<A> : Eq<Arr<A>>
    {
        public static readonly EqArr<A> Inst = default(EqArr<A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(Arr<A> x, Arr<A> y) =>
            default(EqArr<EqDefault<A>, A>).Equals(x, y);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Arr<A> x) =>
            default(HashableArr<HashableDefault<A>, A>).GetHashCode(x);
    }

}
