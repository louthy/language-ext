using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances;

public struct SeqIndex<A> : Indexable<Seq<A>, int, A>
{
    public static A Get(Seq<A> ma, int key) =>
        TryGet(ma, key).IfNone(() => throw new IndexOutOfRangeException(nameof(key)));

    [Pure]
    public static Option<A> TryGet(Seq<A> ma, int key) =>
        key < 0 || key >= ma.Count
            ? None
            : Some(ma[key]);
}
