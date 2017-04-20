using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    public struct EqSeq<EQ, A> : Eq<ISeq<A>>
        where EQ : struct, Eq<A>
    {
        public static readonly EqSeq<EQ, A> Inst = default(EqSeq<EQ, A>);

        /// <summary>
        /// Equality check
        /// </summary>
        [Pure]
        public bool Equals(ISeq<A> x, ISeq<A> y)
        {
            if (x == null) return y == null;
            if (y == null) return false;
            if (x.Count != y.Count) return false;

            while (true)
            {
                bool a = x.IsEmpty;
                bool b = y.IsEmpty;
                if (a != b) return false;
                if (a && b) return true;

                if (!default(EQ).Equals(x.Head, y.Head)) return false;
                x = x.Tail;
                y = y.Tail;
            }
        }

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(ISeq<A> x) =>
            hash(x);
    }
}
