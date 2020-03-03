using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class IEnumerableTExtensions
    {
        public static IEnumerable<Arr<B>> Traverse<A, B>(this Arr<IEnumerable<A>> ma, Func<A, B> f)
        {
            var res = new Arr<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = xs.Map(f).ToArray();
                ix++;
            }
            return res;
        }
        
        public static IEnumerable<Either<L, B>> Traverse<L, A, B>(this Either<L, IEnumerable<A>> ma, Func<A, B> f) =>
            ma.Match(
                Left: _ => Seq<Either<L, B>>.Empty,
                Right: xs => xs.Map(x => Right<L, B>(f(x))));

        public static IEnumerable<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, IEnumerable<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                Left: _ => Seq<EitherUnsafe<L, B>>.Empty,
                Right: xs => xs.Map(x => RightUnsafe<L, B>(f(x))));

        public static IEnumerable<HashSet<B>> Traverse<A, B>(this HashSet<IEnumerable<A>> ma, Func<A, B> f)
        {
            var res = new HashSet<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toHashSet(xs.AsEnumerable().Map(f));
                ix++;
            }
            return res;
        }

        public static IEnumerable<Identity<B>> Traverse<A, B>(this Identity<IEnumerable<A>> ma, Func<A, B> f) =>
            ma.Value.Map(a => new Identity<B>(f(a)));

        public static IEnumerable<Lst<B>> Traverse<A, B>(this Lst<IEnumerable<A>> ma, Func<A, B> f)
        {
            var res = new Lst<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toList(xs.AsEnumerable().Map(f));
                ix++;
            }
            return res;
        }

        public static IEnumerable<Option<B>> Traverse<A, B>(this Option<IEnumerable<A>> ma, Func<A, B> f) =>
            ma.Match(
                None: () => Seq<Option<B>>.Empty,
                Some: xs => xs.Map(x => Some(f(x))));

        public static IEnumerable<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<IEnumerable<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                None: () => Seq<OptionUnsafe<B>>.Empty,
                Some: xs => xs.Map(x => SomeUnsafe(f(x))));

        public static IEnumerable<Que<B>> Traverse<A, B>(this Que<IEnumerable<A>> ma, Func<A, B> f)
        {
            var res = new Que<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toQueue(xs.AsEnumerable().Map(f));
                ix++;
            }
            return res;
        }

        public static IEnumerable<Seq<B>> Traverse<A, B>(this Seq<IEnumerable<A>> ma, Func<A, B> f)
        {
            var res = new Seq<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = xs.Map(f).ToSeq();
                ix++;
            }
            return res;
        }

        public static IEnumerable<IEnumerable<B>> Traverse<A, B>(this IEnumerable<IEnumerable<A>> ma, Func<A, B> f)
        {
            var res = new List<IEnumerable<B>>();
            foreach (var xs in ma)
            {
                res.Add(xs.Map(f));
            }
            return res;
        }

        public static IEnumerable<Set<B>> Traverse<A, B>(this Set<IEnumerable<A>> ma, Func<A, B> f)
        {
            var res = new Set<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toSet(xs.AsEnumerable().Map(f));
                ix++;
            }
            return res;
        }

        public static IEnumerable<Stck<B>> Traverse<A, B>(this Stck<IEnumerable<A>> ma, Func<A, B> f)
        {
            var res = new Stck<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toStack(xs.AsEnumerable().Map(f));
                ix++;
            }
            return res;
        }

        public static IEnumerable<Try<B>> Traverse<A, B>(this Try<IEnumerable<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => Seq<Try<B>>.Empty,
                Succ: xs => xs.Map(x => Try<B>(f(x))));

        public static IEnumerable<TryOption<B>> Traverse<A, B>(this TryOption<IEnumerable<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => Seq<TryOption<B>>.Empty,
                None: () => Seq<TryOption<B>>.Empty,
                Some: xs => xs.Map(x => TryOption<B>(f(x))));

        public static IEnumerable<Validation<L, B>> Traverse<L, A, B>(this Validation<L, IEnumerable<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: _ => Seq<Validation<L, B>>.Empty,
                Succ: xs => xs.Map(x => Success<L, B>(f(x))));

        public static IEnumerable<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, IEnumerable<A>> ma, Func<A, B> f) 
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ma.Match(
                Fail: __ => Seq<Validation<MonoidFail, Fail, B>>.Empty,
                Succ: xs => xs.Map(x => Success<MonoidFail, Fail, B>(f(x))));
    }
}
