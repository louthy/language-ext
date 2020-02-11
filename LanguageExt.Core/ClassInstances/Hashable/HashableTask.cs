using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct HashableTask<A> : Hashable<Task<A>>
    {
        public int GetHashCode(Task<A> x) =>
            x.Id.GetHashCode();
    }
}
