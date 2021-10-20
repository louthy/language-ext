using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Option type equality
    /// </summary>
    public struct EqOptionUnsafe<A> : Eq<OptionUnsafe<A>>
    {
        [Pure]
        public bool Equals(OptionUnsafe<A> x, OptionUnsafe<A> y) =>
            default(EqOptionalUnsafe<MOptionUnsafe<A>, OptionUnsafe<A>, A>).Equals(x, y);

        [Pure]
        public int GetHashCode(OptionUnsafe<A> x) =>
            default(HashableOptionUnsafe<A>).GetHashCode(x);
        
        [Pure]
        public Task<bool> EqualsAsync(OptionUnsafe<A> x, OptionUnsafe<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(OptionUnsafe<A> x) =>
            GetHashCode(x).AsTask();
    }
}
