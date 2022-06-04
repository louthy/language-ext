#nullable enable
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
        public static Seq<Arr<B>> Traverse<A, B>(this Arr<Seq<A>> ma, Func<A, B> f) =>
            CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                .Map(toArray)
                .ToSeq();
        
        public static Seq<Either<L, B>> Traverse<L, A, B>(this Either<L, Seq<A>> ma, Func<A, B> f) =>
            ma.Match(
                Left:   e => Seq1(Either<L, B>.Left(e)),
                Right: xs => xs.Map(x => Right<L, B>(f(x))));

        public static Seq<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, Seq<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                Left:   e => Seq1(EitherUnsafe<L, B>.Left(e)),
                Right: xs => xs.Map(x => RightUnsafe<L, B>(f(x))));

        public static Seq<HashSet<B>> Traverse<A, B>(this HashSet<Seq<A>> ma, Func<A, B> f) =>
            CollT.AllCombinationsOf(ma.ToArray().Map(xs => xs.ToList()).ToArray(), f)
                .Map(toHashSet)
                .ToSeq();

        public static Seq<Identity<B>> Traverse<A, B>(this Identity<Seq<A>> ma, Func<A, B> f) =>
            ma.Value.Map(a => new Identity<B>(f(a)));

        public static Seq<Lst<B>> Traverse<A, B>(this Lst<Seq<A>> ma, Func<A, B> f) =>
            CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToSystemArray(), f)
                .Map(toList)
                .ToSeq();

        public static Seq<Fin<B>> Traverse<A, B>(this Fin<Seq<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: er => Seq1(FinFail<B>(er)),
                Succ: xs => xs.Map(x => FinSucc(f(x))));

        public static Seq<Option<B>> Traverse<A, B>(this Option<Seq<A>> ma, Func<A, B> f) =>
            ma.Match(
                None: () => Seq1(Option<B>.None),
                Some: xs => xs.Map(x => Some(f(x))));

        public static Seq<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<Seq<A>> ma, Func<A, B> f) =>
            ma.MatchUnsafe(
                None: () => Seq1(OptionUnsafe<B>.None),
                Some: xs => xs.Map(x => SomeUnsafe(f(x))));

        public static Seq<Que<B>> Traverse<A, B>(this Que<Seq<A>> ma, Func<A, B> f) =>
            CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                .Map(toQueue)
                .ToSeq();

        public static Seq<Seq<B>> Traverse<A, B>(this Seq<Seq<A>> ma, Func<A, B> f) =>
            CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                .Map(toSeq)
                .ToSeq();

        public static Seq<IEnumerable<B>> Traverse<A, B>(this IEnumerable<Seq<A>> ma, Func<A, B> f) =>
            CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                .Map(xs => xs.AsEnumerable())
                .ToSeq();

        public static Seq<Set<B>> Traverse<A, B>(this Set<Seq<A>> ma, Func<A, B> f) =>
            CollT.AllCombinationsOf(ma.ToArray().Map(xs => xs.ToList()).ToArray(), f)
                .Map(toSet)
                .ToSeq();

        public static Seq<Stck<B>> Traverse<A, B>(this Stck<Seq<A>> ma, Func<A, B> f) =>
            CollT.AllCombinationsOf(ma.Reverse().Map(xs => xs.ToList()).ToArray(), f)
                .Map(toStack)
                .ToSeq();

        public static Seq<Try<B>> Traverse<A, B>(this Try<Seq<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => Seq1(TryFail<B>(ex)),
                Succ: xs => xs.Map(x => Try<B>(f(x))));

        public static Seq<TryOption<B>> Traverse<A, B>(this TryOption<Seq<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => Seq1(TryOptionFail<B>(ex)),
                None: () => Seq1(TryOptional<B>(None)),
                Some: xs => xs.Map(x => TryOption<B>(f(x))));

        public static Seq<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, Seq<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => Seq1(Validation<Fail, B>.Fail(ex)),
                Succ: xs => xs.Map(x => Success<Fail, B>(f(x))));

        public static Seq<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, Seq<A>> ma, Func<A, B> f) 
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ma.Match(
                Fail: ex => Seq1(Validation<MonoidFail, Fail, B>.Fail(ex)),
                Succ: xs => xs.Map(x => Success<MonoidFail, Fail, B>(f(x))));

        public static Seq<Eff<B>> Traverse<A, B>(this Eff<Seq<A>> ma, Func<A, B> f) =>
            ma.Match(
                Fail: ex => Seq1(FailEff<B>(ex)),
                Succ: xs => xs.Map(x => SuccessEff<B>(f(x)))).Run().Value;    
    }
}
