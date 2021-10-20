using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public struct HashSetIndex<A> : Indexable<HashSet<A>, A, A>
    {
        public A Get(HashSet<A> ma, A key) =>
            TryGet(ma, key).IfNone(() => throw new IndexOutOfRangeException(nameof(key)));

        [Pure]
        public Option<A> TryGet(HashSet<A> ma, A key) =>
            ma.Find(key);
    }
}
