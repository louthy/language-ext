using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Equality instance for `Patch`
    /// </summary>
    /// <typeparam name="EqA"></typeparam>
    /// <typeparam name="A"></typeparam>
    public struct EqPatch<EqA, A> : Eq<Patch<EqA, A>> where EqA : struct, Eq<A>
    {
        [Pure]
        public bool Equals(Patch<EqA, A> x, Patch<EqA, A> y) => 
            x == y;

        [Pure]
        public int GetHashCode(Patch<EqA, A> x) =>
            default(HashablePatch<EqA, A>).GetHashCode(x);
        
        [Pure]
        public Task<bool> EqualsAsync(Patch<EqA, A> x, Patch<EqA, A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(Patch<EqA, A> x) =>
            GetHashCode(x).AsTask();
    }
}
