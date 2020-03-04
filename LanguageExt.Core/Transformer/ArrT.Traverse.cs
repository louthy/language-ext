using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class ArrT
    {
        public static Arr<Arr<B>> Traverse<A, B>(this Arr<Arr<A>> ma, Func<A, B> f)
        {
            var res = new Arr<B>[ma.Count];
            var ix = 0;
            foreach (var arr in ma)
            {
                res[ix] = arr.Map(f);
                ix++;
            }
            return new Arr<Arr<B>>(res);            
        }
        
        public static Arr<Either<L, B>> Traverse<L, A, B>(this Either<L, Arr<A>> ma, Func<A, B> f) =>
            ma.Match(
                Left: _ => Arr<Either<L, B>>.Empty,
                Right: xs => xs.Map(x => Right<L, B>(f(x))));

        public static Arr<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, Arr<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                Left: _ => Arr<EitherUnsafe<L, B>>.Empty,
                Right: xs => xs.Map(x => RightUnsafe<L, B>(f(x))));

        public static Arr<HashSet<B>> Traverse<A, B>(this HashSet<Arr<A>> ma, Func<A, B> f)
        {
            var res = new HashSet<B>[ma.Count];
            var ix = 0;
            foreach (var arr in ma)
            {
                res[ix] = toHashSet(arr.AsEnumerable().Map(f));
                ix++;
            }
            return new Arr<HashSet<B>>(res);
        }

        public static Arr<Identity<B>> Traverse<A, B>(this Identity<Arr<A>> ma, Func<A, B> f) =>
            ma.Value.Map(a => new Identity<B>(f(a)));

        public static Arr<Lst<B>> Traverse<A, B>(this Lst<Arr<A>> ma, Func<A, B> f)
        {
            var res = new Lst<B>[ma.Count];
            var ix = 0;
            foreach (var arr in ma)
            {
                res[ix] = toList(arr.AsEnumerable().Map(f));
                ix++;
            }
            return new Arr<Lst<B>>(res);
        }

        public static Arr<Option<B>> Traverse<A, B>(this Option<Arr<A>> ma, Func<A, B> f) =>
            ma.Match(
                None: () => Arr<Option<B>>.Empty,
                Some: xs => xs.Map(x => Some(f(x))));

        public static Arr<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<Arr<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                None: () => Arr<OptionUnsafe<B>>.Empty,
                Some: xs => xs.Map(x => SomeUnsafe(f(x))));

        public static Arr<Que<B>> Traverse<A, B>(this Que<Arr<A>> ma, Func<A, B> f)
        {
            var res = new Que<B>[ma.Count];
            var ix = 0;
            foreach (var arr in ma)
            {
                res[ix] = toQueue(arr.AsEnumerable().Map(f));
                ix++;
            }
            return new Arr<Que<B>>(res);
        }

        public static Arr<Seq<B>> Traverse<A, B>(this Seq<Arr<A>> ma, Func<A, B> f)
        {
            var res = new List<Seq<B>>();
            foreach (var arr in ma)
            {
                res.Add(Seq(arr.AsEnumerable().Map(f)));
            }
            return new Arr<Seq<B>>(res);
        }

        public static Arr<IEnumerable<B>> Traverse<A, B>(this IEnumerable<Arr<A>> ma, Func<A, B> f)
        {
            var res = new List<IEnumerable<B>>();
            foreach (var arr in ma)
            {
                res.Add(arr.AsEnumerable().Map(f));
            }
            return new Arr<IEnumerable<B>>(res);
        }

        public static Arr<Set<B>> Traverse<A, B>(this Set<Arr<A>> ma, Func<A, B> f)
        {
            var res = new Set<B>[ma.Count];
            var ix = 0;
            foreach (var arr in ma)
            {
                res[ix] = toSet(arr.AsEnumerable().Map(f));
                ix++;
            }
            return new Arr<Set<B>>(res);
        }

        public static Arr<Stck<B>> Traverse<A, B>(this Stck<Arr<A>> ma, Func<A, B> f)
        {
            var res = new Stck<B>[ma.Count];
            var ix = 0;
            foreach (var arr in ma)
            {
                res[ix] = toStack(arr.AsEnumerable().Map(f));
                ix++;
            }
            return new Arr<Stck<B>>(res);
        }

        public static Arr<Try<B>> Traverse<A, B>(this Try<Arr<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => Arr<Try<B>>.Empty,
                Succ: xs => xs.Map(x => Try<B>(f(x))));

        public static Arr<TryOption<B>> Traverse<A, B>(this TryOption<Arr<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => Arr<TryOption<B>>.Empty,
                None: () => Arr<TryOption<B>>.Empty,
                Some: xs => xs.Map(x => TryOption<B>(f(x))));

        public static Arr<Validation<L, B>> Traverse<L, A, B>(this Validation<L, Arr<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: _ => Arr<Validation<L, B>>.Empty,
                Succ: xs => xs.Map(x => Success<L, B>(f(x))));

        public static Arr<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, Arr<A>> ma, Func<A, B> f) 
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ma.Match(
                Fail: __ => Arr<Validation<MonoidFail, Fail, B>>.Empty,
                Succ: xs => xs.Map(x => Success<MonoidFail, Fail, B>(f(x))));
    }
}
