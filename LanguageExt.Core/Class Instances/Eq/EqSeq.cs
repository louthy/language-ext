using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    public struct EqSeq<EqA, A> : Eq<Seq<A>>
        where EqA : struct, Eq<A>
    {
        public static readonly EqSeq<EqA, A> Inst = default(EqSeq<EqA, A>);

        /// <summary>
        /// Equality check
        /// </summary>
        [Pure]
        public bool Equals(Seq<A> x, Seq<A> y)
        {
            if (x.Count != y.Count) return false;

            while (true)
            {
                bool a = x.IsEmpty;
                bool b = y.IsEmpty;
                if (a != b) return false;
                if (a && b) return true;

                if (!default(EqA).Equals(x.Head, y.Head)) return false;
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
        public int GetHashCode(Seq<A> x) =>
            default(HashableSeq<EqA, A>).GetHashCode(x);
            
        [Pure]
        public Task<bool> EqualsAsync(Seq<A> x, Seq<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(Seq<A> x) =>
            GetHashCode(x).AsTask();
    }

    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    public struct EqSeq<A> : Eq<Seq<A>>
    {
        public static readonly EqSeq<A> Inst = default(EqSeq<A>);

        /// <summary>
        /// Equality check
        /// </summary>
        [Pure]
        public bool Equals(Seq<A> x, Seq<A> y) =>
            default(EqSeq<EqDefault<A>, A>).Equals(x, y);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(Seq<A> x) =>
            default(HashableSeq<A>).GetHashCode(x);
            
        [Pure]
        public Task<bool> EqualsAsync(Seq<A> x, Seq<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(Seq<A> x) =>
            GetHashCode(x).AsTask();
    }
}
