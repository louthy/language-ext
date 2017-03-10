using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Uses the standard .NET EqualityComparer<A>.Default.Equals(a,b) method to
    /// provide equality testing.
    /// </summary>
    public struct EqDefault<A> : Eq<A>
    {
        public static readonly EqDefault<A> Inst = default(EqDefault<A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(A a, A b)
        {
            if (isnull(a)) return isnull(b);
            if (isnull(b)) return false;
            if (ReferenceEquals(a, b)) return true;
            return Comparer.Equals(a, b);
        }

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(A x) =>
            x.IsNull() ? 0 : Comparer.GetHashCode(x);

        // Below is a shameless hack to make Func and anonymous Funcs equality comparable
        // This is primarily to support Sets being used as applicatives, where the functor
        // must be in a set itself.  A smarter solution is required.

        static readonly bool IsFunc =
            typeof(A).GetTypeInfo().ToString().StartsWith("System.Func") ||
            typeof(A).GetTypeInfo().ToString().StartsWith("<>");

        static readonly IEqualityComparer<A> Comparer =
            IsFunc
                ? new DelEq() as IEqualityComparer<A>
                : EqualityComparer<A>.Default;

        class DelEq : IEqualityComparer<A>
        {
            public bool Equals(A x, A y) =>
                ReferenceEquals(x, y);

            public int GetHashCode(A x) =>
                x?.GetHashCode() ?? 0;
        }


    }
}
