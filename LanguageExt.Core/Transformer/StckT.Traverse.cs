using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class StckT
    {
        public static Stck<Arr<B>> Traverse<A, B>(this Arr<Stck<A>> ma, Func<A, B> f)
        {
            var res = new Arr<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = xs.Map(f).ToArray();
                ix++;
            }
            return toStack(res);            
        }
        
        public static Stck<Either<L, B>> Traverse<L, A, B>(this Either<L, Stck<A>> ma, Func<A, B> f) =>
            ma.Match(
                Left: _ => Stck<Either<L, B>>.Empty,
                Right: xs => toStack(xs.Map(x => Right<L, B>(f(x)))));

        public static Stck<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, Stck<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                Left: _ => Stck<EitherUnsafe<L, B>>.Empty,
                Right: xs => toStack(xs.Map(x => RightUnsafe<L, B>(f(x)))));

        public static Stck<HashSet<B>> Traverse<A, B>(this HashSet<Stck<A>> ma, Func<A, B> f)
        {
            var res = new HashSet<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toHashSet(xs.AsEnumerable().Map(f));
                ix++;
            }
            return toStack<HashSet<B>>(res);
        }

        public static Stck<Identity<B>> Traverse<A, B>(this Identity<Stck<A>> ma, Func<A, B> f) =>
            toStack(ma.Value.Map(a => new Identity<B>(f(a))));

        public static Stck<Lst<B>> Traverse<A, B>(this Lst<Stck<A>> ma, Func<A, B> f)
        {
            var res = new Lst<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toList(xs.AsEnumerable().Map(f));
                ix++;
            }
            return toStack<Lst<B>>(res);
        }

        public static Stck<Option<B>> Traverse<A, B>(this Option<Stck<A>> ma, Func<A, B> f) =>
            ma.Match(
                None: () => Stck<Option<B>>.Empty,
                Some: xs => toStack(xs.Map(x => Some(f(x)))));

        public static Stck<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<Stck<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                None: () => Stck<OptionUnsafe<B>>.Empty,
                Some: xs => toStack(xs.Map(x => SomeUnsafe(f(x)))));

        public static Stck<Que<B>> Traverse<A, B>(this Que<Stck<A>> ma, Func<A, B> f)
        {
            var res = new Que<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toQueue(xs.AsEnumerable().Map(f));
                ix++;
            }
            return toStack<Que<B>>(res);
        }

        public static Stck<Seq<B>> Traverse<A, B>(this Seq<Stck<A>> ma, Func<A, B> f)
        {
            var res = new Seq<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = xs.AsEnumerable().Map(f).ToSeq();
                ix++;
            }
            return toStack<Seq<B>>(res);
        }

        public static Stck<IEnumerable<B>> Traverse<A, B>(this IEnumerable<Stck<A>> ma, Func<A, B> f)
        {
            var res = new List<IEnumerable<B>>();
            foreach (var xs in ma)
            {
                res.Add(xs.AsEnumerable().Map(f).ToSeq());
            }
            return toStack<IEnumerable<B>>(res);
        }

        public static Stck<Set<B>> Traverse<A, B>(this Set<Stck<A>> ma, Func<A, B> f)
        {
            var res = new Set<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toSet(xs.AsEnumerable().Map(f));
                ix++;
            }
            return toStack<Set<B>>(res);
        }

        public static Stck<Stck<B>> Traverse<A, B>(this Stck<Stck<A>> ma, Func<A, B> f)
        {
            var res = new Stck<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toStack(xs.AsEnumerable().Map(f));
                ix++;
            }
            return toStack<Stck<B>>(res);
        }

        public static Stck<Try<B>> Traverse<A, B>(this Try<Stck<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => Stck<Try<B>>.Empty,
                Succ: xs => toStack(xs.Map(x => Try<B>(f(x)))));

        public static Stck<TryOption<B>> Traverse<A, B>(this TryOption<Stck<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => Stck<TryOption<B>>.Empty,
                None: () => Stck<TryOption<B>>.Empty,
                Some: xs => toStack(xs.Map(x => TryOption<B>(f(x)))));

        public static Stck<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, Stck<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: _ => Stck<Validation<Fail, B>>.Empty,
                Succ: xs => toStack(xs.Map(x => Success<Fail, B>(f(x)))));

        public static Stck<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, Stck<A>> ma, Func<A, B> f) 
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ma.Match(
                Fail: __ => Stck<Validation<MonoidFail, Fail, B>>.Empty,
                Succ: xs => toStack(xs.Map(x => Success<MonoidFail, Fail, B>(f(x)))));
    }
}
