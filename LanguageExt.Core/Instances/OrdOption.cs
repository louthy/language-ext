using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Bound monadic value equality
    /// </summary>
    public struct OrdOption<ORD, A> : Ord<Option<A>> where ORD : struct, Ord<A>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(Option<A> x, Option<A> y) =>
            x.IsNone && y.IsNone
                ? true
                : x.IsNone || y.IsNone
                    ? false
                    : equals<ORD, A>(x.Value, y.Value);

        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if x greater than y : 1
        /// 
        /// if x less than y    : -1
        /// 
        /// if x equals y       : 0
        /// </returns>
        public int Compare(Option<A> x, Option<A> y)
        {
            if (x.IsNone && y.IsNone) return 0;
            if (x.IsSome && y.IsNone) return 1;
            if (x.IsNone && y.IsSome) return -1;
            return default(ORD).Compare(x.Value, y.Value);
        }
    }
}
