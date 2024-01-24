using System;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances;

public struct HashMapIndex<K, A> : Indexable<HashMap<K, A>, K, A>
{
    public static A Get(HashMap<K, A> ma, K key) =>
        TryGet(ma, key).IfNone(() => throw new IndexOutOfRangeException(nameof(key)));

    [Pure]
    public static Option<A> TryGet(HashMap<K, A> ma, K key) =>
        ma.Find(key);
}
