using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// HashSet equality
    /// </summary>
    public struct EqHashSet<EQ, A> : Eq<HashSet<A>> where EQ : struct, Eq<A>
    {
        public static readonly EqHashSet<EQ, A> Inst = default(EqHashSet<EQ, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(HashSet<A> x, HashSet<A> y)
        {
            if (x.Count != y.Count) return false;

            var enumx = x.GetEnumerator();
            var enumy = y.GetEnumerator();
            for (int i = 0; i < x.Count; i++)
            {
                enumx.MoveNext();
                enumy.MoveNext();
                if (!default(EQ).Equals(enumx.Current, enumy.Current)) return false;
            }
            return true;
        }

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(HashSet<A> x) =>
            default(HashableHashSet<EQ, A>).GetHashCode(x);
  
        [Pure]
        public Task<bool> EqualsAsync(HashSet<A> x, HashSet<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(HashSet<A> x) => 
            GetHashCode(x).AsTask();      
    }

    /// <summary>
    /// HashSet equality
    /// </summary>
    public struct EqHashSet<A> : Eq<HashSet<A>>
    {
        public static readonly EqHashSet<A> Inst = default(EqHashSet<A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(HashSet<A> x, HashSet<A> y) =>
            default(EqHashSet<EqDefault<A>, A>).Equals(x, y);

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(HashSet<A> x) =>
            default(HashableHashSet<A>).GetHashCode(x);
  
        [Pure]
        public Task<bool> EqualsAsync(HashSet<A> x, HashSet<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(HashSet<A> x) => 
            GetHashCode(x).AsTask();      
    }

}
