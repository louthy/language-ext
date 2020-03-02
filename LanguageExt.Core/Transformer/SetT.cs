using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class SetTExtensions
    {
        public static Set<Arr<B>> Traverse<A, B>(this Arr<Set<A>> ma, Func<A, B> f)
        {
            var res = new Arr<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = xs.Map(f).ToArray();
                ix++;
            }
            return toSet(res);            
        }
        
        public static Set<Either<L, B>> Traverse<L, A, B>(this Either<L, Set<A>> ma, Func<A, B> f) =>
            ma.Match(
                Left: _ => Set<Either<L, B>>.Empty,
                Right: xs => xs.Map(x => Right<L, B>(f(x))));

        public static Set<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, Set<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                Left: _ => Set<EitherUnsafe<L, B>>.Empty,
                Right: xs => xs.Map(x => RightUnsafe<L, B>(f(x))));

        public static Set<HashSet<B>> Traverse<A, B>(this HashSet<Set<A>> ma, Func<A, B> f)
        {
            var res = new HashSet<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toHashSet(xs.AsEnumerable().Map(f));
                ix++;
            }
            return toSet<HashSet<B>>(res);
        }

        public static Set<Identity<B>> Traverse<A, B>(this Identity<Set<A>> ma, Func<A, B> f) =>
            ma.Value.Map(a => new Identity<B>(f(a)));

        public static Set<Lst<B>> Traverse<A, B>(this Lst<Set<A>> ma, Func<A, B> f)
        {
            var res = new Lst<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toList(xs.AsEnumerable().Map(f));
                ix++;
            }
            return toSet<Lst<B>>(res);
        }

        public static Set<Option<B>> Traverse<A, B>(this Option<Set<A>> ma, Func<A, B> f) =>
            ma.Match(
                None: () => Set<Option<B>>.Empty,
                Some: xs => xs.Map(x => Some(f(x))));

        public static Set<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<Set<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                None: () => Set<OptionUnsafe<B>>.Empty,
                Some: xs => xs.Map(x => SomeUnsafe(f(x))));

        public static Set<Que<B>> Traverse<A, B>(this Que<Set<A>> ma, Func<A, B> f)
        {
            var res = new Que<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toQueue(xs.AsEnumerable().Map(f));
                ix++;
            }
            return toSet<Que<B>>(res);
        }

        public static Set<Seq<B>> Traverse<A, B>(this Seq<Set<A>> ma, Func<A, B> f)
        {
            var res = new Seq<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = xs.AsEnumerable().Map(f).ToSeq();
                ix++;
            }
            return toSet<Seq<B>>(res);
        }

        public static Set<Set<B>> Traverse<A, B>(this Set<Set<A>> ma, Func<A, B> f)
        {
            var res = new Set<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toSet(xs.AsEnumerable().Map(f));
                ix++;
            }
            return toSet<Set<B>>(res);
        }

        public static Set<Stck<B>> Traverse<A, B>(this Stck<Set<A>> ma, Func<A, B> f)
        {
            var res = new Stck<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toStack(xs.AsEnumerable().Map(f));
                ix++;
            }
            return toSet<Stck<B>>(res);
        }

        public static Set<Try<B>> Traverse<A, B>(this Try<Set<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => Set<Try<B>>.Empty,
                Succ: xs => xs.Map(x => Try<B>(f(x))));

        public static Set<TryOption<B>> Traverse<A, B>(this TryOption<Set<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => Set<TryOption<B>>.Empty,
                None: () => Set<TryOption<B>>.Empty,
                Some: xs => xs.Map(x => TryOption<B>(f(x))));

        public static Set<Validation<L, B>> Traverse<L, A, B>(this Validation<L, Set<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: _ => Set<Validation<L, B>>.Empty,
                Succ: xs => xs.Map(x => Success<L, B>(f(x))));

        public static Set<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, Set<A>> ma, Func<A, B> f) 
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ma.Match(
                Fail: __ => Set<Validation<MonoidFail, Fail, B>>.Empty,
                Succ: xs => xs.Map(x => Success<MonoidFail, Fail, B>(f(x))));
    }
}
