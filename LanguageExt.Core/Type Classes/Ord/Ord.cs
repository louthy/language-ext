using System.Collections.Generic;
using LanguageExt.Attributes;
using System.Diagnostics.Contracts;

namespace LanguageExt.TypeClasses
{
    [Typeclass("Ord*")]
    public interface Ord<A> : Eq<A>, OrdAsync<A>, Typeclass 
    {
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
        [Pure]
        int Compare(A x, A y);
    }
        
    public static class OrdExt
    {
        class OrdComparer<A> : IComparer<A>
        {
            readonly Ord<A> ord;

            public OrdComparer(Ord<A> ord) =>
                this.ord = ord;

            public int Compare(A x, A y) =>
                ord.Compare(x, y);
        }

        public static IComparer<A> ToComparable<A>(this Ord<A> self) =>
            new OrdComparer<A>(self);
    }
}
