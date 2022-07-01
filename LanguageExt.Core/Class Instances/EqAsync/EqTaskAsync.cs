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
        public async Task<bool> EqualsAsync(Task<A> tx, Task<A> ty) =>
            await WaitAsync.All(tx, ty).ConfigureAwait(false) switch
            {
                var (x, y)  => await default(EqA).EqualsAsync(x, y).ConfigureAwait(false)
            };

        public Task<int> GetHashCodeAsync(Task<A> x) =>
            default(HashableTaskAsync<EqA, A>).GetHashCodeAsync(x);
    }
}
