using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Equality class instance for all record types
    /// </summary>
    /// <typeparam name="A">Record type</typeparam>
    public struct EqRecord<A> : Eq<A> where A : Record<A>
    {
        public bool Equals(A x, A y) =>
            RecordType<A>.EqualityTyped(x, y);

        public int GetHashCode(A x) =>
            default(HashableRecord<A>).GetHashCode(x);
    }
}
