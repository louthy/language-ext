using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Uses the standard .NET  Comparer<A>.Default.Compare(a,b) method to
    /// provide equality testing.
    /// </summary>
    public struct OrdDefault<A> : Ord<A>
    {
        public static readonly OrdDefault<A> Inst = default(OrdDefault<A>);

        static readonly IComparer<A> comparer;
        static readonly Func<A, A, int> ord;

        static OrdDefault()
        {
            bool isFunc =
                typeof(A).GetTypeInfo().ToString().StartsWith("System.Func") ||
                typeof(A).GetTypeInfo().ToString().StartsWith("<>");

            comparer = isFunc
                ? new DelCompare() as IComparer<A>
                : Comparer<A>.Default;

            if (isFunc)
            {
                ord = (a, b) => comparer.Compare(a, b);
            }
            else
            {
                var def = Class<Ord<A>>.Default;
                if (def == null)
                {
                    ord = comparer.Compare;
                }
                else
                {
                    ord = def.Compare;
                }
            }
        }

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public int Compare(A x, A y) =>
            ord(x, y);

        [Pure]
        public bool Equals(A x, A y) =>
            default(EqDefault<A>).Equals(x, y);

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(A x) =>
            default(EqDefault<A>).GetHashCode(x);

        // Below is a shameless hack to make Func and anonymous Funcs equality comparable
        // This is primarily to support Sets being used as applicatives, where the functor
        // must be in a set itself.  A smarter solution is required.

        class DelCompare : IComparer<A>
        {
            public int Compare(A x, A y) =>
                ReferenceEquals(x, y) ? 0 : -1;
        }
    }
}
