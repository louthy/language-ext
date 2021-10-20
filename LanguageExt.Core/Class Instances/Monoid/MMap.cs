using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public struct MMap<K, V> :
        Foldable<Map<K, V>, V>,
        Monoid<Map<K, V>>,
        Eq<Map<K, V>>
    {
        public static readonly MMap<K, V> Inst = default(MMap<K, V>);

        [Pure]
        public Map<K, V> Append(Map<K, V> x, Map<K, V> y) =>
            x + y;

        [Pure]
        public Func<Unit, int> Count(Map<K, V> fa) => _ =>
            fa.Count;

        [Pure]
        public Map<K, V> Subtract(Map<K, V> x, Map<K, V> y) =>
            x - y;

        [Pure]
        public Map<K, V> Empty() =>
            Map<K, V>.Empty;

        [Pure]
        public bool Equals(Map<K, V> x, Map<K, V> y) =>
            x == y;

        [Pure]
        public Func<Unit, S> Fold<S>(Map<K, V> fa, S state, Func<S, V, S> f) => _ =>
             fa.Fold(state, f);

        [Pure]
        public Func<Unit, S> FoldBack<S>(Map<K, V> fa, S state, Func<S, V, S> f) => _ =>
            fa.Values.Reverse().FoldBack(state, f);

        [Pure]
        public int GetHashCode(Map<K, V> x) =>
            x.GetHashCode();
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(Map<K, V> x, Map<K, V> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Map<K, V> x) =>
            GetHashCode(x).AsTask();       
    }
}
