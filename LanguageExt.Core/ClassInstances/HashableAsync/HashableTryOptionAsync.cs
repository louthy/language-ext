using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct HashableTryOptionAsync<HashA, A> : HashableAsync<TryOptionAsync<A>> where HashA : struct, HashableAsync<A>
    {
        public async Task<int> GetHashCodeAsync(TryOptionAsync<A> x)
        {
            var dx = await x.Try();
            return dx.IsSome 
                ? await default(HashA).GetHashCodeAsync(dx.Value.Value) 
                : 0;
        }
    }

    public struct HashableTryOptionAsync<A> : HashableAsync<TryOptionAsync<A>>
    {
        public Task<int> GetHashCodeAsync(TryOptionAsync<A> x) =>
            default(HashableTryOptionAsync<HashableDefaultAsync<A>, A>).GetHashCodeAsync(x);
    }
}
