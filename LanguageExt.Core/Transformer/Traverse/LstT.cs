#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class LstT
{
    public static Lst<Arr<B>> Traverse<A, B>(this Arr<Lst<A>> ma, Func<A, B> f) =>
        CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
             .Map(toArray)
             .Freeze();
        
    public static Lst<Either<L, B>> Traverse<L, A, B>(this Either<L, Lst<A>> ma, Func<A, B> f) =>
        ma.Match(
            Left: e => List(Either<L, B>.Left(e)),
            Right: xs => xs.Map(x => Right<L, B>(f(x))));

    public static Lst<HashSet<B>> Traverse<A, B>(this HashSet<Lst<A>> ma, Func<A, B> f) =>
        CollT.AllCombinationsOf(ma.ToArray().Map(xs => xs.ToList()).ToArray(), f)
             .Map(toHashSet)
             .Freeze();

    public static Lst<Identity<B>> Traverse<A, B>(this Identity<Lst<A>> ma, Func<A, B> f) =>
        ma.Value.Map(a => new Identity<B>(f(a)));

    public static Lst<Lst<B>> Traverse<A, B>(this Lst<Lst<A>> ma, Func<A, B> f) =>
        CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
             .Map(toList)
             .Freeze();

    public static Lst<Fin<B>> Traverse<A, B>(this Fin<Lst<A>> ma, Func<A, B> f) =>
        ma.Match(
            Fail: er => List(Fin<B>.Fail(er)),
            Succ: xs => xs.Map(x => FinSucc(f(x))));

    public static Lst<Option<B>> Traverse<A, B>(this Option<Lst<A>> ma, Func<A, B> f) =>
        ma.Match(
            None: () => List(Option<B>.None),
            Some: xs => xs.Map(x => Some(f(x))));

    public static Lst<Que<B>> Traverse<A, B>(this Que<Lst<A>> ma, Func<A, B> f) =>
        CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
             .Map(toQueue)
             .Freeze();

    public static Lst<Seq<B>> Traverse<A, B>(this Seq<Lst<A>> ma, Func<A, B> f) =>
        CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
             .Map(toSeq)
             .Freeze();

    public static Lst<IEnumerable<B>> Traverse<A, B>(this IEnumerable<Lst<A>> ma, Func<A, B> f) =>
        CollT.AllCombinationsOf(ma.Map(xs => xs.ToList()).ToArray(), f)
             .Map(xs => xs.AsEnumerable())
             .Freeze();

    public static Lst<Set<B>> Traverse<A, B>(this Set<Lst<A>> ma, Func<A, B> f) =>
        CollT.AllCombinationsOf(ma.ToArray().Map(xs => xs.ToList()).ToArray(), f)
             .Map(toSet)
             .Freeze();

    public static Lst<Stck<B>> Traverse<A, B>(this Stck<Lst<A>> ma, Func<A, B> f) =>
        CollT.AllCombinationsOf(ma.Reverse().Map(xs => xs.ToList()).ToArray(), f)
             .Map(toStack)
             .Freeze();

    public static Lst<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, Lst<A>> ma, Func<A, B> f) =>
        ma.Match(
            Fail: ex => List(Validation<Fail, B>.Fail(ex)),
            Succ: xs => xs.Map(x => Success<Fail, B>(f(x))));

    public static Lst<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, Lst<A>> ma, Func<A, B> f) 
        where MonoidFail : Monoid<Fail>, Eq<Fail> =>
        ma.Match(
            Fail: ex => List(Validation<MonoidFail, Fail, B>.Fail(ex)),
            Succ: xs => xs.Map(x => Success<MonoidFail, Fail, B>(f(x))));

    public static Lst<Eff<B>> Traverse<A, B>(this Eff<Lst<A>> ma, Func<A, B> f) =>
        ma.Match(
            Fail: ex => List(FailEff<B>(ex)),
            Succ: xs => xs.Map(x => SuccessEff(f(x)))).Run().Value;    
}
