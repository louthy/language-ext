using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Linq;

namespace LanguageExt.ClassInstances;

public struct TArray<A> : Monoid<A[]>
{
    static readonly A[] emp = [];

    [Pure]
    public static A[] Append(A[] x, A[] y) =>
        x.ConcatFast(y).ToArray();

    [Pure]
    public static A[] Empty => emp;
}
