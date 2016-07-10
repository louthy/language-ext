using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Compare the equality of any type in the Optional type-class
    /// </summary>
    public struct EqOpt<EQ, A> : Eq<Optional<A>>
        where EQ : struct, Eq<A>
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
                                Some: b => @equals<EQ, A>(a, b),
                                None: () => false),
                        None: () => false);
    }
}
