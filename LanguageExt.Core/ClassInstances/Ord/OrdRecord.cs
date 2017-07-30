using System;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Ordering class instance for all record types
    /// </summary>
    /// <typeparam name="A">Record type</typeparam>
    public struct OrdRecord<A> : Ord<A> where A : Record<A>
    {
        public int Compare(A x, A y) =>
            RecordType<A>.Compare(x, y);

        public bool Equals(A x, A y) =>
            RecordType<A>.EqualityTyped(x, y);

        public int GetHashCode(A x) =>
            RecordType<A>.Hash(x);
    }
}
