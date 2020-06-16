using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct HashableTryAsync<HashA, A> : HashableAsync<TryAsync<A>> where HashA : struct, HashableAsync<A>
    {
        public async Task<int> GetHashCodeAsync(TryAsync<A> x)
        {
            var dx = await x.Try();
            return dx.IsSuccess 
                ? await default(HashA).GetHashCodeAsync(dx.Value) 
                : 0;
        }
    }

    public struct HashableTryAsync<A> : HashableAsync<TryAsync<A>>
    {
        public Task<int> GetHashCodeAsync(TryAsync<A> x) =>
            default(HashableTryAsync<HashableDefaultAsync<A>, A>).GetHashCodeAsync(x);
    }
}
