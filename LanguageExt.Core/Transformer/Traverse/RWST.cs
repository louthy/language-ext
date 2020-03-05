using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class RwsT
    {
        //
        // Collections
        //
        
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
            SequenceFast(ma.Reverse()).Map(toStack);

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
            TraverseFast(ma.Reverse(), f).Map(toStack);

        public static RWS<MonoidW, R, W, S, IEnumerable<B>> Traverse<MonoidW, R, W, S, A, B>(this IEnumerable<RWS<MonoidW, R, W, S, A>> ma, Func<A, B> f) where MonoidW : struct, Monoid<W> =>
            TraverseFast(ma, f).Map(Enumerable.AsEnumerable);        
    }
}
