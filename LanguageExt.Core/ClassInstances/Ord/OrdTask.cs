using LanguageExt.TypeClasses;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct OrdTask<A> : Ord<Task<A>>
    {
        public int Compare(Task<A> x, Task<A> y) =>
            x.Id.CompareTo(y.Id);

        public bool Equals(Task<A> x, Task<A> y) =>
            x.Id == y.Id;

        public int GetHashCode(Task<A> x) =>
            x.Id.GetHashCode();
    }
}
