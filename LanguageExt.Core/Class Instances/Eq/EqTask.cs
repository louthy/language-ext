using System;
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
            Task.FromResult<Func<A, A, bool>>(default(EqDefault<A>).Equals)
                .Apply(x)
                .Apply(y);

        [Pure]
        public Task<int> GetHashCodeAsync(Task<A> x) =>
            from a in x
            from r in default(HashableDefaultAsync<A>).GetHashCodeAsync(a)
            select r;
    }
}
