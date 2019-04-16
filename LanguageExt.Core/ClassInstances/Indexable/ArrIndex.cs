using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public struct ArrIndex<A> : Indexable<Arr<A>, int, A>
    {
        public A Get(Arr<A> ma, int key) =>
            TryGet(ma, key).IfNone(() => throw new IndexOutOfRangeException(nameof(key)));

        [Pure]
        public Option<A> TryGet(Arr<A> ma, int key) =>
            ma == null || key < 0 || key >= ma.Count
                ? None
                : Some(ma[key]);
    }
}
