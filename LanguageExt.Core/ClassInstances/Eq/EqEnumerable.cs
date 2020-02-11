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
    public struct EqEnumerable<EQ, A> : Eq<IEnumerable<A>>
        where EQ : struct, Eq<A>
    {
        public static readonly EqEnumerable<EQ, A> Inst = default(EqEnumerable<EQ, A>);

        /// <summary>
        /// Equality check
        /// </summary>
        [Pure]
        public bool Equals(IEnumerable<A> x, IEnumerable<A> y)
        {
            if (x == null) return y == null;
            if (y == null) return false;

            var enumx = x.GetEnumerator();
            var enumy = y.GetEnumerator();
            while(true)
            {
                bool a = enumx.MoveNext();
                bool b = enumy.MoveNext();
                if (a != b) return false;
                if (!a && !b) return true;
                if (!default(EQ).Equals(enumx.Current, enumy.Current)) return false;
            }
        }

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(IEnumerable<A> x) =>
            default(HashableEnumerable<EQ, A>).GetHashCode(x);
    }

    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    public struct EqEnumerable<A> : Eq<IEnumerable<A>>
    {
        public static readonly EqEnumerable<A> Inst = default(EqEnumerable<A>);

        /// <summary>
        /// Equality check
        /// </summary>
        [Pure]
        public bool Equals(IEnumerable<A> x, IEnumerable<A> y) =>
            default(EqEnumerable<EqDefault<A>, A>).Equals(x, y);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(IEnumerable<A> x) =>
            default(HashableEnumerable<A>).GetHashCode(x);
    }
}
