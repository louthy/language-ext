using System;
using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Compare the equality of any values bound by the Try monad
    /// </summary>
    public struct EqTry<EQ, A> : Eq<Try<A>>
        where EQ : struct, Eq<A>
    {
        public static readonly EqTry<EQ, A> Inst = default(EqTry<EQ, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(Try<A> x, Try<A> y)
        {
            var a = x.Try();
            var b = y.Try();
            if (a.IsFaulted && b.IsFaulted) return true;
            if (a.IsFaulted || b.IsFaulted) return false;
            return equals<EQ, A>(a.Value, b.Value);
        }

        [Pure]
        public int GetHashCode(Try<A> x) =>
            default(HashableTry<EQ, A>).GetHashCode(x);

        [Pure]
        public Task<bool> EqualsAsync(Try<A> x, Try<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(Try<A> x) =>
            GetHashCode(x).AsTask();
    }

    /// <summary>
    /// Compare the equality of any values bound by the Try monad
    /// </summary>
    public struct EqTry<A> : Eq<Try<A>>
    {
        public static readonly EqTry<A> Inst = default(EqTry<A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(Try<A> x, Try<A> y) =>
            default(EqTry<EqDefault<A>, A>).Equals(x, y);

        [Pure]
        public int GetHashCode(Try<A> x) =>
            default(HashableTry<A>).GetHashCode(x);

        [Pure]
        public Task<bool> EqualsAsync(Try<A> x, Try<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(Try<A> x) =>
            GetHashCode(x).AsTask();    
    }
}
