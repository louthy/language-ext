#nullable enable
using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class StateT
    {
        public static State<S, Seq<B>> Traverse<S, A, B>(this Seq<State<S, A>> ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(Prelude.toSeq);

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
            TraverseFast(ma.Reverse(), f).Map(toStack);

        public static State<S, IEnumerable<B>> Traverse<S, A, B>(this IEnumerable<State<S, A>> ma, Func<A, B> f) =>
            TraverseFast(ma, f).Map(Enumerable.AsEnumerable);    

        internal static State<S, List<B>> TraverseFast<S, A, B>(this IEnumerable<State<S, A>> ma, Func<A, B> f) => state =>
        {
            var values = new List<B>();
            foreach (var item in ma)
            {
                var (a, s, bottom) = item(state);
                if (bottom) return (Value: new List<B>(), State: default(S), IsFaulted: true);
                state = s is null ? state : s;
                values.Add(f(a));
            }
            return (Value: values, State: state, IsFaulted: false);
        };
    }
}
