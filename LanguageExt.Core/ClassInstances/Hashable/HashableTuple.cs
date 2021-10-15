using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct HashableTuple<HashA, HashB, A, B> : Hashable<(A, B)>
        where HashA : struct, Hashable<A>
        where HashB : struct, Hashable<B>
    {
        public Task<int> GetHashCodeAsync((A, B) pair) =>
            default(HashA)
               .GetHashCodeAsync(pair.Item1)
               .Bind(x => default(HashB)
                             .GetHashCodeAsync(pair.Item2)
                             .Map(y => FNV32.Next(x, y)));


        public int GetHashCode((A, B) pair) =>
            FNV32.Next(default(HashA).GetHashCode(pair.Item1), 
                       default(HashB).GetHashCode(pair.Item2));
    }
}
