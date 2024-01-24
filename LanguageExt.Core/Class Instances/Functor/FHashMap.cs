using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct FHashMap<K, A, B> : 
    Functor<HashMap<K, A>, HashMap<K, B>, A, B>
{
    [Pure]
    public static HashMap<K, B> Map(HashMap<K, A> ma, Func<A, B> f) =>
        ma.Map(f);
}

public struct FHashMap<EqK, K, A, B> :
    Functor<HashMap<EqK, K, A>, HashMap<EqK, K, B>, A, B>
    where EqK : Eq<K>
{
    [Pure]
    public static HashMap<EqK, K, B> Map(HashMap<EqK, K, A> ma, Func<A, B> f) =>
        ma.Map(f);
}
