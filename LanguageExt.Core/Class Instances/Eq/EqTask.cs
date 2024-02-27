using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances;

public struct EqTask<A> : Eq<Task<A>>
{
    [Pure]
    public static bool Equals(Task<A> x, Task<A> y) =>
        x.Id == y.Id;

    [Pure]
    public static int GetHashCode(Task<A> x) =>
        HashableTask<A>.GetHashCode(x);
}
