using System;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class Lens
    {
        /// <summary>
        /// Sequentially composes two lenses
        /// </summary>
        public static Lens<A, C> compose<A, B, C>(Lens<A, B> la, Lens<B, C> lb) =>
            Lens<A, C>.New(
                Get: a => lb.Get(la.Get(a)),
                Set: v => la.Update(lb.SetF(v)));

        /// <summary>
        /// Sequentially composes three lenses
        /// </summary>
        public static Lens<A, D> compose<A, B, C, D>(Lens<A, B> la, Lens<B, C> lb, Lens<C, D> lc) =>
            compose(compose(la, lb), lc);

        /// <summary>
        /// Sequentially composes four lenses
        /// </summary>
        public static Lens<A, E> compose<A, B, C, D, E>(Lens<A, B> la, Lens<B, C> lb, Lens<C, D> lc, Lens<D, E> ld) =>
            compose(compose(la, lb, lc), ld);

        /// <summary>
        /// Sequentially composes five lenses
        /// </summary>
        public static Lens<A, F> compose<A, B, C, D, E, F>(Lens<A, B> la, Lens<B, C> lb, Lens<C, D> lc, Lens<D, E> ld, Lens<E, F> le) =>
            compose(compose(la, lb, lc, ld), le);

        /// <summary>
        /// Sequentially composes six lenses
        /// </summary>
        public static Lens<A, G> compose<A, B, C, D, E, F, G>(Lens<A, B> la, Lens<B, C> lb, Lens<C, D> lc, Lens<D, E> ld, Lens<E, F> le, Lens<F, G> lf) =>
            compose(compose(la, lb, lc, ld, le), lf);

        /// <summary>
        /// Sequentially composes seven lenses
        /// </summary>
        public static Lens<A, H> compose<A, B, C, D, E, F, G, H>(Lens<A, B> la, Lens<B, C> lb, Lens<C, D> lc, Lens<D, E> ld, Lens<E, F> le, Lens<F, G> lf, Lens<G, H> lg) =>
            compose(compose(la, lb, lc, ld, le, lf), lg);

        /// <summary>
        /// Sequentially composes eight lenses
        /// </summary>
        public static Lens<A, I> compose<A, B, C, D, E, F, G, H, I>(Lens<A, B> la, Lens<B, C> lb, Lens<C, D> lc, Lens<D, E> ld, Lens<E, F> le, Lens<F, G> lf, Lens<G, H> lg, Lens<H, I> lh) =>
            compose(compose(la, lb, lc, ld, le, lf, lg), lh);

        /// <summary>
        /// Pair two lenses
        /// </summary>
        public static Lens<(A, B), (C, D)> pair<A, B, C, D>(Lens<A, C> First, Lens<B, D> Second) =>
            Lens<(A, B), (C, D)>.New(
                Get: a => (First.Get(a.Item1), Second.Get(a.Item2)),
                Set: v => a => (First.Set(v.Item1, a.Item1), Second.Set(v.Item2, a.Item2)));

        /// <summary>
        /// <paramref name="pred"/> is applied to source. 
        /// If true, <paramref name="Then"/> is selected.
        /// If false, <paramref name="Else"/> is selected.
        /// </summary>
        public static Lens<A, B> cond<A, B>(Func<A, bool> pred, Lens<A, B> Then, Lens<A, B> Else)
        {
            Lens<A, B> choose(A a) => pred(a) ? Then : Else;
            return Lens<A, B>.New(
                Get: a => choose(a).Get(a),
                Set: v => a => choose(a).Set(v, a));
        }

        /// <summary>
        /// Applies a lens in the 'get' direction within a state monad   
        /// </summary>
        public static State<A, B> getState<A, B>(Lens<A, B> la) =>
            get<A>().Map(la.Get);

        /// <summary>
        /// Applies a lens in the 'set' direction within a state monad
        /// </summary>
        public static State<A, Unit> putState<A, B>(Lens<A, B> la, B value) =>
            from a in get<A>()
            from _ in put(la.Set(value, a))
            select unit;

        /// <summary>
        /// Update through a lens within a state monad
        /// </summary>
        public static State<A, Unit> updateState<A, B>(Lens<A, B> la, Func<B, B> f) =>
            from b in getState(la)
            from _ in putState(la, f(b))
            select unit;

        /// <summary>
        /// Gets/sets the fst element in a pair
        /// </summary>
        public static Lens<(A, B), A> fst<A, B>() =>
            Lens<(A, B), A>.New(
                Get: ab => ab.Item1,
                Set: a => ab => (a, ab.Item2));

        /// <summary>
        /// Gets/sets the snd element in a pair
        /// </summary>
        public static Lens<(A, B), B> snd<A, B>() =>
            Lens<(A, B), B>.New(
                Get: ab => ab.Item2,
                Set: b => ab => (ab.Item1, b));

        /// <summary>
        /// Identity lens
        /// </summary>
        public static Lens<A, A> identity<A>() =>
            Lens<A, A>.New(
                Get: a => a,
                Set: a => _ => a);

        /// <summary>
        /// Lens for a particular value in a set
        /// </summary>
        public static Lens<Set<A>, bool> setItem<A>(A value) =>
            Lens<Set<A>, bool>.New(
                Get: set => set.Contains(value),
                Set: con => set => con ? set.Add(value) : set.Remove(value));

        /// <summary>
        /// Lens for a particular key in a map
        /// </summary>
        public static Lens<Map<K, V>, Option<V>> mapItem<K, V>(K key) =>
            Lens<Map<K, V>, Option<V>>.New(
                Get: map => map.Find(key),
                Set: val => map => val.Match(
                    Some: x => map.Add(key, x),
                    None: () => map.Remove(key)));

        /// <summary>
        /// Creates a lens that maps the given lens in a map
        /// </summary>
        public static Lens<Map<K, A>, Map<K, B>> mapMap<K, A, B>(Lens<A, B> la) =>
            Lens<Map<K, A>, Map<K, B>>.New(
                Get: m => m.Map(la.Get),
                Set: v => m => toMap(
                                m.Keys
                                 .Zip(m.Values.Zip(v.Values))
                                 .Map(kab => (kab.Item1, la.Set(kab.Item2.Item2, kab.Item2.Item1)))));

        /// <summary>
        /// Lens for a particular value in an array
        /// </summary>
        public static Lens<Arr<A>, A> arrItem<A>(int index) =>
            Lens<Arr<A>, A>.New(
                Get: arr => arr[index],
                Set: val => arr => arr.SetItem(index, val));

        /// <summary>
        /// Creates a lens that maps the given lens in an array
        /// </summary>
        public static Lens<Arr<A>, Arr<B>> arrMap<A, B>(Lens<A, B> la) =>
            Lens<Arr<A>, Arr<B>>.New(
                Get: lst => lst.Map(la.Get),
                Set: val => lst => lst.Zip(val).Map(ab => la.Set(ab.Item2, ab.Item1)).ToArr());

        /// <summary>
        /// Lens for a particular value in a list
        /// </summary>
        public static Lens<Lst<A>, A> listItem<A>(int index) =>
            Lens<Lst<A>, A>.New(
                Get: lst => lst[index],
                Set: val => lst => lst.SetItem(index, val));

        /// <summary>
        /// Creates a lens that maps the given lens in a list
        /// </summary>
        public static Lens<Lst<A>, Lst<B>> listMap<A, B>(Lens<A, B> la) =>
            Lens<Lst<A>, Lst<B>>.New(
                Get: lst => lst.Map(la.Get),
                Set: val => lst => lst.Zip(val).Map(ab => la.Set(ab.Item2, ab.Item1)).Freeze());

        /// <summary>
        /// Creates a lens that maps the given lens in an enumerable
        /// </summary>
        public static Lens<IEnumerable<A>, IEnumerable<B>> enumMap<A, B>(Lens<A, B> la) =>
            Lens<IEnumerable<A>, IEnumerable<B>>.New(
                Get: lst => lst.Map(la.Get),
                Set: val => lst => lst.Zip(val).Map(ab => la.Set(ab.Item2, ab.Item1)));

        /// <summary>
        /// Creates a lens that maps the given lens in a seq
        /// </summary>
        public static Lens<Seq<A>, Seq<B>> seqMap<A, B>(Lens<A, B> la) =>
            Lens<Seq<A>, Seq<B>>.New(
                Get: lst => lst.Map(la.Get),
                Set: val => lst => lst.Zip(val).Map(ab => la.Set(ab.Item2, ab.Item1)));
    }
}
