using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Always returns true for equality checks
    /// </summary>
    public struct EqTrue<A> : Eq<A>
    {
        [Pure]
        public bool Equals(A x, A y) =>
            true;

        [Pure]
        public int GetHashCode(A x) =>
            default(EqDefault<A>).GetHashCode(x);

        [Pure]
        public Task<bool> EqualsAsync(A x, A y) =>
            true.AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(A x) =>
            default(EqDefault<A>).GetHashCode(x).AsTask();
    }
}
