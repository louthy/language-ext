using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public struct HashMapIndex<K, A> : Indexable<HashMap<K, A>, K, A>
    {
        public A Get(HashMap<K, A> ma, K key) =>
            TryGet(ma, key).IfNone(() => throw new IndexOutOfRangeException(nameof(key)));

        [Pure]
        public Option<A> TryGet(HashMap<K, A> ma, K key) =>
            ma.Find(key);
    }
}
