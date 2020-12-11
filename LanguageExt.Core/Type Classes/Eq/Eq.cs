using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Equality type-class
    /// </summary>
    /// <typeparam name="A">
    /// The type for which equality is defined
    /// </typeparam>
    [Typeclass("Eq*")]
    public interface Eq<A> : Hashable<A>, EqAsync<A>, Typeclass
    {
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        bool Equals(A x, A y);
    }

    public static class EqExt
    {
        class EqEqualityComparer<A> : IEqualityComparer<A>
        {
            readonly Eq<A> eq;

            public EqEqualityComparer(Eq<A> eq) =>
                this.eq = eq;

            public bool Equals(A x, A y) =>
                eq.Equals(x, y);

            public int GetHashCode(A obj) =>
                eq.GetHashCode(obj);
        }

        public static IEqualityComparer<A> ToEqualityComparer<A>(this Eq<A> self) =>
            new EqEqualityComparer<A>(self);
    }
}
