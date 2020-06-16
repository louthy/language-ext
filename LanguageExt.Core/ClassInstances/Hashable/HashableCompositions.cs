using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct HashableCompositions<A> : Hashable<Compositions<A>>
    {
        [Pure]
        public int GetHashCode(Compositions<A> x) =>
            x.GetHashCode();

        [Pure]
        public Task<int> GetHashCodeAsync(Compositions<A> x) =>
            GetHashCode(x).AsTask();
    }
}
