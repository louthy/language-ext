using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public struct MapIndex<K, A> : Indexable<Map<K, A>, K, A>
    {
        public A Get(Map<K, A> ma, K key) =>
            TryGet(ma, key).IfNone(() => throw new IndexOutOfRangeException(nameof(key)));

        [Pure]
        public Option<A> TryGet(Map<K, A> ma, K key) =>
            ma.Find(key);
    }
}
