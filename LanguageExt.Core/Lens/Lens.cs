using System;
using System.Collections.Generic;

namespace LanguageExt
{
    public static class Lens
    {
        /// <summary>
        /// Pair two lenses
        /// </summary>
        public static Lens<(A, B), (C, D)> tuple<A, B, C, D>(Lens<A, C> First, Lens<B, D> Second) =>
            Lens<(A, B), (C, D)>.New(
                Get: a => (First.Get(a.Item1), Second.Get(a.Item2)),
                Set: v => a => (First.Set(v.Item1, a.Item1), Second.Set(v.Item2, a.Item2)));

        /// <summary>
        /// Triple three lenses
        /// </summary>
        public static Lens<(A, B, C), (D, E, F)> tuple<A, B, C, D, E, F>(Lens<A, D> First, Lens<B, E> Second, Lens<C, F> Third) =>
            Lens<(A, B, C), (D, E, F)>.New(
                Get: a => (First.Get(a.Item1), Second.Get(a.Item2), Third.Get(a.Item3)),
                Set: v => a => (First.Set(v.Item1, a.Item1), Second.Set(v.Item2, a.Item2), Third.Set(v.Item3, a.Item3)));

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
        /// Gets/sets the fst element in a pair
        /// </summary>
        public static Lens<(A, B), A> fst<A, B>() =>
            Lens<(A, B), A>.New(
                Get: ab => ab.Item1,
                Set: a => ab => (a, ab.Item2));

        /// <summary>
        /// Gets/sets the fst element in a triple
        /// </summary>
        public static Lens<(A, B, C), A> fst<A, B, C>() =>
            Lens<(A, B, C), A>.New(
                Get: ab => ab.Item1,
                Set: a => ab => (a, ab.Item2, ab.Item3));

        /// <summary>
        /// Gets/sets the fst element in a quad
        /// </summary>
        public static Lens<(A, B, C, D), A> fst<A, B, C, D>() =>
            Lens<(A, B, C, D), A>.New(
                Get: ab => ab.Item1,
                Set: a => ab => (a, ab.Item2, ab.Item3, ab.Item4));

        /// <summary>
        /// Gets/sets the snd element in a pair
        /// </summary>
        public static Lens<(A, B), B> snd<A, B>() =>
            Lens<(A, B), B>.New(
                Get: ab => ab.Item2,
                Set: b => ab => (ab.Item1, b));

        /// <summary>
        /// Gets/sets the snd element in a pair
        /// </summary>
        public static Lens<(A, B, C), B> snd<A, B, C>() =>
            Lens<(A, B, C), B>.New(
                Get: ab => ab.Item2,
                Set: b => ab => (ab.Item1, b, ab.Item3));

        /// <summary>
        /// Gets/sets the snd element in a pair
        /// </summary>
        public static Lens<(A, B, C, D), B> snd<A, B, C, D>() =>
            Lens<(A, B, C, D), B>.New(
                Get: ab => ab.Item2,
                Set: b => ab => (ab.Item1, b, ab.Item3, ab.Item4));


        /// <summary>
        /// Gets/sets the thrd element in a pair
        /// </summary>
        public static Lens<(A, B, C), C> thrd<A, B, C>() =>
            Lens<(A, B, C), C>.New(
                Get: abc => abc.Item3,
                Set: c => abc => (abc.Item1, abc.Item2, c));

        /// <summary>
        /// Gets/sets the thrd element in a pair
        /// </summary>
        public static Lens<(A, B, C, D), C> thrd<A, B, C, D>() =>
            Lens<(A, B, C, D), C>.New(
                Get: abc => abc.Item3,
                Set: c => abc => (abc.Item1, abc.Item2, c, abc.Item4));

        /// <summary>
        /// Identity lens
        /// </summary>
        public static Lens<A, A> identity<A>() =>
            Lens<A, A>.New(
                Get: a => a,
                Set: a => _ => a);

        /// <summary>
        /// Creates a lens that maps the given lens in an enumerable
        /// </summary>
        public static Lens<IEnumerable<A>, IEnumerable<B>> enumMap<A, B>(Lens<A, B> la) =>
            Lens<IEnumerable<A>, IEnumerable<B>>.New(
                Get: lst => lst.Map(la.Get),
                Set: val => lst => lst.Zip(val).Map(ab => la.Set(ab.Item2, ab.Item1)));
    }
}
