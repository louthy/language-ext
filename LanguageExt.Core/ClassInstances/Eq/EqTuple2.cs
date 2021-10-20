using System;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct EqTuple2<EqA, EqB, A, B> : Eq<(A, B)>
        where EqA : struct, Eq<A>
        where EqB : struct, Eq<B>
    {
        public Task<int> GetHashCodeAsync((A, B) pair) =>
            default(EqA)
               .GetHashCodeAsync(pair.Item1)
               .Bind(x => default(EqB)
                             .GetHashCodeAsync(pair.Item2)
                             .Map(y => FNV32.Next(x, y)));


        public int GetHashCode((A, B) pair) =>
            FNV32.Next(default(EqA).GetHashCode(pair.Item1), 
                       default(EqB).GetHashCode(pair.Item2));
        
        public Task<bool> EqualsAsync((A, B) x, (A, B) y) =>
            default(EqA).EqualsAsync(x.Item1, y.Item1)
                        .MapAsync(async eq => eq && await default(EqB).EqualsAsync(x.Item2, y.Item2).ConfigureAwait(false));

        public bool Equals((A, B) x, (A, B) y) =>
            default(EqA).Equals(x.Item1, y.Item1) &&
            default(EqB).Equals(x.Item2, y.Item2);
    }
}
