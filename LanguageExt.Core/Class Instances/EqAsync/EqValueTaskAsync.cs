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
        public async Task<bool> EqualsAsync(ValueTask<A> tx, ValueTask<A> ty) =>
            await WaitAsync.All(tx, ty).ConfigureAwait(false) switch
            {
                var (x, y)  => await default(EqA).EqualsAsync(x, y).ConfigureAwait(false)
            };

        public Task<int> GetHashCodeAsync(ValueTask<A> x) =>
            default(HashableValueTaskAsync<EqA, A>).GetHashCodeAsync(x);
    }
}
