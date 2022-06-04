#nullable enable
using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class QueT
    {
        public static Que<Arr<B>> Traverse<A, B>(this Arr<Que<A>> ma, Func<A, B> f) =>
            toQueue(CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                         .Map(toArray));
        
        public static Que<Either<L, B>> Traverse<L, A, B>(this Either<L, Que<A>> ma, Func<A, B> f) =>
            ma.Match(
                Left:  ex => Queue(Either<L, B>.Left(ex)),
                Right: xs => new Que<Either<L, B>>(xs.Map(x => Right<L, B>(f(x)))));

        public static Que<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, Que<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                Left:  ex => Queue(EitherUnsafe<L, B>.Left(ex)),
                Right: xs => new Que<EitherUnsafe<L, B>>(xs.Map(x => RightUnsafe<L, B>(f(x)))));

        public static Que<HashSet<B>> Traverse<A, B>(this HashSet<Que<A>> ma, Func<A, B> f) =>
            toQueue(CollT.AllCombinationsOf(ma.ToArray().Map(xs => xs.ToList()).ToArray(), f)
                .Map(toHashSet));

        public static Que<Identity<B>> Traverse<A, B>(this Identity<Que<A>> ma, Func<A, B> f) =>
            toQueue(ma.Value.Map(a => new Identity<B>(f(a))));

        public static Que<Lst<B>> Traverse<A, B>(this Lst<Que<A>> ma, Func<A, B> f) =>
            toQueue(CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToSystemArray(), f)
                .Map(toList));

        public static Que<Fin<B>> Traverse<A, B>(this Fin<Que<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: er => Queue(FinFail<B>(er)),
                Succ: xs => xs.Map(x => FinSucc(f(x))));

        public static Que<Option<B>> Traverse<A, B>(this Option<Que<A>> ma, Func<A, B> f) =>
            ma.Match(
                None: () => Queue(Option<B>.None),
                Some: xs => xs.Map(x => Some(f(x))));

        public static Que<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<Que<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                None: () => Queue(OptionUnsafe<B>.None),
                Some: xs => xs.Map(x => SomeUnsafe(f(x))));

        public static Que<Que<B>> Traverse<A, B>(this Que<Que<A>> ma, Func<A, B> f) =>
            toQueue(CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                .Map(toQueue));

        public static Que<Seq<B>> Traverse<A, B>(this Seq<Que<A>> ma, Func<A, B> f) =>
            toQueue(CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                .Map(toSeq));

        public static Que<IEnumerable<B>> Traverse<A, B>(this IEnumerable<Que<A>> ma, Func<A, B> f) =>
            toQueue(CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                .Map(xs => xs.AsEnumerable()));

        public static Que<Set<B>> Traverse<A, B>(this Set<Que<A>> ma, Func<A, B> f) =>
            toQueue(CollT.AllCombinationsOf(ma.ToArray().Map(xs => xs.ToList()).ToArray(), f)
                .Map(toSet));

        public static Que<Stck<B>> Traverse<A, B>(this Stck<Que<A>> ma, Func<A, B> f) =>
            toQueue(CollT.AllCombinationsOf(ma.Reverse().Map(xs => xs.ToList()).ToArray(), f)
                .Map(toStack));

        public static Que<Try<B>> Traverse<A, B>(this Try<Que<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => Queue(TryFail<B>(ex)),
                Succ: xs => xs.Map(x => Try<B>(f(x))));

        public static Que<TryOption<B>> Traverse<A, B>(this TryOption<Que<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => Queue(TryOptionFail<B>(ex)),
                None: () => Queue(TryOptional<B>(None)),
                Some: xs => xs.Map(x => TryOption<B>(f(x))));

        public static Que<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, Que<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => Queue(Validation<Fail, B>.Fail(ex)),
                Succ: xs => xs.Map(x => Success<Fail, B>(f(x))));

        public static Que<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, Que<A>> ma, Func<A, B> f) 
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ma.Match(
                Fail: ex => Queue(Validation<MonoidFail, Fail, B>.Fail(ex)),
                Succ: xs => xs.Map(x => Success<MonoidFail, Fail, B>(f(x))));

        public static Que<Eff<B>> Traverse<A, B>(this Eff<Que<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => Queue(FailEff<B>(ex)),
                Succ: xs => xs.Map(x => SuccessEff<B>(f(x)))).Run().Value;    
    }
}
