#nullable enable
using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class IEnumerableT
    {
        public static IEnumerable<Arr<B>> Traverse<A, B>(this Arr<IEnumerable<A>> ma, Func<A, B> f) =>
            CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                .Map(toArray);

        public static IEnumerable<Either<L, B>> Traverse<L, A, B>(this Either<L, IEnumerable<A>> ma, Func<A, B> f) =>
            ma.Match(
                Left: e => new[] {Left<L, B>(e)},
                Right: xs => xs.Map(x => Right<L, B>(f(x))));

        public static IEnumerable<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, IEnumerable<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                Left: e => new[] {LeftUnsafe<L, B>(e)},
                Right: xs => xs.Map(x => RightUnsafe<L, B>(f(x))));

        public static IEnumerable<HashSet<B>> Traverse<A, B>(this HashSet<IEnumerable<A>> ma, Func<A, B> f) =>
            CollT.AllCombinationsOf(ma.ToArray().Map(xs => xs.ToList()).ToArray(), f)
                .Map(toHashSet);

        public static IEnumerable<Identity<B>> Traverse<A, B>(this Identity<IEnumerable<A>> ma, Func<A, B> f) =>
            ma.Value.Map(a => new Identity<B>(f(a)));

        public static IEnumerable<Lst<B>> Traverse<A, B>(this Lst<IEnumerable<A>> ma, Func<A, B> f) =>
            CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToSystemArray(), f)
                .Map(toList);

        public static IEnumerable<Fin<B>> Traverse<A, B>(this Fin<IEnumerable<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: er => new[] {Fin<B>.Fail(er)},
                Succ: xs => xs.Map(x => FinSucc(f(x))));

        public static IEnumerable<Option<B>> Traverse<A, B>(this Option<IEnumerable<A>> ma, Func<A, B> f) =>
            ma.Match(
                None: () => new[] {Option<B>.None},
                Some: xs => xs.Map(x => Some(f(x))));

        public static IEnumerable<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<IEnumerable<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                None: () => new[] {OptionUnsafe<B>.None},
                Some: xs => xs.Map(x => SomeUnsafe(f(x))));

        public static IEnumerable<Que<B>> Traverse<A, B>(this Que<IEnumerable<A>> ma, Func<A, B> f) =>
            CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                .Map(toQueue);
        
        public static IEnumerable<Seq<B>> Traverse<A, B>(this Seq<IEnumerable<A>> ma, Func<A, B> f) =>
            CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                .Map(toSeq);

        public static IEnumerable<IEnumerable<B>> Traverse<A, B>(this IEnumerable<IEnumerable<A>> ma, Func<A, B> f) =>
            CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f);

        public static IEnumerable<Set<B>> Traverse<A, B>(this Set<IEnumerable<A>> ma, Func<A, B> f) =>
            CollT.AllCombinationsOf(ma.ToArray().Map(xs => xs.ToList()).ToArray(), f)
                .Map(toSet);

        public static IEnumerable<Stck<B>> Traverse<A, B>(this Stck<IEnumerable<A>> ma, Func<A, B> f) =>
            CollT.AllCombinationsOf(ma.Reverse().Map(xs => xs.ToList()).ToArray(), f)
                .Map(toStack);

        public static IEnumerable<Try<B>> Traverse<A, B>(this Try<IEnumerable<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => new[] {TryFail<B>(ex)},
                Succ: xs => xs.Map(x => Try<B>(f(x))));

        public static IEnumerable<TryOption<B>> Traverse<A, B>(this TryOption<IEnumerable<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => new[] {TryOptionFail<B>(ex)},
                None: () => new[] {TryOptional<B>(None)},
                Some: xs => xs.Map(x => TryOption<B>(f(x))));

        public static IEnumerable<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, IEnumerable<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: es => new[] {Validation<Fail, B>.Fail(es)},
                Succ: xs => xs.Map(x => Success<Fail, B>(f(x))));

        public static IEnumerable<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, IEnumerable<A>> ma, Func<A, B> f) 
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ma.Match(
                Fail: es => new[] {Validation<MonoidFail, Fail, B>.Fail(es)},
                Succ: xs => xs.Map(x => Success<MonoidFail, Fail, B>(f(x))));

        public static IEnumerable<Eff<B>> Traverse<A, B>(this Eff<IEnumerable<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => new [] { FailEff<B>(ex) },
                Succ: xs => xs.Map(x => SuccessEff<B>(f(x)))).Run().Value;    
    }
}
