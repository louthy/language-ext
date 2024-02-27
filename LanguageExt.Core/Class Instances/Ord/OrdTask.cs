using LanguageExt.Traits;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances;

public struct OrdTask<A> : Ord<Task<A>>
{
    public static int Compare(Task<A> x, Task<A> y) =>
        x.Id.CompareTo(y.Id);

    public static bool Equals(Task<A> x, Task<A> y) =>
        EqTask<A>.Equals(x, y);

    public static int GetHashCode(Task<A> x) =>
        HashableTask<A>.GetHashCode(x);
}
