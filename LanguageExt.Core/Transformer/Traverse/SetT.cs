#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class SetT
{
    public static Set<Arr<B>> Traverse<A, B>(this Arr<Set<A>> ma, Func<A, B> f) =>
        toSet(CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                   .Map(toArray));
        
    public static Set<Either<L, B>> Traverse<L, A, B>(this Either<L, Set<A>> ma, Func<A, B> f) =>
        ma.Match(
            Left: e => Set(Either<L, B>.Left(e)),
            Right: xs => xs.Map(x => Right<L, B>(f(x))));

    public static Set<HashSet<B>> Traverse<A, B>(this HashSet<Set<A>> ma, Func<A, B> f) =>
        toSet(CollT.AllCombinationsOf(ma.ToArray().Map(xs => xs.ToList()).ToArray(), f)
                   .Map(toHashSet));

    public static Set<Identity<B>> Traverse<A, B>(this Identity<Set<A>> ma, Func<A, B> f) =>
        ma.Value.Map(a => new Identity<B>(f(a)));

    public static Set<Lst<B>> Traverse<A, B>(this Lst<Set<A>> ma, Func<A, B> f) =>
        toSet(CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                   .Map(toList));

    public static Set<Fin<B>> Traverse<A, B>(this Fin<Set<A>> ma, Func<A, B> f) =>
        ma.Match(
            Fail: er => Set(FinFail<B>(er)),
            Succ: xs => xs.Map(x => FinSucc(f(x))));

    public static Set<Option<B>> Traverse<A, B>(this Option<Set<A>> ma, Func<A, B> f) =>
        ma.Match(
            None: () => Set(Option<B>.None),
            Some: xs => xs.Map(x => Some(f(x))));

    public static Set<Que<B>> Traverse<A, B>(this Que<Set<A>> ma, Func<A, B> f) =>
        toSet(CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                   .Map(toQueue));

    public static Set<Seq<B>> Traverse<A, B>(this Seq<Set<A>> ma, Func<A, B> f) =>
        toSet(CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                   .Map(toSeq));

    public static Set<IEnumerable<B>> Traverse<A, B>(this IEnumerable<Set<A>> ma, Func<A, B> f) =>
        toSet(CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
                   .Map(xs => xs.AsEnumerable()));

    public static Set<Set<B>> Traverse<A, B>(this Set<Set<A>> ma, Func<A, B> f) =>
        toSet(CollT.AllCombinationsOf(ma.ToArray().Map(xs => xs.ToList()).ToArray(), f)
                   .Map(toSet));

    public static Set<Stck<B>> Traverse<A, B>(this Stck<Set<A>> ma, Func<A, B> f) =>
        toSet(CollT.AllCombinationsOf(ma.Reverse().Map(xs => xs.ToList()).ToArray(), f)
                   .Map(toStack));

    public static Set<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, Set<A>> ma, Func<A, B> f) =>
        ma.Match(
            Fail: ex => Set(Validation<Fail, B>.Fail(ex)),
            Succ: xs => xs.Map(x => Success<Fail, B>(f(x))));

    public static Set<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, Set<A>> ma, Func<A, B> f) 
        where MonoidFail : Monoid<Fail>, Eq<Fail> =>
        ma.Match(
            Fail: ex => Set(Validation<MonoidFail, Fail, B>.Fail(ex)),
            Succ: xs => xs.Map(x => Success<MonoidFail, Fail, B>(f(x))));

    public static Set<Eff<B>> Traverse<A, B>(this Eff<Set<A>> ma, Func<A, B> f) =>
        ma.Match(
            Fail: ex => Set(FailEff<B>(ex)),
            Succ: xs => xs.Map(x => SuccessEff(f(x)))).Run().Value;    
}
