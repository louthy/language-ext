using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct EqValueTaskAsync<A> : EqAsync<ValueTask<A>>
    {
        public Task<bool> EqualsAsync(ValueTask<A> x, ValueTask<A> y) =>
            default(EqValueTaskAsync<EqDefaultAsync<A>, A>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(ValueTask<A> x) =>
            default(HashableValueTaskAsync<A>).GetHashCodeAsync(x);
    }

    public struct EqValueTaskAsync<EqA, A> : EqAsync<ValueTask<A>> where EqA : struct, EqAsync<A>
    {
        public async Task<bool> EqualsAsync(ValueTask<A> x, ValueTask<A> y)
        {
            try
            {
                var ts = await Task.WhenAll(x.AsTask(), y.AsTask()).ConfigureAwait(false);
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

        public Task<int> GetHashCodeAsync(ValueTask<A> x) =>
            default(HashableValueTaskAsync<EqA, A>).GetHashCodeAsync(x);
    }
}
