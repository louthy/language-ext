using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct EqTaskAsync<A> : EqAsync<Task<A>>
    {
        public Task<bool> EqualsAsync(Task<A> x, Task<A> y) =>
            default(EqTaskAsync<EqDefault<A>, A>).EqualsAsync(x, y);

        public Task<int> GetHashCodeAsync(Task<A> x) =>
            default(HashableTaskAsync<A>).GetHashCodeAsync(x);
    }

    public struct EqTaskAsync<EqA, A> : EqAsync<Task<A>> where EqA : struct, Eq<A>
    {
        public Task<bool> EqualsAsync(Task<A> x, Task<A> y) =>
            default(ApplTask<A, bool>).Apply(default(EqA).Equals, x, y);

        public Task<int> GetHashCodeAsync(Task<A> x) =>
            default(HashableTaskAsync<EqA, A>).GetHashCodeAsync(x);
    }
}
