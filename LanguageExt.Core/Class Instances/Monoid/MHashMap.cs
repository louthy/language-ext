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
        public Func<Unit, int> Count(HashMap<K, V> fa) => _ =>
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
        public Func<Unit, S> Fold<S>(HashMap<K, V> fa, S state, Func<S, V, S> f) => _ =>
            fa.Fold(state, f);

        [Pure]
        public Func<Unit, S> FoldBack<S>(HashMap<K, V> fa, S state, Func<S, V, S> f) => _ =>
            fa.Values.Reverse().FoldBack(state, f);

        [Pure]
        public int GetHashCode(HashMap<K, V> x) =>
            x.GetHashCode();
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(HashMap<K, V> x, HashMap<K, V> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(HashMap<K, V> x) =>
            GetHashCode(x).AsTask();       
    }
}
