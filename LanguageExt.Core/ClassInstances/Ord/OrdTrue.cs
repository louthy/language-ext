using System;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Always returns true for equality checks and 0 for ordering
    /// </summary>
    public struct OrdTrue<A> : Ord<A>
    {
        public int Compare(A x, A y) =>
            0;

        public bool Equals(A x, A y) =>
            true;

        public int GetHashCode(A x) =>
            default(OrdDefault<A>).GetHashCode(x);
    }
}
