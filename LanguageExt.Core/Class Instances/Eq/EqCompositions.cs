using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct EqCompositions<A> : Eq<Compositions<A>>
    {
        public bool Equals(Compositions<A> x, Compositions<A> y) =>
            x == y;

        public int GetHashCode(Compositions<A> x) =>
            default(HashableCompositions<A>).GetHashCode(x);

        [Pure]
        public Task<bool> EqualsAsync(Compositions<A> x, Compositions<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(Compositions<A> x) => 
            GetHashCode(x).AsTask();
    }
}
