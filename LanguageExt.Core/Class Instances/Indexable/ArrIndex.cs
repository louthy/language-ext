using System;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances;

public struct ArrIndex<A> : Indexable<Arr<A>, int, A>
{
    public static A Get(Arr<A> ma, int key) =>
        TryGet(ma, key).IfNone(() => throw new IndexOutOfRangeException(nameof(key)));

    [Pure]
    public static Option<A> TryGet(Arr<A> ma, int key) =>
        key < 0 || key >= ma.Count
            ? None
            : Some(ma[key]);
}
