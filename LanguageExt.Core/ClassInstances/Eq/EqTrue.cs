using System;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Always returns true for equality checks
    /// </summary>
    public struct EqTrue<A> : Eq<A>
    {
        public bool Equals(A x, A y) =>
            true;

        public int GetHashCode(A x) =>
            default(EqDefault<A>).GetHashCode(x);
    }
}
