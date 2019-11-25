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

        static EqDefault()
        {
            if (Reflect.IsFunc(typeof(A)))
            {
                eq = (a, b) => ReferenceEquals(a, b);
                hash = x => x.IsNull() ? 0 : x.GetHashCode();
            }
            else if (Reflect.IsAnonymous(typeof(A)))
            {
                eq = IL.EqualsTyped<A>(false);
                hash = IL.GetHashCode<A>(false);
            }
            else
            {
                var def = Class<Ord<A>>.Default;
                if (def == null)
                {
                    eq = EqualityComparer<A>.Default.Equals;
                    hash = x => x.IsNull() ? 0 : x.GetHashCode();
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
    }
}
