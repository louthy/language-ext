using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Compare the equality of any type in the TryOption type-class
    /// </summary>
    public struct EqTryOpt<EQ, A> : Eq<TryOption<A>>
        where EQ : struct, Eq<A>
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="lhs">The left hand side of the equality operation</param>
        /// <param name="rhs">The right hand side of the equality operation</param>
        /// <returns>True if lhs and rhs are equal</returns>
        public bool Equals(TryOption<A> lhs, TryOption<A> rhs)
        {
            var x = lhs.Try();
            var y = lhs.Try();
            if (x.IsFaulted && y.IsFaulted) return true;
            if (x.IsFaulted || y.IsFaulted) return false;
            return equals<EQ, A>(x.Value, y.Value);
        }
    }
}
