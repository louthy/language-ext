using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Hash for `Patch` `Edit`
    /// </summary>
    public struct HashableEdit<EqA, A> : Hashable<Edit<EqA, A>> where EqA : struct, Eq<A>
    {
        [Pure]
        public int GetHashCode(Edit<EqA, A> x) => 
            x?.GetHashCode() ?? 0;

        [Pure]
        public Task<int> GetHashCodeAsync(Edit<EqA, A> x) =>
            GetHashCode(x).AsTask();
    }
}
