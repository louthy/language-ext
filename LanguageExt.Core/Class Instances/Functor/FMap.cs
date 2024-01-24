using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct FMap<K, A, B> : 
    Functor<Map<K, A>, Map<K, B>, A, B>
{
    [Pure]
    public static Map<K, B> Map(Map<K, A> ma, Func<A, B> f) =>
        ma.Map(f);
}

public struct FMap<OrdK, K, A, B> :
    Functor<Map<OrdK, K, A>, Map<OrdK, K, B>, A, B>
    where OrdK : Ord<K>
{
    [Pure]
    public static Map<OrdK, K, B> Map(Map<OrdK, K, A> ma, Func<A, B> f) =>
        ma.Map(f);
}
