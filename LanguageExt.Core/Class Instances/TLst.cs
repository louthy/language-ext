using System;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances;

public struct TLst<A> : Monoid<Lst<A>>
{
    [Pure]
    public static Lst<A> Append(Lst<A> x, Lst<A> y) =>
        x.Append(y);

    [Pure]
    public static Lst<A> Subtract(Lst<A> x, Lst<A> y) =>
        x.Subtract(y);

    [Pure]
    public static Lst<A> Empty() =>
        List.empty<A>();
}
