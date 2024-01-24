using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Linq;

namespace LanguageExt.ClassInstances;

public struct TArr<A> : 
    Monoid<Arr<A>>
{
    static readonly Arr<A> emp = [];

    [Pure]
    public static Arr<A> Append(Arr<A> x, Arr<A> y) =>
        x.ConcatFast(y).ToArray();

    [Pure]
    public static Arr<A> Empty() => emp;
}
