using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Finds an appropriate Eq from the loaded assemblies, if one can't be found then it
    /// falls back to the standard .NET EqualityComparer<A>.Default.Equals(a,b) method to
    /// provide equality testing.
    /// </summary>
    public readonly struct EqDefault<A> : Eq<A>
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
            return EqClass<A>.Equals(a, b);
        }

        /// <summary>
        /// Get hash code of the value
        /// </summary>
        /// <param name="x">Value to get the hash code of</param>
        /// <returns>The hash code of x</returns>
        [Pure]
        public int GetHashCode(A x) =>
            default(HashableDefault<A>).GetHashCode(x);

        [Pure]
        public Task<bool> EqualsAsync(A x, A y) =>
            default(EqDefaultAsync<A>).EqualsAsync(x, y);

        [Pure]
        public Task<int> GetHashCodeAsync(A x) => 
            default(HashableDefaultAsync<A>).GetHashCodeAsync(x);
    }

    /// <summary>
    /// This is a utility type for when two generic types are used, but it's not clear if they
    /// have the same underlying type.  We'd like to do structural equality if they are, and
    /// return false if they're not.
    /// </summary>
    public static class EqDefault<A, B> 
    {
        static readonly Func<A, B, bool> Eq;

        static EqDefault()
        {
            Eq = typeof(A).FullName == typeof(B).FullName
                ? (x, y) => y is A y1 && default(EqDefault<A>).Equals(x, y1)
                : (_, _) => false;
        }
        
        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public static bool Equals(A a, B b)
        {
            if (isnull(a)) return isnull(b);
            if (isnull(b)) return false;
            if (ReferenceEquals(a, b)) return true;
            return Eq(a, b);
        }
    }
}
