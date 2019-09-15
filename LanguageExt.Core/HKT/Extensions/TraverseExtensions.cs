using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class SeqWriterExtensions
    {
        internal static Writer<MonoidW, W, List<A>> SequenceFast<MonoidW, W, A>(this IEnumerable<Writer<MonoidW, W, A>> ma) where MonoidW : struct, Monoid<W> => () =>
        {
            var values = new List<A>();
            var output = default(MonoidW).Empty();
            foreach (var item in ma)
            {
                var (a, o, bottom) = item();
                if (bottom) return (Value: new List<A>(), Output: default(MonoidW).Empty(), IsBottom: true);
                values.Add(a);
                output = default(MonoidW).Append(output, o);
            }
            return (Value: values, Output: output, IsBottom: false);
        };

        internal static Writer<MonoidW, W, List<B>> TraverseFast<MonoidW, W, A, B>(this IEnumerable<Writer<MonoidW, W, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> => () =>
        {
            var values = new List<B>();
            var output = default(MonoidW).Empty();
            foreach (var item in ma)
            {
                var (a, o, bottom) = item();
                if (bottom) return (Value: new List<B>(), Output: default(MonoidW).Empty(), IsBottom: true);
                values.Add(f(a));
                output = default(MonoidW).Append(output, o);
            }
            return (Value: values, Output: output, IsBottom: false);
        };

        public static Writer<MonoidW, W, Seq<A>> Sequence<MonoidW, W, A>(this Seq<Writer<MonoidW, W, A>> ma) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ma).Map(Prelude.Seq);

        public static Writer<MonoidW, W, Lst<A>> Sequence<MonoidW, W, A>(this Lst<Writer<MonoidW, W, A>> ma) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ma).Map(toList);

        public static Writer<MonoidW, W, Arr<A>> Sequence<MonoidW, W, A>(this Arr<Writer<MonoidW, W, A>> ma) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ma).Map(toArray);

        public static Writer<MonoidW, W, A[]> Sequence<MonoidW, W, A>(this Writer<MonoidW, W, A>[] ma) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ma).Map(x => x.ToArray());

        public static Writer<MonoidW, W, Set<A>> Sequence<MonoidW, W, A>(this Set<Writer<MonoidW, W, A>> ma) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ma).Map(toSet);

        public static Writer<MonoidW, W, HashSet<A>> Sequence<MonoidW, W, A>(this HashSet<Writer<MonoidW, W, A>> ma) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ma).Map(toHashSet);

        public static Writer<MonoidW, W, Stck<A>> Sequence<MonoidW, W, A>(this Stck<Writer<MonoidW, W, A>> ma) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ma).Map(toStack);

        public static Writer<MonoidW, W, IEnumerable<A>> Sequence<MonoidW, W, A>(this IEnumerable<Writer<MonoidW, W, A>> ma) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ma).Map(Enumerable.AsEnumerable);


        public static Writer<MonoidW, W, Seq<B>> Traverse<MonoidW, W, A, B>(this Seq<Writer<MonoidW, W, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            TraverseFast(ma, f).Map(Prelude.Seq);

        public static Writer<MonoidW, W, Lst<B>> Traverse<MonoidW, W, A, B>(this Lst<Writer<MonoidW, W, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            TraverseFast(ma, f).Map(toList);

        public static Writer<MonoidW, W, Arr<B>> Traverse<MonoidW, W, A, B>(this Arr<Writer<MonoidW, W, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            TraverseFast(ma, f).Map(toArray);

        public static Writer<MonoidW, W, B[]> Traverse<MonoidW, W, A, B>(this Writer<MonoidW, W, A>[] ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            TraverseFast(ma, f).Map(x => x.ToArray());

        public static Writer<MonoidW, W, Set<B>> Traverse<MonoidW, W, A, B>(this Set<Writer<MonoidW, W, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            TraverseFast(ma, f).Map(toSet);

        public static Writer<MonoidW, W, HashSet<B>> Traverse<MonoidW, W, A, B>(this HashSet<Writer<MonoidW, W, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            TraverseFast(ma, f).Map(toHashSet);

        public static Writer<MonoidW, W, Stck<B>> Traverse<MonoidW, W, A, B>(this Stck<Writer<MonoidW, W, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            TraverseFast(ma, f).Map(toStack);

        public static Writer<MonoidW, W, IEnumerable<B>> Traverse<MonoidW, W, A, B>(this IEnumerable<Writer<MonoidW, W, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            TraverseFast(ma, f).Map(Enumerable.AsEnumerable);
    }

    public static class SeqReaderExtensions
    {
        internal static Reader<Env, List<A>> SequenceFast<Env, A>(this IEnumerable<Reader<Env, A>> ma) => env =>
        {
            var values = new List<A>();
            foreach (var item in ma)
            {
                var resA = item(env);
                if (resA.IsFaulted) return ReaderResult<List<A>>.New(resA.ErrorInt);
                values.Add(resA.Value);
            }
            return ReaderResult<List<A>>.New(values);
        };

        internal static Reader<Env, List<B>> TraverseFast<Env, A, B>(this IEnumerable<Reader<Env, A>> ma, Func<A, B> f) => env =>
        {
            var values = new List<B>();
            foreach (var item in ma)
            {
                var resA = item(env);
                if (resA.IsFaulted) return ReaderResult<List<B>>.New(resA.ErrorInt);
                values.Add(f(resA.Value));
            }
            return ReaderResult<List<B>>.New(values);
        };

        public static Reader<Env, Seq<A>> Sequence<Env, A>(this Seq<Reader<Env, A>> ma) =>
            SequenceFast(ma).Map(Prelude.Seq);

        public static Reader<Env, Lst<A>> Sequence<Env, A>(this Lst<Reader<Env, A>> ma) =>
            SequenceFast(ma).Map(toList);

        public static Reader<Env, Arr<A>> Sequence<Env, A>(this Arr<Reader<Env, A>> ma) =>
            SequenceFast(ma).Map(toArray);

        public static Reader<Env, A[]> Sequence<Env, A>(this Reader<Env, A>[] ma) =>
            SequenceFast(ma).Map(x => x.ToArray());

        public static Reader<Env, Set<A>> Sequence<Env, A>(this Set<Reader<Env, A>> ma) =>
            SequenceFast(ma).Map(toSet);

        public static Reader<Env, HashSet<A>> Sequence<Env, A>(this HashSet<Reader<Env, A>> ma) =>
            SequenceFast(ma).Map(toHashSet);

        public static Reader<Env, Stck<A>> Sequence<Env, A>(this Stck<Reader<Env, A>> ma) =>
            SequenceFast(ma).Map(toStack);

        public static Reader<Env, IEnumerable<A>> Sequence<Env, A>(this IEnumerable<Reader<Env, A>> ma) =>
            SequenceFast(ma).Map(Enumerable.AsEnumerable);


        public static Reader<Env, Seq<B>> Traverse<Env, A, B>(this Seq<Reader<Env, A>> ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(Prelude.Seq);

        public static Reader<Env, Lst<B>> Traverse<Env, A, B>(this Lst<Reader<Env, A>> ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(toList);

        public static Reader<Env, Arr<B>> Traverse<Env, A, B>(this Arr<Reader<Env, A>> ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(toArray);

        public static Reader<Env, B[]> Traverse<Env, A, B>(this Reader<Env, A>[] ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(x => x.ToArray());

        public static Reader<Env, Set<B>> Traverse<Env, A, B>(this Set<Reader<Env, A>> ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(toSet);

        public static Reader<Env, HashSet<B>> Traverse<Env, A, B>(this HashSet<Reader<Env, A>> ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(toHashSet);

        public static Reader<Env, Stck<B>> Traverse<Env, A, B>(this Stck<Reader<Env, A>> ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(toStack);

        public static Reader<Env, IEnumerable<B>> Traverse<Env, A, B>(this IEnumerable<Reader<Env, A>> ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(Enumerable.AsEnumerable);
    }

    public static class SeqStateExtensions
    {
        internal static State<S, List<A>> SequenceFast<S, A>(this IEnumerable<State<S, A>> ma) => state =>
        {
            var values = new List<A>();
            foreach (var item in ma)
            {
                var (a, s, bottom) = item(state);
                if (bottom) return (Value: new List<A>(), State: default(S), IsFaulted: true);
                state = s;
                values.Add(a);
            }
            return (Value: values, State: state, IsFaulted: false);
        };

        internal static State<S, List<B>> TraverseFast<S, A, B>(this IEnumerable<State<S, A>> ma, Func<A, B> f) => state =>
        {
            var values = new List<B>();
            foreach (var item in ma)
            {
                var (a, s, bottom) = item(state);
                if (bottom) return (Value: new List<B>(), State: default(S), IsFaulted: true);
                state = s;
                values.Add(f(a));
            }
            return (Value: values, State: state, IsFaulted: false);
        };

        public static State<S, Seq<A>> Sequence<S, A>(this Seq<State<S, A>> ma) =>
            SequenceFast(ma).Map(Prelude.Seq);

        public static State<S, Lst<A>> Sequence<S, A>(this Lst<State<S, A>> ma) =>
            SequenceFast(ma).Map(toList);

        public static State<S, Arr<A>> Sequence<S, A>(this Arr<State<S, A>> ma) =>
            SequenceFast(ma).Map(toArray);

        public static State<S, A[]> Sequence<S, A>(this State<S, A>[] ma) =>
            SequenceFast(ma).Map(x => x.ToArray());

        public static State<S, Set<A>> Sequence<S, A>(this Set<State<S, A>> ma) =>
            SequenceFast(ma).Map(toSet);

        public static State<S, HashSet<A>> Sequence<S, A>(this HashSet<State<S, A>> ma) =>
            SequenceFast(ma).Map(toHashSet);

        public static State<S, Stck<A>> Sequence<S, A>(this Stck<State<S, A>> ma) =>
            SequenceFast(ma).Map(toStack);

        public static State<S, IEnumerable<A>> Sequence<S, A>(this IEnumerable<State<S, A>> ma) =>
            SequenceFast(ma).Map(Enumerable.AsEnumerable);


        public static State<S, Seq<B>> Traverse<S, A, B>(this Seq<State<S, A>> ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(Prelude.Seq);

        public static State<S, Lst<B>> Traverse<S, A, B>(this Lst<State<S, A>> ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(toList);

        public static State<S, Arr<B>> Traverse<S, A, B>(this Arr<State<S, A>> ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(toArray);

        public static State<S, B[]> Traverse<S, A, B>(this State<S, A>[] ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(x => x.ToArray());

        public static State<S, Set<B>> Traverse<S, A, B>(this Set<State<S, A>> ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(toSet);

        public static State<S, HashSet<B>> Traverse<S, A, B>(this HashSet<State<S, A>> ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(toHashSet);

        public static State<S, Stck<B>> Traverse<S, A, B>(this Stck<State<S, A>> ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(toStack);

        public static State<S, IEnumerable<B>> Traverse<S, A, B>(this IEnumerable<State<S, A>> ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(Enumerable.AsEnumerable);
    }

    public static class SeqRwsExtensions
    {
        internal static RWS<MonoidW, R, W, S, List<A>> SequenceFast<MonoidW, R, W, S, A>(this IEnumerable<RWS<MonoidW, R, W, S, A>> ma) where MonoidW : struct, Monoid<W> => (env, state) =>
        {
            var values = new List<A>();
            var output = default(MonoidW).Empty();
            foreach (var item in ma)
            {
                var res = item(env, state);
                if (res.IsFaulted) return RWSResult<MonoidW, R, W, S, List<A>>.New(state, res.Error);
                values.Add(res.Value);
                state = res.State;
                output = default(MonoidW).Append(output, res.Output);
            }
            return RWSResult<MonoidW, R, W, S, List<A>>.New(output, state, values);
        };

        internal static RWS<MonoidW, R, W, S, List<B>> TraverseFast<MonoidW, R, W, S, A, B>(this IEnumerable<RWS<MonoidW, R, W, S, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> => (env, state) =>
        {
            var values = new List<B>();
            var output = default(MonoidW).Empty();
            foreach (var item in ma)
            {
                var res = item(env, state);
                if (res.IsFaulted) return RWSResult<MonoidW, R, W, S, List<B>>.New(state, res.Error);
                values.Add(f(res.Value));
                state = res.State;
                output = default(MonoidW).Append(output, res.Output);
            }
            return RWSResult<MonoidW, R, W, S, List<B>>.New(output, state, values);
        };

        public static RWS<MonoidW, R, W, S, Seq<A>> Sequence<MonoidW, R, W, S, A>(this Seq<RWS<MonoidW, R, W, S, A>> ma) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ma).Map(Prelude.Seq);

        public static RWS<MonoidW, R, W, S, Lst<A>> Sequence<MonoidW, R, W, S, A>(this Lst<RWS<MonoidW, R, W, S, A>> ma) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ma).Map(toList);

        public static RWS<MonoidW, R, W, S, Arr<A>> Sequence<MonoidW, R, W, S, A>(this Arr<RWS<MonoidW, R, W, S, A>> ma) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ma).Map(toArray);

        public static RWS<MonoidW, R, W, S, A[]> Sequence<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A>[] ma) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ma).Map(x => x.ToArray());

        public static RWS<MonoidW, R, W, S, Set<A>> Sequence<MonoidW, R, W, S, A>(this Set<RWS<MonoidW, R, W, S, A>> ma) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ma).Map(toSet);

        public static RWS<MonoidW, R, W, S, HashSet<A>> Sequence<MonoidW, R, W, S, A>(this HashSet<RWS<MonoidW, R, W, S, A>> ma) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ma).Map(toHashSet);

        public static RWS<MonoidW, R, W, S, Stck<A>> Sequence<MonoidW, R, W, S, A>(this Stck<RWS<MonoidW, R, W, S, A>> ma) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ma).Map(toStack);

        public static RWS<MonoidW, R, W, S, IEnumerable<A>> Sequence<MonoidW, R, W, S, A>(this IEnumerable<RWS<MonoidW, R, W, S, A>> ma) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ma).Map(Enumerable.AsEnumerable);


        public static RWS<MonoidW, R, W, S, Seq<B>> Traverse<MonoidW, R, W, S, A, B>(this Seq<RWS<MonoidW, R, W, S, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            TraverseFast(ma, f).Map(Prelude.Seq);

        public static RWS<MonoidW, R, W, S, Lst<B>> Traverse<MonoidW, R, W, S, A, B>(this Lst<RWS<MonoidW, R, W, S, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            TraverseFast(ma, f).Map(toList);

        public static RWS<MonoidW, R, W, S, Arr<B>> Traverse<MonoidW, R, W, S, A, B>(this Arr<RWS<MonoidW, R, W, S, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            TraverseFast(ma, f).Map(toArray);

        public static RWS<MonoidW, R, W, S, B[]> Traverse<MonoidW, R, W, S, A, B>(this RWS<MonoidW, R, W, S, A>[] ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            TraverseFast(ma, f).Map(x => x.ToArray());

        public static RWS<MonoidW, R, W, S, Set<B>> Traverse<MonoidW, R, W, S, A, B>(this Set<RWS<MonoidW, R, W, S, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            TraverseFast(ma, f).Map(toSet);

        public static RWS<MonoidW, R, W, S, HashSet<B>> Traverse<MonoidW, R, W, S, A, B>(this HashSet<RWS<MonoidW, R, W, S, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            TraverseFast(ma, f).Map(toHashSet);

        public static RWS<MonoidW, R, W, S, Stck<B>> Traverse<MonoidW, R, W, S, A, B>(this Stck<RWS<MonoidW, R, W, S, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            TraverseFast(ma, f).Map(toStack);

        public static RWS<MonoidW, R, W, S, IEnumerable<B>> Traverse<MonoidW, R, W, S, A, B>(this IEnumerable<RWS<MonoidW, R, W, S, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            TraverseFast(ma, f).Map(Enumerable.AsEnumerable);
    }
}
