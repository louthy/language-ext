using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Equality class instance for all record types
    /// </summary>
    /// <typeparam name="A">Record type</typeparam>
    public struct EqRecord<A> : Eq<A> where A : Record<A>
    {
        [Pure]
        public bool Equals(A x, A y) =>
            RecordType<A>.EqualityTyped(x, y);

        [Pure]
        public int GetHashCode(A x) =>
            default(HashableRecord<A>).GetHashCode(x);
        
        [Pure]
        public Task<bool> EqualsAsync(A x, A y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(A x) =>
            GetHashCode(x).AsTask();
    }
}
