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
            var ts = await Task.WhenAll(x, y);

            if (x.IsFaulted && y.IsFaulted)
            {
                throw new AggregateException(x.Exception.InnerExceptions.Concat(y.Exception.InnerExceptions));
            }
            else if (x.IsFaulted)
            {
                ExceptionDispatchInfo.Capture(x.Exception.InnerException).Throw();
            }
            else if (y.IsFaulted)
            {
                ExceptionDispatchInfo.Capture(y.Exception.InnerException).Throw();
            }

            return await default(EqA).EqualsAsync(ts[0], ts[1]);
        }

        public Task<int> GetHashCodeAsync(Task<A> x) =>
            default(HashableTaskAsync<EqA, A>).GetHashCodeAsync(x);
    }
}
