using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class SeqT
    {
        public static Seq<Arr<B>> Traverse<A, B>(this Arr<Seq<A>> ma, Func<A, B> f)
        {
            var res = new Arr<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = xs.Map(f).ToArray();
                ix++;
            }
            return Seq.FromArray(res);            
        }
        
        public static Seq<Either<L, B>> Traverse<L, A, B>(this Either<L, Seq<A>> ma, Func<A, B> f) =>
            ma.Match(
                Left: _ => Seq<Either<L, B>>.Empty,
                Right: xs => xs.Map(x => Right<L, B>(f(x))));

        public static Seq<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, Seq<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                Left: _ => Seq<EitherUnsafe<L, B>>.Empty,
                Right: xs => xs.Map(x => RightUnsafe<L, B>(f(x))));

        public static Seq<HashSet<B>> Traverse<A, B>(this HashSet<Seq<A>> ma, Func<A, B> f)
        {
            var res = new HashSet<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toHashSet(xs.AsEnumerable().Map(f));
                ix++;
            }
            return Seq.FromArray<HashSet<B>>(res);
        }

        public static Seq<Identity<B>> Traverse<A, B>(this Identity<Seq<A>> ma, Func<A, B> f) =>
            ma.Value.Map(a => new Identity<B>(f(a)));

        public static Seq<Lst<B>> Traverse<A, B>(this Lst<Seq<A>> ma, Func<A, B> f)
        {
            var res = new Lst<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toList(xs.AsEnumerable().Map(f));
                ix++;
            }
            return Seq.FromArray<Lst<B>>(res);
        }

        public static Seq<Option<B>> Traverse<A, B>(this Option<Seq<A>> ma, Func<A, B> f) =>
            ma.Match(
                None: () => Seq<Option<B>>.Empty,
                Some: xs => xs.Map(x => Some(f(x))));

        public static Seq<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<Seq<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                None: () => Seq<OptionUnsafe<B>>.Empty,
                Some: xs => xs.Map(x => SomeUnsafe(f(x))));

        public static Seq<Que<B>> Traverse<A, B>(this Que<Seq<A>> ma, Func<A, B> f)
        {
            var res = new Que<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toQueue(xs.AsEnumerable().Map(f));
                ix++;
            }
            return Seq.FromArray<Que<B>>(res);
        }

        public static Seq<Seq<B>> Traverse<A, B>(this Seq<Seq<A>> ma, Func<A, B> f)
        {
            var res = new Seq<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = xs.Map(f);
                ix++;
            }
            return Seq.FromArray<Seq<B>>(res);
        }

        public static Seq<IEnumerable<B>> Traverse<A, B>(this IEnumerable<Seq<A>> ma, Func<A, B> f)
        {
            var res = new List<IEnumerable<B>>();
            foreach (var xs in ma)
            {
                res.Add(xs.Map(f));
            }
            return Seq.FromArray(res.ToArray());
        }

        public static Seq<Set<B>> Traverse<A, B>(this Set<Seq<A>> ma, Func<A, B> f)
        {
            var res = new Set<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toSet(xs.AsEnumerable().Map(f));
                ix++;
            }
            return Seq.FromArray<Set<B>>(res);
        }

        public static Seq<Stck<B>> Traverse<A, B>(this Stck<Seq<A>> ma, Func<A, B> f)
        {
            var res = new Stck<B>[ma.Count];
            var ix = 0;
            foreach (var xs in ma)
            {
                res[ix] = toStack(xs.AsEnumerable().Map(f));
                ix++;
            }
            return Seq.FromArray<Stck<B>>(res);
        }

        public static Seq<Try<B>> Traverse<A, B>(this Try<Seq<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => Seq<Try<B>>.Empty,
                Succ: xs => xs.Map(x => Try<B>(f(x))));

        public static Seq<TryOption<B>> Traverse<A, B>(this TryOption<Seq<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => Seq<TryOption<B>>.Empty,
                None: () => Seq<TryOption<B>>.Empty,
                Some: xs => xs.Map(x => TryOption<B>(f(x))));

        public static Seq<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, Seq<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: _ => Seq<Validation<Fail, B>>.Empty,
                Succ: xs => xs.Map(x => Success<Fail, B>(f(x))));

        public static Seq<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, Seq<A>> ma, Func<A, B> f) 
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ma.Match(
                Fail: __ => Seq<Validation<MonoidFail, Fail, B>>.Empty,
                Succ: xs => xs.Map(x => Success<MonoidFail, Fail, B>(f(x))));
    }
}
