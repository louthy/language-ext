using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Compare the equality of any type in the Optional type-class
    /// </summary>
    public struct EqOpt<EQ, OPTION, OA, A> : Eq<OA>
        where EQ     : struct, Eq<A>
        where OPTION : struct, Optional<OA, A>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(OA x, OA y)
        {
            var xIsSome = default(OPTION).IsSome(x);
            var yIsSome = default(OPTION).IsSome(y);
            var xIsNone = !xIsSome;
            var yIsNone = !yIsSome;

            return xIsNone && yIsNone
                ? true
                : xIsNone || yIsNone
                    ? false
                    : default(OPTION).Match(x,
                        Some: a =>
                            default(OPTION).Match(y,
                                Some: b => @equals<EQ, A>(a, b),
                                None: () => false),
                        None: () => false);
        }
    }
}
