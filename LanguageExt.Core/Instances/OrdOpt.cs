using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Compare the equality and ordering of any type in the Optional
    /// type-class
    /// </summary>
    public struct OrdOpt<ORD, A> : Ord<Optional<A>>
        where ORD : struct, Ord<A>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(Optional<A> x, Optional<A> y) =>
            x.IsNoneA(x) && y.IsNoneA(y)
                ? true
                : x.IsNoneA(x) || y.IsNoneA(y)
                    ? false
                    : x.Match(x,
                        Some: a =>
                            y.Match(y,
                                Some: b => @equals<ORD, A>(a, b),
                                None: () => false),
                        None: () => false);

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
        public int Compare(Optional<A> mx, Optional<A> my)
        {
            if (mx.IsNoneA(mx) && my.IsNoneA(my)) return 0;
            if (mx.IsSomeA(mx) && my.IsNoneA(my)) return 1;
            if (mx.IsNoneA(mx) && my.IsSomeA(my)) return -1;

            return mx.Match(mx,
                Some: a =>
                    my.Match(my,
                        Some: b => compare<ORD, A>(a, b),
                        None: () => 0),
                None: () => 0);
        }
    }
}
