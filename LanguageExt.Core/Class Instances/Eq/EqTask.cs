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
        public async Task<bool> EqualsAsync(Task<A> x, Task<A> y)
        {
            try
            {
                var ts = await Task.WhenAll(x, y).ConfigureAwait(false);
                return default(EqDefault<A>).Equals(ts[0], ts[1]);
            }
            catch (Exception)
            {
                if (x.IsFaulted && y.IsFaulted) return true;
                if (x.IsFaulted || y.IsFaulted) return false;
                if (x.IsCanceled && y.IsCanceled) return true;
                return false;
            }
        }

        [Pure]
        public Task<int> GetHashCodeAsync(Task<A> x) =>
            from a in x
            from r in default(HashableDefaultAsync<A>).GetHashCodeAsync(a)
            select r;
    }
}
