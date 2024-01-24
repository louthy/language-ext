using System;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances;

public struct ArrayIndex<A> : Indexable<A[], int, A>
{
    public static A Get(A[] ma, int key) =>
        TryGet(ma, key).IfNone(() => throw new IndexOutOfRangeException(nameof(key)));

    [Pure]
    public static Option<A> TryGet(A[] ma, int key) =>
        key < 0 || key >= ma.Length
            ? None
            : Some(ma[key]);
}
