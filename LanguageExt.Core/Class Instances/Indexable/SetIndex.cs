using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public struct SetIndex<A> : Indexable<Set<A>, A, A>
    {
        public A Get(Set<A> ma, A key) =>
            TryGet(ma, key).IfNone(() => throw new IndexOutOfRangeException(nameof(key)));

        [Pure]
        public Option<A> TryGet(Set<A> ma, A key) =>
            ma.Find(key);
    }
}
