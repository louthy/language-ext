using System;
using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Compare the equality of any type in the TryOption type-class
    /// </summary>
    public struct EqTryOpt<EQ, A> : Eq<TryOption<A>>
        where EQ : struct, Eq<A>
    {
        public static readonly EqTryOpt<EQ, A> Inst = default(EqTryOpt<EQ, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="lhs">The left hand side of the equality operation</param>
        /// <param name="rhs">The right hand side of the equality operation</param>
        /// <returns>True if lhs and rhs are equal</returns>
        [Pure]
        public bool Equals(TryOption<A> lhs, TryOption<A> rhs)
        {
            var x = lhs.Try();
            var y = rhs.Try();
            if (x.IsFaulted && y.IsFaulted) return true;
            if (x.IsFaulted || y.IsFaulted) return false;
            return equals<EQ, A>(x.Value, y.Value);
        }

        [Pure]
        public int GetHashCode(TryOption<A> x)
        {
            var res = x.Try();
            return res.IsFaulted || res.Value.IsNone || res.Value.Value.IsNull() ? 0 : res.Value.GetHashCode();
        }
    }

    /// <summary>
    /// Compare the equality of any type in the TryOption type-class
    /// </summary>
    public struct EqTryOpt<A> : Eq<TryOption<A>>
    {
        public static readonly EqTryOpt<A> Inst = default(EqTryOpt<A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="lhs">The left hand side of the equality operation</param>
        /// <param name="rhs">The right hand side of the equality operation</param>
        /// <returns>True if lhs and rhs are equal</returns>
        [Pure]
        public bool Equals(TryOption<A> lhs, TryOption<A> rhs) =>
            default(EqTryOpt<EqDefault<A>, A>).Equals(lhs, rhs);

        [Pure]
        public int GetHashCode(TryOption<A> x) =>
            default(EqTryOpt<EqDefault<A>, A>).GetHashCode(x);
    }
}
