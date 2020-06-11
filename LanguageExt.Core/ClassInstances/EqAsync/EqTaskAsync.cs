using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct EqTaskAsync<A> : EqAsync<Task<A>>
    {
        public Task<bool> EqualsAsync(Task<A> x, Task<A> y) =>
            default(EqTaskAsync<EqDefaultAsync<A>, A>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(Task<A> x) =>
            default(HashableTaskAsync<A>).GetHashCodeAsync(x);
    }

    public struct EqTaskAsync<EqA, A> : EqAsync<Task<A>> where EqA : struct, EqAsync<A>
    {
        public async Task<bool> EqualsAsync(Task<A> x, Task<A> y)
        {
            try
            {
                var ts = await Task.WhenAll(x, y).ConfigureAwait(false);
                return await default(EqA).EqualsAsync(ts[0], ts[1]).ConfigureAwait(false);
            }
            catch (Exception)
            {
                if (x.IsFaulted && y.IsFaulted) return true;
                if (x.IsFaulted || y.IsFaulted) return false;
                if (x.IsCanceled && y.IsCanceled) return true;
                return false;
            }
        }

        public Task<int> GetHashCodeAsync(Task<A> x) =>
            default(HashableTaskAsync<EqA, A>).GetHashCodeAsync(x);
    }
}
