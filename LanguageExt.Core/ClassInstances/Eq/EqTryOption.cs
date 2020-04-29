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
    public struct EqTryOption<EQ, A> : Eq<TryOption<A>>
        where EQ : struct, Eq<A>
    {
        public static readonly EqTryOption<EQ, A> Inst = default(EqTryOption<EQ, A>);

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
        public int GetHashCode(TryOption<A> x) =>
            default(HashableTryOption<EQ, A>).GetHashCode(x);
    }

    /// <summary>
    /// Compare the equality of any type in the TryOption type-class
    /// </summary>
    public struct EqTryOption<A> : Eq<TryOption<A>>
    {
        public static readonly EqTryOption<A> Inst = default(EqTryOption<A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="lhs">The left hand side of the equality operation</param>
        /// <param name="rhs">The right hand side of the equality operation</param>
        /// <returns>True if lhs and rhs are equal</returns>
        [Pure]
        public bool Equals(TryOption<A> lhs, TryOption<A> rhs) =>
            default(EqTryOption<EqDefault<A>, A>).Equals(lhs, rhs);

        [Pure]
        public int GetHashCode(TryOption<A> x) =>
            default(HashableTryOption<A>).GetHashCode(x);
    }
}
