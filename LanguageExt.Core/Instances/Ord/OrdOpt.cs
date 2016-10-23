using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Compare the equality and ordering of any type in the Optional
    /// type-class
    /// </summary>
    public struct OrdOpt<ORD, OPTION, OA, A> : Ord<OA>
        where ORD    : struct, Ord<A>
        where OPTION : struct, Optional<OA, A>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(OA x, OA y) =>
            default(EqOpt<ORD, OPTION, OA, A>).Equals(x, y);

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
        public int Compare(OA mx, OA my)
        {
            var xIsSome = default(OPTION).IsSome(mx);
            var yIsSome = default(OPTION).IsSome(my);
            var xIsNone = !xIsSome;
            var yIsNone = !yIsSome;

            if (xIsNone && yIsNone) return 0;
            if (xIsSome && yIsNone) return 1;
            if (xIsNone && yIsSome) return -1;

            return default(OPTION).Match(mx,
                Some: a =>
                    default(OPTION).Match(my, 
                        Some: b => compare<ORD, A>(a, b),
                        None: () => 0),
                None: () => 0);
        }
    }
}
