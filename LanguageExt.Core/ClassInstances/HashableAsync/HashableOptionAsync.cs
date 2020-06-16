using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Option type hash
    /// </summary>
    public struct HashableOptionAsync<A> : HashableAsync<OptionAsync<A>>
    {
        public async Task<int> GetHashCodeAsync(OptionAsync<A> x)
        {
            var (s, d) = await x.Data;
            return s ? await default(HashableDefaultAsync<A>).GetHashCodeAsync(d) : 0;
        }
    }

    /// <summary>
    /// Option type hash
    /// </summary>
    public struct HashableOptionAsync<HashA, A> : HashableAsync<OptionAsync<A>> 
        where HashA : struct, HashableAsync<A>
    {
        public async Task<int> GetHashCodeAsync(OptionAsync<A> x)
        {
            var (s, d) = await x.Data;
            return s ? await default(HashA).GetHashCodeAsync(d) : 0;
        }
    }
}
