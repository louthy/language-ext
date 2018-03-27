using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct EqTaskAsync<A> : EqAsync<Task<A>>
    {
        public Task<bool> Equals(Task<A> x, Task<A> y) =>
            default(EqTaskAsync<EqDefault<A>, A>).Equals(x, y);

        public Task<int> GetHashCode(Task<A> x) =>
            default(EqTaskAsync<EqDefault<A>, A>).GetHashCode(x);
    }

    public struct EqTaskAsync<EqA, A> : EqAsync<Task<A>> where EqA : struct, Eq<A>
    {
        public Task<bool> Equals(Task<A> x, Task<A> y) =>
            default(ApplTask<A, bool>).Apply(default(EqA).Equals, x, y);

        public Task<int> GetHashCode(Task<A> x) =>
            x.Map(default(EqA).GetHashCode);
    }
}
