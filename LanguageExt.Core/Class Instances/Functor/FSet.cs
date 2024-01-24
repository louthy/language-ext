using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct FSet<A, B> :
    Functor<Set<A>, Set<B>, A, B>
{
    [Pure]
    public static Set<B> Map(Set<A> ma, Func<A, B> f) =>
        ma.Map(f);
}

public struct FSet<OrdA, OrdB, A, B> : Functor<Set<OrdA, A>, Set<OrdB, B>, A, B>
    where OrdA : Ord<A>
    where OrdB : Ord<B>
{
    [Pure]
    public static Set<OrdB, B> Map(Set<OrdA, A> ma, Func<A, B> f) =>
        ma.Map<OrdB, B>(f);
}
