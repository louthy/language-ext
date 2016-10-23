using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.Instances
{
    public struct MHashMap<K, V> :
        Foldable<HashMap<K, V>, V>,
        Monoid<HashMap<K, V>>,
        Eq<HashMap<K, V>>
    {
        public HashMap<K, V> Append(HashMap<K, V> x, HashMap<K, V> y) =>
            x + y;

        public int Count(HashMap<K, V> fa) =>
            fa.Count;

        public HashMap<K, V> Subtract(HashMap<K, V> x, HashMap<K, V> y) =>
            x - y;

        public HashMap<K, V> Empty() =>
            HashMap<K, V>.Empty;

        public bool Equals(HashMap<K, V> x, HashMap<K, V> y) =>
            x == y;

        public S Fold<S>(HashMap<K, V> fa, S state, Func<S, V, S> f) =>
            fa.Fold(state, f);

        public S FoldBack<S>(HashMap<K, V> fa, S state, Func<S, V, S> f) =>
            fa.Values.Reverse().FoldBack(state, f);
    }
}
