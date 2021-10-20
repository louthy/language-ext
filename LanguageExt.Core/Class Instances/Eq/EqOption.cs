using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Option type equality
    /// </summary>
    public struct EqOption<A> : Eq<Option<A>>
    {
        [Pure]
        public bool Equals(Option<A> x, Option<A> y) =>
            default(EqOptional<MOption<A>, Option<A>, A>).Equals(x, y);

        [Pure]
        public int GetHashCode(Option<A> x) =>
            default(HashableOption<A>).GetHashCode(x);
   
        [Pure]
        public Task<bool> EqualsAsync(Option<A> x, Option<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(Option<A> x) => 
            GetHashCode(x).AsTask();       
    }
}
