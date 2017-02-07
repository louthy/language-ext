using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public struct MMap<K, V> :
        Foldable<Map<K, V>, V>,
        Monoid<Map<K, V>>,
        Eq<Map<K, V>>
    {
        public static readonly MMap<K, V> Inst = default(MMap<K, V>);

        public Map<K, V> Append(Map<K, V> x, Map<K, V> y) =>
            x + y;

        public int Count(Map<K, V> fa) =>
            fa.Count;

        public Map<K, V> Subtract(Map<K, V> x, Map<K, V> y) =>
            x - y;

        public Map<K, V> Empty() =>
            Map<K, V>.Empty;

        public bool Equals(Map<K, V> x, Map<K, V> y) =>
            x == y;

        public S Fold<S>(Map<K, V> fa, S state, Func<S, V, S> f) =>
            fa.Fold(state, f);

        public S FoldBack<S>(Map<K, V> fa, S state, Func<S, V, S> f) =>
            fa.Values.Reverse().FoldBack(state, f);

        public int GetHashCode(Map<K, V> x) =>
            x.GetHashCode();
    }
}
