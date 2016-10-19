using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Compare the equality and ordering of any type in the Optional
    /// type-class
    /// </summary>
    public struct OrdSeq<ORD, A> : Ord<Seq<A>>
        where ORD : struct, Ord<A>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(Seq<A> x, Seq<A> y) =>
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
        public int Compare(Seq<A> x, Seq<A> y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(x, null)) return -1;
            if (ReferenceEquals(y, null)) return 1;

            var enumx = x.AsEnumerable().GetEnumerator();
            var enumy = y.AsEnumerable().GetEnumerator();

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
    }
}
