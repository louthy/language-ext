using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public struct MHashMap<K, V> :
        Foldable<HashMap<K, V>, V>,
        Monoid<HashMap<K, V>>,
        Eq<HashMap<K, V>>
    {
        public static readonly MHashMap<K, V> Inst = default(MHashMap<K, V>);

        [Pure]
        public HashMap<K, V> Append(HashMap<K, V> x, HashMap<K, V> y) =>
            x + y;

        [Pure]
        public int Count(HashMap<K, V> fa) =>
            fa.Count;

        [Pure]
        public HashMap<K, V> Subtract(HashMap<K, V> x, HashMap<K, V> y) =>
            x - y;

        [Pure]
        public HashMap<K, V> Empty() =>
            HashMap<K, V>.Empty;

        [Pure]
        public bool Equals(HashMap<K, V> x, HashMap<K, V> y) =>
            x == y;

        [Pure]
        public S Fold<S>(HashMap<K, V> fa, S state, Func<S, V, S> f) =>
            fa.Fold(state, f);

        [Pure]
        public S FoldBack<S>(HashMap<K, V> fa, S state, Func<S, V, S> f) =>
            fa.Values.Reverse().FoldBack(state, f);
    }
}
