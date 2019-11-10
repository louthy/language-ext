using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Finds an appropriate Eq from the loaded assemblies, if one can't be found then it
    /// falls back to the standard .NET EqualityComparer<A>.Default.Equals(a,b) method to
    /// provide equality testing.
    /// </summary>
    public struct EqDefault<A> : Eq<A>
    {
        public static readonly EqDefault<A> Inst = default(EqDefault<A>);

        static readonly Func<A, A, bool> eq;
        static readonly Func<A, int> hash;
        static readonly IEqualityComparer<A> comparer;

        static EqDefault()
        {
            bool isFunc =
                typeof(A).GetTypeInfo().ToString().StartsWith("System.Func") ||
                typeof(A).GetTypeInfo().ToString().StartsWith("<>");

            comparer = isFunc
                ? new DelEq() as IEqualityComparer<A>
                : EqualityComparer<A>.Default;

            if (isFunc)
            {
                eq = (a, b) => comparer.Equals(a, b);
                hash = x => x.IsNull() ? 0 : comparer.GetHashCode(x);
            }
            else
            {
                var def = Class<Eq<A>>.Default;
                if(def == null)
                {
                    eq = comparer.Equals;
                    hash = x => x.IsNull() ? 0 : comparer.GetHashCode(x);
                }
                else
                {
                    eq = def.Equals;
                    hash = def.GetHashCode;
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
        public bool Equals(A a, A b)
        {
            if (isnull(a)) return isnull(b);
            if (isnull(b)) return false;
            if (ReferenceEquals(a, b)) return true;
            return eq(a, b);
        }

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(A x) =>
            hash(x);

        // Below is a shameless hack to make Func and anonymous Funcs equality comparable
        // This is primarily to support Sets being used as applicatives, where the functor
        // must be in a set itself.  A smarter solution is required.

        class DelEq : IEqualityComparer<A>
        {
            public bool Equals(A x, A y) =>
                ReferenceEquals(x, y);

            public int GetHashCode(A x) =>
                x?.GetHashCode() ?? 0;
        }
    }
}
