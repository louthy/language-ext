using LanguageExt;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Equality and ordering
    /// </summary>
    public struct OrdSeq<ORD, A> : Ord<ISeq<A>>
        where ORD : struct, Ord<A>
    {
        public static readonly OrdSeq<ORD, A> Inst = default(OrdSeq<ORD, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(ISeq<A> x, ISeq<A> y) =>
            default(EqSeq<ORD, A>).Equals(x, y);

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
        public int Compare(ISeq<A> x, ISeq<A> y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(x, null)) return -1;
            if (ReferenceEquals(y, null)) return 1;

            var enumx = x.GetEnumerator();
            var enumy = y.GetEnumerator();

            while(true)
            {
                bool r1 = enumx.MoveNext();
                bool r2 = enumy.MoveNext();
                if (!r1 && !r2) return 0;
                if (!r1) return -1;
                if (!r2) return 1;

                var cmp = default(ORD).Compare(enumx.Current, enumy.Current);
                if (cmp != 0) return cmp;
            }
        }

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(ISeq<A> x) =>
            hash(x);
    }
}
