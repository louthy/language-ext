using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public struct LstIndex<A> : Indexable<Lst<A>, int, A>
    {
        public A Get(Lst<A> ma, int key) =>
            TryGet(ma, key).IfNone(() => throw new IndexOutOfRangeException(nameof(key)));

        [Pure]
        public Option<A> TryGet(Lst<A> ma, int key) =>
            key < 0 || key >= ma.Count
                ? None
                : Some(ma[key]);
    }
}
