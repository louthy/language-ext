using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Hash for all record types
    /// </summary>
    /// <typeparam name="A">Record type</typeparam>
    public struct HashableRecord<A> : Hashable<A> where A : Record<A>
    {
        public int GetHashCode(A x) =>
            RecordType<A>.Hash(x);
    }
}
