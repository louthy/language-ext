using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct HashableTryAsync<HashA, A> : HashableAsync<TryAsync<A>> where HashA : struct, Hashable<A>
    {
        public Task<int> GetHashCodeAsync(TryAsync<A> x) =>
            x.Map(default(HashA).GetHashCode).IfFail(0);
    }

    public struct HashableTryAsync<A> : HashableAsync<TryAsync<A>>
    {
        public Task<int> GetHashCodeAsync(TryAsync<A> x) =>
            default(HashableTryAsync<HashableDefault<A>, A>).GetHashCodeAsync(x);
    }
}
