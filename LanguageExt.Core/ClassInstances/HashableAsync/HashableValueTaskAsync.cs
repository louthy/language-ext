using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct HashableValueTaskAsync<A> : HashableAsync<ValueTask<A>>
    {
        public Task<int> GetHashCodeAsync(ValueTask<A> x) =>
            default(HashableValueTaskAsync<HashableDefaultAsync<A>, A>).GetHashCodeAsync(x);
    }

    public struct HashableValueTaskAsync<HashA, A> : HashableAsync<ValueTask<A>> where HashA : struct, HashableAsync<A>
    {
        public Task<int> GetHashCodeAsync(ValueTask<A> x) =>
            x.AsTask().Bind(default(HashA).GetHashCodeAsync);
    }
}
