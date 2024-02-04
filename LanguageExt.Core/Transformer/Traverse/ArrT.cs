#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ArrT
{
    public static Arr<Arr<B>> Traverse<A, B>(this Arr<Arr<A>> xxs, Func<A, B> f) =>
        CollT.AllCombinationsOf(xxs.Map(xs => xs.ToList()).ToArray(), f)
             .Map(toArray)
             .ToArr();
        
    public static Arr<Either<L, B>> Traverse<L, A, B>(this Either<L, Arr<A>> ma, Func<A, B> f) =>
        ma.Match(
            Left:  e  => Array(Left<L, B>(e)),
            Right: xs => xs.Map(x => Right<L, B>(f(x))));

    public static Arr<HashSet<B>> Traverse<A, B>(this HashSet<Arr<A>> ma, Func<A, B> f) =>
        CollT.AllCombinationsOf(ma.ToArray().Map(xs => xs.ToList()).ToArray(), f)
             .Map(toHashSet)
             .ToArr();

    public static Arr<Identity<B>> Traverse<A, B>(this Identity<Arr<A>> ma, Func<A, B> f) =>
        ma.Value.Map(a => Id(f(a)));

    public static Arr<Lst<B>> Traverse<A, B>(this Lst<Arr<A>> xxs, Func<A, B> f) =>
        CollT.AllCombinationsOf(xxs.Map(xs => xs.ToList()).ToArray(), f)
             .Map(toList)
             .ToArr();

    public static Arr<Fin<B>> Traverse<A, B>(this Fin<Arr<A>> ma, Func<A, B> f) =>
        ma.Match(
            Fail: er => Array(Fin<B>.Fail(er)),
            Succ: xs => xs.Map(x => FinSucc(f(x))));

    public static Arr<Option<B>> Traverse<A, B>(this Option<Arr<A>> ma, Func<A, B> f) =>
        ma.Match(
            None: () => Array(Option<B>.None),
            Some: xs => xs.Map(x => Some(f(x))));

    public static Arr<Que<B>> Traverse<A, B>(this Que<Arr<A>> xxs, Func<A, B> f) =>
        CollT.AllCombinationsOf(xxs.Map(xs => xs.ToList()).ToArray(), f)
             .Map(toQueue)
             .ToArr();

    public static Arr<Seq<B>> Traverse<A, B>(this Seq<Arr<A>> xxs, Func<A, B> f) =>
        CollT.AllCombinationsOf(xxs.ToArray().Map(xs => xs.ToList()).ToArray(), f)
             .Map(toSeq)
             .ToArr();

    public static Arr<IEnumerable<B>> Traverse<A, B>(this IEnumerable<Arr<A>> xxs, Func<A, B> f) =>
        CollT.AllCombinationsOf(xxs.Map(xs => xs.ToList()).ToArray(), f)
             .Map(xs => xs.AsEnumerable())
             .ToArr();

    public static Arr<Set<B>> Traverse<A, B>(this Set<Arr<A>> xxs, Func<A, B> f) =>
        CollT.AllCombinationsOf(xxs.Map(xs => xs.ToList()).ToArray(), f)
             .Map(toSet)
             .ToArr();

    public static Arr<Stck<B>> Traverse<A, B>(this Stck<Arr<A>> xxs, Func<A, B> f) =>
        CollT.AllCombinationsOf(xxs.Reverse().Map(xs => xs.ToList()).ToArray(), f)
             .Map(toStack)
             .ToArr();

    public static Arr<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, Arr<A>> ma, Func<A, B> f) =>
        ma.Match(
            Fail: e => Array(Validation<Fail, B>.Fail(e)),
            Succ: xs => xs.Map(x => Success<Fail, B>(f(x))));

    public static Arr<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, Arr<A>> ma, Func<A, B> f) 
        where MonoidFail : Monoid<Fail>, Eq<Fail> =>
        ma.Match(
            Fail: es => Array(Validation<MonoidFail, Fail, B>.Fail(es)),
            Succ: xs => xs.Map(x => Success<MonoidFail, Fail, B>(f(x))));

    public static Arr<Eff<B>> Traverse<A, B>(this Eff<Arr<A>> ma, Func<A, B> f) =>
        ma.Match(
            Fail: ex => Array(FailEff<B>(ex)),
            Succ: xs => xs.Map(x => SuccessEff(f(x)))).Run().Value;    
}
