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
        public static Stck<Arr<B>> Traverse<A, B>(this Arr<Stck<A>> ma, Func<A, B> f) =>
            toStack(CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                .Map(toArray));
        
        public static Stck<Either<L, B>> Traverse<L, A, B>(this Either<L, Stck<A>> ma, Func<A, B> f) =>
            ma.Match(
                Left: _ => Stck<Either<L, B>>.Empty,
                Right: xs => toStack(xs.Map(x => Right<L, B>(f(x)))));

        public static Stck<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, Stck<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                Left: _ => Stck<EitherUnsafe<L, B>>.Empty,
                Right: xs => toStack(xs.Map(x => RightUnsafe<L, B>(f(x)))));

        public static Stck<HashSet<B>> Traverse<A, B>(this HashSet<Stck<A>> ma, Func<A, B> f) =>
            toStack(CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                .Map(toHashSet));

        public static Stck<Identity<B>> Traverse<A, B>(this Identity<Stck<A>> ma, Func<A, B> f) =>
            toStack(ma.Value.Map(a => new Identity<B>(f(a))));

        public static Stck<Lst<B>> Traverse<A, B>(this Lst<Stck<A>> ma, Func<A, B> f) =>
            toStack(CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToSystemArray(), f)
                .Map(toList));

        public static Stck<Option<B>> Traverse<A, B>(this Option<Stck<A>> ma, Func<A, B> f) =>
            ma.Match(
                None: () => Stck<Option<B>>.Empty,
                Some: xs => toStack(xs.Map(x => Some(f(x)))));

        public static Stck<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<Stck<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                None: () => Stck<OptionUnsafe<B>>.Empty,
                Some: xs => toStack(xs.Map(x => SomeUnsafe(f(x)))));

        public static Stck<Que<B>> Traverse<A, B>(this Que<Stck<A>> ma, Func<A, B> f) =>
            toStack(CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                .Map(toQueue));

        public static Stck<Seq<B>> Traverse<A, B>(this Seq<Stck<A>> ma, Func<A, B> f) =>
            toStack(CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                .Map(xs => Seq(xs)));

        public static Stck<IEnumerable<B>> Traverse<A, B>(this IEnumerable<Stck<A>> ma, Func<A, B> f) =>
            toStack(CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                .Map(xs => xs.AsEnumerable()));

        public static Stck<Set<B>> Traverse<A, B>(this Set<Stck<A>> ma, Func<A, B> f) =>
            toStack(CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                .Map(toSet));

        public static Stck<Stck<B>> Traverse<A, B>(this Stck<Stck<A>> ma, Func<A, B> f) =>
            toStack(CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                .Map(toStack));

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
