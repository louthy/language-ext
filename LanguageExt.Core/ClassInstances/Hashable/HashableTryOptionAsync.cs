using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct HashableTryOptionAsync<HashA, A> : HashableAsync<TryOptionAsync<A>> where HashA : struct, Hashable<A>
    {
        public Task<int> GetHashCodeAsync(TryOptionAsync<A> x) =>
            x.Map(default(HashA).GetHashCode)
             .IfNoneOrFail(0);
    }

    public struct HashableTryOptionAsync<A> : HashableAsync<TryOptionAsync<A>>
    {
        public Task<int> GetHashCodeAsync(TryOptionAsync<A> x) =>
            default(HashableTryOptionAsync<HashableDefault<A>, A>).GetHashCodeAsync(x);
    }
}
