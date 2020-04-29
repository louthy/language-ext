using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct HashableTaskAsync<A> : HashableAsync<Task<A>>
    {
        public Task<int> GetHashCodeAsync(Task<A> x) =>
            default(HashableTaskAsync<HashableDefault<A>, A>).GetHashCodeAsync(x);
    }

    public struct HashableTaskAsync<HashA, A> : HashableAsync<Task<A>> where HashA : struct, Hashable<A>
    {
        public Task<int> GetHashCodeAsync(Task<A> x) =>
            x.Map(default(HashA).GetHashCode);
    }
}
