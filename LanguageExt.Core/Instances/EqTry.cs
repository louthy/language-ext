using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Compare the equality of any values bound by the Try monad
    /// </summary>
    public struct EqTry<EQ, A> : Eq<Try<A>>
        where EQ : struct, Eq<A>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        public bool Equals(Try<A> x, Try<A> y)
        {
            var a = x.Try();
            var b = y.Try();
            if (a.IsFaulted && b.IsFaulted) return true;
            if (a.IsFaulted || b.IsFaulted) return false;
            return equals<EQ, A>(a.Value, b.Value);
        }
    }
}
