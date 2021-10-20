using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Equality instance for `Patch` `Edit`
    /// </summary>
    public struct EqEdit<EqA, A> : Eq<Edit<EqA, A>> where EqA : struct, Eq<A>
    {
        public bool Equals(Edit<EqA, A> x, Edit<EqA, A> y) => 
            x == y;

        public int GetHashCode(Edit<EqA, A> x) =>
            default(HashableEdit<EqA, A>).GetHashCode(x);
  
        [Pure]
        public Task<bool> EqualsAsync(Edit<EqA, A> x, Edit<EqA, A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(Edit<EqA, A> x) => 
            GetHashCode(x).AsTask();      
    }
}
