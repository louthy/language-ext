using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;

namespace LanguageExt
{
    public static class AtomExtensions
    {
        public static IObservable<A> OnChange<A>(this Atom<A> atom) =>
            Observable.FromEvent<A>(
                add =>
                {
                    atom.Change += new AtomChangedEvent<A>(add);
                },
                remove =>
                {
                    atom.Change -= new AtomChangedEvent<A>(remove);
                });

        public static IObservable<A> OnChange<M, A>(this Atom<M, A> atom) =>
            Observable.FromEvent<A>(
                add =>
                {
                    atom.Change += new AtomChangedEvent<A>(add);
                },
                remove =>
                {
                    atom.Change -= new AtomChangedEvent<A>(remove);
                });

        public static IObservable<(HashMap<K, V> Previous, HashMap<K, V> Current, HashMap<K, Change<V>> Changes)> OnChange<K, V>(this AtomHashMap<K, V> atom) =>
            Observable.FromEvent<(HashMap<K, V> Previous, HashMap<K, V> Current, HashMap<K, Change<V>> Changes)>(
                add =>
                {
                    atom.Change += add.MapChange();
                },
                remove =>
                {
                    atom.Change -= remove.MapChange();
                });
        
        static AtomHashMapChangeEvent<K, V> MapChange<K, V>(
            this Action<(HashMap<K, V> Previous, HashMap<K, V> Current, HashMap<K, Change<V>> Changes)> self) =>
            new AtomHashMapChangeEvent<K, V>((previous, current, changes) => self((previous, current, changes)));
    }
}
