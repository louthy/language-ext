using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct EqTask<A> : Eq<Task<A>>
    {
        public bool Equals(Task<A> x, Task<A> y) =>
            x.Id == y.Id;

        public int GetHashCode(Task<A> x) =>
            default(HashableTask<A>).GetHashCode(x);
    }
}
