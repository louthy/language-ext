using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    public struct EqQue<EQ, A> : Eq<Que<A>> where EQ : struct, Eq<A>
    {
        public static readonly EqQue<EQ, A> Inst = default(EqQue<EQ, A>);

        [Pure]
        public bool Equals(Que<A> x, Que<A> y)
        {
            if (x.Count != y.Count) return false;

            var enumx = x.GetEnumerator();
            var enumy = y.GetEnumerator();
            var count = x.Count;

            for (int i = 0; i < count; i++)
            {
                enumx.MoveNext();
                enumy.MoveNext();
                if (default(EQ).Equals(enumx.Current, enumy.Current)) return false;
            }
            return true;
        }


        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Que<A> x) =>
            default(HashableQue<EQ, A>).GetHashCode(x);
    }

    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    public struct EqQue<A> : Eq<Que<A>>
    {
        public static readonly EqQue<A> Inst = default(EqQue<A>);

        [Pure]
        public bool Equals(Que<A> x, Que<A> y) =>
            default(EqQue<EqDefault<A>, A>).Equals(x, y);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Que<A> x) =>
            default(HashableQue<A>).GetHashCode(x);
    }

}
