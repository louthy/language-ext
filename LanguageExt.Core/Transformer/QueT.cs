using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class QueTExtensions
    {
        public static Que<Arr<B>> Traverse<A, B>(this Arr<Que<A>> ma, Func<A, B> f)
        {
            var res = new Arr<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = xs.Map(f).ToArray();
                ix++;
            }
            return toQueue(res);            
        }
        
        public static Que<Either<L, B>> Traverse<L, A, B>(this Either<L, Que<A>> ma, Func<A, B> f) =>
            ma.Match(
                Left: _ => Que<Either<L, B>>.Empty,
                Right: xs => new Que<Either<L, B>>(xs.Map(x => Right<L, B>(f(x)))));

        public static Que<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, Que<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                Left: _ => Que<EitherUnsafe<L, B>>.Empty,
                Right: xs => new Que<EitherUnsafe<L, B>>(xs.Map(x => RightUnsafe<L, B>(f(x)))));

        public static Que<HashSet<B>> Traverse<A, B>(this HashSet<Que<A>> ma, Func<A, B> f)
        {
            var res = new HashSet<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toHashSet(xs.AsEnumerable().Map(f));
                ix++;
            }
            return new Que<HashSet<B>>(res);
        }

        public static Que<Identity<B>> Traverse<A, B>(this Identity<Que<A>> ma, Func<A, B> f) =>
            toQueue(ma.Value.Map(a => new Identity<B>(f(a))));

        public static Que<Lst<B>> Traverse<A, B>(this Lst<Que<A>> ma, Func<A, B> f)
        {
            var res = new Lst<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toList(xs.AsEnumerable().Map(f));
                ix++;
            }
            return new Que<Lst<B>>(res);
        }

        public static Que<Option<B>> Traverse<A, B>(this Option<Que<A>> ma, Func<A, B> f) =>
            ma.Match(
                None: () => Que<Option<B>>.Empty,
                Some: xs => toQueue(xs.Map(x => Some(f(x)))));

        public static Que<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<Que<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                None: () => Que<OptionUnsafe<B>>.Empty,
                Some: xs => toQueue(xs.Map(x => SomeUnsafe(f(x)))));

        public static Que<Que<B>> Traverse<A, B>(this Que<Que<A>> ma, Func<A, B> f)
        {
            var res = new Que<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toQueue(xs.AsEnumerable().Map(f));
                ix++;
            }
            return new Que<Que<B>>(res);
        }

        public static Que<Seq<B>> Traverse<A, B>(this Seq<Que<A>> ma, Func<A, B> f)
        {
            var res = new Seq<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = xs.AsEnumerable().Map(f).ToSeq();
                ix++;
            }
            return new Que<Seq<B>>(res);
        }

        public static Que<IEnumerable<B>> Traverse<A, B>(this IEnumerable<Que<A>> ma, Func<A, B> f)
        {
            var res = new List<IEnumerable<B>>();
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = xs.AsEnumerable().Map(f).ToSeq();
                ix++;
            }
            return new Que<IEnumerable<B>>(res);
        }        

        public static Que<Set<B>> Traverse<A, B>(this Set<Que<A>> ma, Func<A, B> f)
        {
            var res = new Set<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toSet(xs.AsEnumerable().Map(f));
                ix++;
            }
            return new Que<Set<B>>(res);
        }

        public static Que<Stck<B>> Traverse<A, B>(this Stck<Que<A>> ma, Func<A, B> f)
        {
            var res = new Stck<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toStack(xs.AsEnumerable().Map(f));
                ix++;
            }
            return new Que<Stck<B>>(res);
        }

        public static Que<Try<B>> Traverse<A, B>(this Try<Que<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => Que<Try<B>>.Empty,
                Succ: xs => toQueue(xs.Map(x => Try<B>(f(x)))));

        public static Que<TryOption<B>> Traverse<A, B>(this TryOption<Que<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => Que<TryOption<B>>.Empty,
                None: () => Que<TryOption<B>>.Empty,
                Some: xs => toQueue(xs.Map(x => TryOption<B>(f(x)))));

        public static Que<Validation<L, B>> Traverse<L, A, B>(this Validation<L, Que<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: _ => Que<Validation<L, B>>.Empty,
                Succ: xs => toQueue(xs.Map(x => Success<L, B>(f(x)))));

        public static Que<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, Que<A>> ma, Func<A, B> f) 
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ma.Match(
                Fail: __ => Que<Validation<MonoidFail, Fail, B>>.Empty,
                Succ: xs => toQueue(xs.Map(x => Success<MonoidFail, Fail, B>(f(x)))));
    }
}
