using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Bound monadic value equality
    /// </summary>
    public struct EqOption<EQ, A> : Eq<Option<A>> where EQ : struct, Eq<A>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(Option<A> x, Option<A> y) =>
            x.IsNone() && y.IsNone()
                ? true
                : x.IsNone() || y.IsNone()
                    ? false
                    : equals<EQ, A>(x.Value(), y.Value());
    }
}
