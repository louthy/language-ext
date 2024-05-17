using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt.ClassInstances;

public struct HashSetIndex<A> : Indexable<HashSet<A>, A, A>
{
    public static A Get(HashSet<A> ma, A key) =>
        TryGet(ma, key).IfNone(() => throw new IndexOutOfRangeException(nameof(key)));

    [Pure]
    public static Option<A> TryGet(HashSet<A> ma, A key) =>
        ma.Find(key);
}
