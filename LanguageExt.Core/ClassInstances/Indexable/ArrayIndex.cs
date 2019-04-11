using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public struct ArrayIndex<A> : Indexable<A[], int, A>
    {
        public A Get(A[] ma, int key) =>
            TryGet(ma, key).IfNone(() => throw new IndexOutOfRangeException(nameof(key)));

        [Pure]
        public Option<A> TryGet(A[] ma, int key) =>
            ma == null || key < 0 || key >= ma.Length
                ? None
                : Some(ma[key]);
    }
}
