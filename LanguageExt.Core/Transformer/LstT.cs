using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class LstTExtensions
    {
        public static Lst<Arr<B>> Traverse<A, B>(this Arr<Lst<A>> ma, Func<A, B> f)
        {
            var res = new Arr<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = xs.Map(f).ToArray();
                ix++;
            }
            return toList(res);            
        }
        
        public static Lst<Either<L, B>> Traverse<L, A, B>(this Either<L, Lst<A>> ma, Func<A, B> f) =>
            ma.Match(
                Left: _ => Lst<Either<L, B>>.Empty,
                Right: xs => xs.Map(x => Right<L, B>(f(x))));

        public static Lst<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, Lst<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                Left: _ => Lst<EitherUnsafe<L, B>>.Empty,
                Right: xs => xs.Map(x => RightUnsafe<L, B>(f(x))));

        public static Lst<HashSet<B>> Traverse<A, B>(this HashSet<Lst<A>> ma, Func<A, B> f)
        {
            var res = new HashSet<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toHashSet(xs.AsEnumerable().Map(f));
                ix++;
            }
            return new Lst<HashSet<B>>(res);
        }

        public static Lst<Identity<B>> Traverse<A, B>(this Identity<Lst<A>> ma, Func<A, B> f) =>
            ma.Value.Map(a => new Identity<B>(f(a)));

        public static Lst<Lst<B>> Traverse<A, B>(this Lst<Lst<A>> ma, Func<A, B> f)
        {
            var res = new Lst<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toList(xs.AsEnumerable().Map(f));
                ix++;
            }
            return new Lst<Lst<B>>(res);
        }

        public static Lst<Option<B>> Traverse<A, B>(this Option<Lst<A>> ma, Func<A, B> f) =>
            ma.Match(
                None: () => Lst<Option<B>>.Empty,
                Some: xs => xs.Map(x => Some(f(x))));

        public static Lst<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<Lst<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                None: () => Lst<OptionUnsafe<B>>.Empty,
                Some: xs => xs.Map(x => SomeUnsafe(f(x))));

        public static Lst<Que<B>> Traverse<A, B>(this Que<Lst<A>> ma, Func<A, B> f)
        {
            var res = new Que<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toQueue(xs.AsEnumerable().Map(f));
                ix++;
            }
            return new Lst<Que<B>>(res);
        }

        public static Lst<Seq<B>> Traverse<A, B>(this Seq<Lst<A>> ma, Func<A, B> f)
        {
            var res = new Seq<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = xs.AsEnumerable().Map(f).ToSeq();
                ix++;
            }
            return new Lst<Seq<B>>(res);
        }

        public static Lst<Set<B>> Traverse<A, B>(this Set<Lst<A>> ma, Func<A, B> f)
        {
            var res = new Set<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toSet(xs.AsEnumerable().Map(f));
                ix++;
            }
            return new Lst<Set<B>>(res);
        }

        public static Lst<Stck<B>> Traverse<A, B>(this Stck<Lst<A>> ma, Func<A, B> f)
        {
            var res = new Stck<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toStack(xs.AsEnumerable().Map(f));
                ix++;
            }
            return new Lst<Stck<B>>(res);
        }

        public static Lst<Try<B>> Traverse<A, B>(this Try<Lst<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => Lst<Try<B>>.Empty,
                Succ: xs => xs.Map(x => Try<B>(f(x))));

        public static Lst<TryOption<B>> Traverse<A, B>(this TryOption<Lst<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => Lst<TryOption<B>>.Empty,
                None: () => Lst<TryOption<B>>.Empty,
                Some: xs => xs.Map(x => TryOption<B>(f(x))));

        public static Lst<Validation<L, B>> Traverse<L, A, B>(this Validation<L, Lst<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: _ => Lst<Validation<L, B>>.Empty,
                Succ: xs => xs.Map(x => Success<L, B>(f(x))));

        public static Lst<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, Lst<A>> ma, Func<A, B> f) 
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ma.Match(
                Fail: __ => Lst<Validation<MonoidFail, Fail, B>>.Empty,
                Succ: xs => xs.Map(x => Success<MonoidFail, Fail, B>(f(x))));
    }
}
