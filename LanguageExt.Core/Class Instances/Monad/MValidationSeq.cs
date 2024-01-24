using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct MValidation<FAIL, SUCCESS> :
    Choice<Validation<FAIL, SUCCESS>, Seq<FAIL>, SUCCESS>,
    BiFoldable<Validation<FAIL, SUCCESS>, FAIL, SUCCESS>,
    Monad<Validation<FAIL, SUCCESS>, SUCCESS>
{
    [Pure]
    public static Validation<FAIL, SUCCESS> Append(Validation<FAIL, SUCCESS> x, Validation<FAIL, SUCCESS> y) =>
        x.Match(
            Succ: _ => y.Match(
                Succ: _ => x,
                Fail: _ => y),
            Fail: xf => y.Match(
                Succ: _ => x,
                Fail: yf => Validation<FAIL, SUCCESS>.Fail(MSeq<FAIL>.Append(xf, yf))));

    [Pure]
    public static S BiFold<S>(Validation<FAIL, SUCCESS> foldable, S state, Func<S, FAIL, S> fa, Func<S, SUCCESS, S> fb) =>
        foldable.Match(
            Fail:    f => f.Fold(state, fa),
            Succ:    s => fb(state, s));

    [Pure]
    public static S BiFoldBack<S>(Validation<FAIL, SUCCESS> foldable, S state, Func<S, FAIL, S> fa, Func<S, SUCCESS, S> fb) =>
        foldable.Match(
            Fail: f => f.FoldBack(state, fa),
            Succ: s => fb(state, s));

    [Pure]
    public static Func<Unit, int> Count(Validation<FAIL, SUCCESS> fa) => _ =>
        fa.Match(
            Fail: _ => 0,
            Succ: _ => 1);

    [Pure]
    public static Validation<FAIL, SUCCESS> Empty() =>
        Validation<FAIL, SUCCESS>.Fail(Seq<FAIL>.Empty);

    [Pure]
    public static Func<Unit, S> Fold<S>(Validation<FAIL, SUCCESS> fa, S state, Func<S, SUCCESS, S> f) => _ => 
        fa.Match(
            Fail: _ => state,
            Succ: s => f(state, s));

    [Pure]
    public static Func<Unit, S> FoldBack<S>(Validation<FAIL, SUCCESS> fa, S state, Func<S, SUCCESS, S> f) => _ =>
        fa.Match(
            Fail: _ => state,
            Succ: s => f(state, s));

    [Pure]
    public static bool IsBottom(Validation<FAIL, SUCCESS> choice) =>
        false;

    [Pure]
    public static bool IsLeft(Validation<FAIL, SUCCESS> choice) =>
        choice.IsFail;

    [Pure]
    public static bool IsRight(Validation<FAIL, SUCCESS> choice) =>
        choice.IsSuccess;

    [Pure]
    public static C Match<C>(Validation<FAIL, SUCCESS> choice, Func<Seq<FAIL>, C> Left, Func<SUCCESS, C> Right, Func<C>? Bottom = null) =>
        choice.Match(Right, Left);

    [Pure]
    public static Unit Match(Validation<FAIL, SUCCESS> choice, Action<Seq<FAIL>> Left, Action<SUCCESS> Right, Action? Bottom = null) =>
        choice.Match(Right, Left);

    [Pure]
    public static C MatchUnsafe<C>(Validation<FAIL, SUCCESS> choice, Func<Seq<FAIL>, C> Left, Func<SUCCESS, C> Right, Func<C>? Bottom = null) =>
        choice.MatchUnsafe(Right, Left);

    [Pure]
    public static MB Bind<MONADB, MB, B>(Validation<FAIL, SUCCESS> ma, Func<SUCCESS, MB> f) where MONADB : Monad<Unit, Unit, MB, B> =>
        ma.Match(
            Succ: s => f(s),
            Fail: e => MONADB.Fail(e));

    [Pure]
    public static Validation<FAIL, SUCCESS> BindReturn(Unit outputma, Validation<FAIL, SUCCESS> mb) =>
        mb;

    [Pure]
    public static Validation<FAIL, SUCCESS> Fail(Seq<FAIL> err) =>
        Validation<FAIL, SUCCESS>.Fail(err);

    [Pure]
    public static Validation<FAIL, SUCCESS> Fail(object? err = null) =>
        err switch
        {
            // Messy, but we're doing our best to recover an error rather than return Bottom
                
            FAIL fail       => Validation<FAIL, SUCCESS>.Fail(Seq1(fail)),
            Seq<FAIL> fails => Validation<FAIL, SUCCESS>.Fail(fails),
            _ => Common.Error
                       .Convert<FAIL>(err)
                       .Map(f => Validation<FAIL, SUCCESS>.Fail(Seq1(f)))
                       .IfNone(Validation<FAIL, SUCCESS>.Fail(Seq<FAIL>.Empty))
        };            

    [Pure]
    public static Validation<FAIL, SUCCESS> Run(Func<Unit, Validation<FAIL, SUCCESS>> ma) =>
        ma(unit);

    [Pure]
    public static Validation<FAIL, SUCCESS> Plus(Validation<FAIL, SUCCESS> a, Validation<FAIL, SUCCESS> b) =>
        a || b;

    [Pure]
    public static Validation<FAIL, SUCCESS> Return(SUCCESS x) =>
        Validation<FAIL, SUCCESS>.Success(x);

    [Pure]
    public static Validation<FAIL, SUCCESS> Return(Func<Unit, SUCCESS> f) =>
        Validation<FAIL, SUCCESS>.Success(f(unit));

    [Pure]
    public static Validation<FAIL, SUCCESS> Zero() =>
        Validation<FAIL, SUCCESS>.Fail(Seq<FAIL>.Empty);

    [Pure]
    public static Validation<FAIL, SUCCESS> Apply(Func<SUCCESS, SUCCESS, SUCCESS> f, Validation<FAIL, SUCCESS> fa, Validation<FAIL, SUCCESS> fb) =>
        (Success<FAIL, Func<SUCCESS, SUCCESS, SUCCESS>>(f), fa, fb)
       .Apply((ff, a, b) => ff(a, b));
}
