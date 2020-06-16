using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Either type hash
    /// </summary>
    public struct HashableEitherAsync<L, R> : HashableAsync<EitherAsync<L, R>>
    {
        public Task<int> GetHashCodeAsync(EitherAsync<L, R> x) =>
            default(HashableEitherAsync<HashableDefaultAsync<L>, HashableDefaultAsync<R>, L, R>).GetHashCodeAsync(x);
    }
    
    /// <summary>
    /// Either type hash
    /// </summary>
    public struct HashableEitherAsync<HashL, HashR, L, R> : HashableAsync<EitherAsync<L, R>>
        where HashL : struct, HashableAsync<L>
        where HashR : struct, HashableAsync<R>
    {
        public async Task<int> GetHashCodeAsync(EitherAsync<L, R> x)
        {
            var d = await x.Data;
            return d.State switch
            {
                EitherStatus.IsRight => await default(HashL).GetHashCodeAsync(d.Left),
                EitherStatus.IsLeft => await default(HashR).GetHashCodeAsync(d.Right),
                _ => 0
            };
        }
    }
}
