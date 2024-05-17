using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt.ClassInstances;

public struct SetIndex<A> : Indexable<Set<A>, A, A>
{
    public static A Get(Set<A> ma, A key) =>
        TryGet(ma, key).IfNone(() => throw new IndexOutOfRangeException(nameof(key)));

    [Pure]
    public static Option<A> TryGet(Set<A> ma, A key) =>
        ma.Find(key);
}
