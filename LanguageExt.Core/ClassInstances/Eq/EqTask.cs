using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    
    public struct EqTask<A> : Eq<Task<A>>
    {
        [Pure]
        public bool Equals(Task<A> x, Task<A> y) =>
            x.Id == y.Id;

        [Pure]
        public int GetHashCode(Task<A> x) =>
            default(HashableTask<A>).GetHashCode(x);

        [Pure]
        public Task<bool> EqualsAsync(Task<A> x, Task<A> y) =>
            from a in x
            from b in y
            from r in default(EqDefaultAsync<A>).EqualsAsync(a, b)  
            select r;

        [Pure]
        public Task<int> GetHashCodeAsync(Task<A> x) =>
            from a in x
            from r in default(HashableDefaultAsync<A>).GetHashCodeAsync(a)
            select r;
    }
}
