using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Option type hash
    /// </summary>
    public struct HashableOptionAsync<A> : HashableAsync<OptionAsync<A>>
    {
        public Task<int> GetHashCodeAsync(OptionAsync<A> x) =>
            default(HashableOptionalAsync<MOptionAsync<A>, OptionAsync<A>, A>).GetHashCodeAsync(x);
    }

    /// <summary>
    /// Option type hash
    /// </summary>
    public struct HashableOptionAsync<HashA, A> : HashableAsync<OptionAsync<A>> where HashA : struct, Hashable<A>
    {
        public Task<int> GetHashCodeAsync(OptionAsync<A> x) =>
            default(HashableOptionalAsync<HashA, MOptionAsync<A>, OptionAsync<A>, A>).GetHashCodeAsync(x);
    }
}
