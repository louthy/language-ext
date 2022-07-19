using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;
using System.Text;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static class AtomExtensions
    {
        /// <summary>
        /// Observe changes to the `Atom`
        /// </summary>
        /// <param name="atom">Atom to observe</param>
        /// <typeparam name="A">Value type</typeparam>
        /// <returns>Observable atom</returns>
        [Pure]
        public static IObservable<A> OnChange<A>(this Atom<A> atom) =>
            Observable.FromEvent<A>(
                add => atom.Change += new AtomChangedEvent<A>(add),
                remove => atom.Change -= new AtomChangedEvent<A>(remove));

        /// <summary>
        /// Observe changes to the `Atom`
        /// </summary>
        /// <param name="atom">Atom to observe</param>
        /// <typeparam name="A">Value type</typeparam>
        /// <returns>Observable atom</returns>
        [Pure]
        public static IObservable<A> OnChange<M, A>(this Atom<M, A> atom) =>
            Observable.FromEvent<A>(
                add => atom.Change += new AtomChangedEvent<A>(add),
                remove => atom.Change -= new AtomChangedEvent<A>(remove));

        /// <summary>
        /// Observe changes to the `Ref`
        /// </summary>
        /// <param name="value">Ref to observe</param>
        /// <typeparam name="A">Value type</typeparam>
        /// <returns>Observable ref</returns>
        [Pure]
        public static IObservable<A> OnChange<A>(this Ref<A> atom) =>
            Observable.FromEvent<A>(
                add => atom.Change += new AtomChangedEvent<A>(add),
                remove => atom.Change -= new AtomChangedEvent<A>(remove));

        /// <summary>
        /// Observe changes to the `AtomHashMap`
        /// </summary>
        /// <remarks>This publishes the full patch of a change, which may contain multiple
        /// key updates (if done from within a transaction for-example).</remarks>
        /// <param name="atom">`AtomHashMap` to observe</param>
        /// <typeparam name="A">Value type</typeparam>
        /// <returns>Observable `AtomHashMap`</returns>
        [Pure]
        public static IObservable<HashMapPatch<K, V>> OnChange<K, V>(this AtomHashMap<K, V> atom) =>
            Observable.FromEvent<HashMapPatch<K, V>>(
                add    => atom.Change += new AtomHashMapChangeEvent<K, V>(add),
                remove => atom.Change -= new AtomHashMapChangeEvent<K, V>(remove));

        /// <summary>
        /// Observe changes to the `AtomHashMap`
        /// </summary>
        /// <remarks>This publishes the full patch of a change, which may contain multiple
        /// key updates (if done from within a transaction for-example).</remarks>
        /// <param name="atom">`AtomHashMap` to observe</param>
        /// <typeparam name="A">Value type</typeparam>
        /// <returns>Observable `AtomHashMap`</returns>
        [Pure]
        public static IObservable<HashMapPatch<EqK, K, V>> OnChange<EqK, K, V>(
            this AtomHashMap<EqK, K, V> atom)
            where EqK : struct, Eq<K> =>
            Observable.FromEvent<HashMapPatch<EqK, K, V>>(
                add => atom.Change += new AtomHashMapChangeEvent<EqK, K, V>(add),
                remove => atom.Change -= new AtomHashMapChangeEvent<EqK, K, V>(remove));

        /// <summary>
        /// Observe changes to the `AtomHashMap`
        /// </summary>
        /// <remarks>This publishes the changes to individual key-values within the `AtomHashMap`</remarks>
        /// <typeparam name="A">Value type</typeparam>
        /// <returns>Observable `(K, Change<V>)`</returns>
        [Pure]
        public static IObservable<(K, Change<V>)> OnEntryChange<K, V>(this AtomHashMap<K, V> atom) =>
            atom.OnChange()
                .SelectMany(static p =>
                    p.Changes
                     .AsEnumerable()
                     .Filter(static c => c.Value.HasChanged));

        /// <summary>
        /// Observe changes to the `AtomHashMap`
        /// </summary>
        /// <remarks>This publishes the changes to individual key-values within the `AtomHashMap`</remarks>
        /// <typeparam name="A">Value type</typeparam>
        /// <returns>Observable `(K, Change<V>)`</returns>
        [Pure]
        public static IObservable<(K, Change<V>)> OnEntryChange<EqK, K, V>(this AtomHashMap<EqK, K, V> atom)
            where EqK : struct, Eq<K> =>
            atom.OnChange()
                .SelectMany(static p =>
                    p.Changes
                     .AsEnumerable()
                     .Filter(static c => c.Value.HasChanged));

        /// <summary>
        /// Observe changes to the `AtomHashMap`
        /// </summary>
        /// <remarks>This publishes the latest state of an `AtomHashMap`</remarks>
        /// <param name="atom">`AtomHashMap` to observe</param>
        /// <typeparam name="A">Value type</typeparam>
        /// <returns>Observable `HashMap`</returns>
        [Pure]
        public static IObservable<HashMap<EqK, K, V>> OnMapChange<EqK, K, V>(this AtomHashMap<EqK, K, V> atom)
            where EqK : struct, Eq<K> =>
            atom.OnChange().Select(p => p.To);
    }
}
