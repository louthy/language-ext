using System;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances;

public struct MapIndex<K, A> : Indexable<Map<K, A>, K, A>
{
    public static A Get(Map<K, A> ma, K key) =>
        TryGet(ma, key).IfNone(() => throw new IndexOutOfRangeException(nameof(key)));

    [Pure]
    public static Option<A> TryGet(Map<K, A> ma, K key) =>
        ma.Find(key);
}
