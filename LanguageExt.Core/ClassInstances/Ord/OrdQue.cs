using LanguageExt;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Equality and ordering
    /// </summary>
    public struct OrdQue<ORD, A> : Ord<Que<A>>
        where ORD : struct, Ord<A>
    {
        public static readonly OrdQue<ORD, A> Inst = default(OrdQue<ORD, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(Que<A> x, Que<A> y) =>
            default(EqQue<ORD, A>).Equals(x, y);

        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if x greater than y : 1
        /// if x less than y    : -1
        /// if x equals y       : 0
        /// </returns>
        [Pure]
        public int Compare(Que<A> x, Que<A> y)
        {
            var cmp = x.Count.CompareTo(y.Count);
            if (cmp == 0)
            {
                var enumx = x.GetEnumerator();
                var enumy = y.GetEnumerator();
                var count = x.Count;

                for (int i = 0; i < count; i++)
                {
                    enumx.MoveNext();
                    enumy.MoveNext();
                    cmp = default(ORD).Compare(enumx.Current, enumy.Current);
                    if (cmp != 0) return cmp;
                }
                return 0;
            }
            else
            {
                return cmp;
            }
        }

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(Que<A> x) =>
            x.GetHashCode();
    }
}
