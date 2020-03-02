using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class HashSetTExtensions
    {
        public static HashSet<Arr<B>> Traverse<A, B>(this Arr<HashSet<A>> ma, Func<A, B> f)
        {
            var res = new Arr<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = xs.Map(f).ToArray();
                ix++;
            }
            return toHashSet(res);            
        }
        
        public static HashSet<Either<L, B>> Traverse<L, A, B>(this Either<L, HashSet<A>> ma, Func<A, B> f) =>
            ma.Match(
                Left: _ => HashSet<Either<L, B>>.Empty,
                Right: xs => xs.Map(x => Right<L, B>(f(x))));

        public static HashSet<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, HashSet<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                Left: _ => HashSet<EitherUnsafe<L, B>>.Empty,
                Right: xs => xs.Map(x => RightUnsafe<L, B>(f(x))));

        public static HashSet<HashSet<B>> Traverse<A, B>(this HashSet<HashSet<A>> ma, Func<A, B> f)
        {
            var res = new HashSet<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toHashSet(xs.AsEnumerable().Map(f));
                ix++;
            }
            return toHashSet<HashSet<B>>(res);
        }

        public static HashSet<Identity<B>> Traverse<A, B>(this Identity<HashSet<A>> ma, Func<A, B> f) =>
            ma.Value.Map(a => new Identity<B>(f(a)));

        public static HashSet<Lst<B>> Traverse<A, B>(this Lst<HashSet<A>> ma, Func<A, B> f)
        {
            var res = new Lst<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toList(xs.AsEnumerable().Map(f));
                ix++;
            }
            return toHashSet<Lst<B>>(res);
        }

        public static HashSet<Option<B>> Traverse<A, B>(this Option<HashSet<A>> ma, Func<A, B> f) =>
            ma.Match(
                None: () => HashSet<Option<B>>.Empty,
                Some: xs => xs.Map(x => Some(f(x))));

        public static HashSet<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<HashSet<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                None: () => HashSet<OptionUnsafe<B>>.Empty,
                Some: xs => xs.Map(x => SomeUnsafe(f(x))));

        public static HashSet<Que<B>> Traverse<A, B>(this Que<HashSet<A>> ma, Func<A, B> f)
        {
            var res = new Que<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toQueue(xs.AsEnumerable().Map(f));
                ix++;
            }
            return toHashSet<Que<B>>(res);
        }

        public static HashSet<Seq<B>> Traverse<A, B>(this Seq<HashSet<A>> ma, Func<A, B> f)
        {
            var res = new Seq<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = xs.AsEnumerable().Map(f).ToSeq();
                ix++;
            }
            return toHashSet<Seq<B>>(res);
        }

        public static HashSet<Set<B>> Traverse<A, B>(this Set<HashSet<A>> ma, Func<A, B> f)
        {
            var res = new Set<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toSet(xs.AsEnumerable().Map(f));
                ix++;
            }
            return toHashSet<Set<B>>(res);
        }

        public static HashSet<Stck<B>> Traverse<A, B>(this Stck<HashSet<A>> ma, Func<A, B> f)
        {
            var res = new Stck<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toStack(xs.AsEnumerable().Map(f));
                ix++;
            }
            return toHashSet<Stck<B>>(res);
        }

        public static HashSet<Try<B>> Traverse<A, B>(this Try<HashSet<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => HashSet<Try<B>>.Empty,
                Succ: xs => xs.Map(x => Try<B>(f(x))));

        public static HashSet<TryOption<B>> Traverse<A, B>(this TryOption<HashSet<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => HashSet<TryOption<B>>.Empty,
                None: () => HashSet<TryOption<B>>.Empty,
                Some: xs => xs.Map(x => TryOption<B>(f(x))));

        public static HashSet<Validation<L, B>> Traverse<L, A, B>(this Validation<L, HashSet<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: _ => HashSet<Validation<L, B>>.Empty,
                Succ: xs => xs.Map(x => Success<L, B>(f(x))));

        public static HashSet<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, HashSet<A>> ma, Func<A, B> f) 
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ma.Match(
                Fail: __ => HashSet<Validation<MonoidFail, Fail, B>>.Empty,
                Succ: xs => xs.Map(x => Success<MonoidFail, Fail, B>(f(x))));
    }
}
