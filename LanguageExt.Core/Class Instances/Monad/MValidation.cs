using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct MValidation<MonoidFail, FAIL, SUCCESS> :
    Choice<Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>,
    BiFoldable<Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>,
    Monad<Validation<MonoidFail, FAIL, SUCCESS>, SUCCESS>
    where MonoidFail : Monoid<FAIL>, Eq<FAIL>
{
    [Pure]
    public static Validation<MonoidFail, FAIL, SUCCESS> Append(Validation<MonoidFail, FAIL, SUCCESS> x, Validation<MonoidFail, FAIL, SUCCESS> y) =>
        x.Match(
            Succ: _ => y.Match(
                Succ: _ => x,
                Fail: _ => y),
            Fail: xf => y.Match(
                Succ: _ => x,
                Fail: yf => Validation<MonoidFail, FAIL, SUCCESS>.Fail(MonoidFail.Append(xf, yf))));

    [Pure]
    public static S BiFold<S>(Validation<MonoidFail, FAIL, SUCCESS> foldable, S state, Func<S, FAIL, S> fa, Func<S, SUCCESS, S> fb) =>
        foldable.Match(
            Fail:    f => fa(state, f),
            Succ:    s => fb(state, s));

    [Pure]
    public static S BiFoldBack<S>(Validation<MonoidFail, FAIL, SUCCESS> foldable, S state, Func<S, FAIL, S> fa, Func<S, SUCCESS, S> fb) =>
        foldable.Match(
            Fail: f => fa(state, f),
            Succ: s => fb(state, s));

    [Pure]
    public static Func<Unit, int> Count(Validation<MonoidFail, FAIL, SUCCESS> fa) => _ =>
        fa.Match(
            Fail: _ => 0,
            Succ: _ => 1);

    [Pure]
    public static Validation<MonoidFail, FAIL, SUCCESS> Empty() =>
        Validation<MonoidFail, FAIL, SUCCESS>.Fail(MonoidFail.Empty());

    [Pure]
    public static Func<Unit, S> Fold<S>(Validation<MonoidFail, FAIL, SUCCESS> fa, S state, Func<S, SUCCESS, S> f) => _ => 
        fa.Match(
            Fail: _ => state,
            Succ: s => f(state, s));

    [Pure]
    public static Func<Unit, S> FoldBack<S>(Validation<MonoidFail, FAIL, SUCCESS> fa, S state, Func<S, SUCCESS, S> f) => _ =>
        fa.Match(
            Fail: _ => state,
            Succ: s => f(state, s));

    [Pure]
    public static bool IsBottom(Validation<MonoidFail, FAIL, SUCCESS> choice) =>
        false;

    [Pure]
    public static bool IsLeft(Validation<MonoidFail, FAIL, SUCCESS> choice) =>
        choice.IsFail;

    [Pure]
    public static bool IsRight(Validation<MonoidFail, FAIL, SUCCESS> choice) =>
        choice.IsSuccess;

    [Pure]
    public static C Match<C>(Validation<MonoidFail, FAIL, SUCCESS> choice, Func<FAIL, C> Left, Func<SUCCESS, C> Right, Func<C>? Bottom = null) =>
        choice.Match(Right, Left);

    [Pure]
    public static Unit Match(Validation<MonoidFail, FAIL, SUCCESS> choice, Action<FAIL> Left, Action<SUCCESS> Right, Action? Bottom = null) =>
        choice.Match(Right, Left);

    [Pure]
    public static C MatchUnsafe<C>(Validation<MonoidFail, FAIL, SUCCESS> choice, Func<FAIL, C> Left, Func<SUCCESS, C> Right, Func<C>? Bottom = null) =>
        choice.MatchUnsafe(Right, Left);

    [Pure]
    public static MB Bind<MONADB, MB, B>(Validation<MonoidFail, FAIL, SUCCESS> ma, Func<SUCCESS, MB> f) where MONADB : Monad<Unit, Unit, MB, B> =>
        ma.Match(
            Succ: s => f(s),
            Fail: e => MONADB.Fail(e));

    [Pure]
    public static Validation<MonoidFail, FAIL, SUCCESS> BindReturn(Unit outputma, Validation<MonoidFail, FAIL, SUCCESS> mb) =>
        mb;

    [Pure]
    public static Validation<MonoidFail, FAIL, SUCCESS> Fail(object? err = null) =>
        err switch
        {
            FAIL fail => Validation<MonoidFail, FAIL, SUCCESS>.Fail(fail),
            _ => Common.Error
                       .Convert<FAIL>(err)
                       .Map(Validation<MonoidFail, FAIL, SUCCESS>.Fail)
                       .IfNone(Validation<MonoidFail, FAIL, SUCCESS>.Fail(MonoidFail.Empty()))
        };            

    [Pure]
    public static Validation<MonoidFail, FAIL, SUCCESS> Run(Func<Unit, Validation<MonoidFail, FAIL, SUCCESS>> ma) =>
        ma(unit);

    [Pure]
    public static Validation<MonoidFail, FAIL, SUCCESS> Plus(Validation<MonoidFail, FAIL, SUCCESS> a, Validation<MonoidFail, FAIL, SUCCESS> b) =>
        a || b;

    [Pure]
    public static Validation<MonoidFail, FAIL, SUCCESS> Return(SUCCESS x) =>
        Validation<MonoidFail, FAIL, SUCCESS>.Success(x);

    [Pure]
    public static Validation<MonoidFail, FAIL, SUCCESS> Return(Func<Unit, SUCCESS> f) =>
        Validation<MonoidFail, FAIL, SUCCESS>.Success(f(unit));

    [Pure]
    public static Validation<MonoidFail, FAIL, SUCCESS> Zero() =>
        Validation<MonoidFail, FAIL, SUCCESS>.Fail(MonoidFail.Empty());

    [Pure]
    public static Validation<MonoidFail, FAIL, SUCCESS> Apply(Func<SUCCESS, SUCCESS, SUCCESS> f, Validation<MonoidFail, FAIL, SUCCESS> fa, Validation<MonoidFail, FAIL, SUCCESS> fb) =>
        (Success<MonoidFail, FAIL, Func<SUCCESS, SUCCESS, SUCCESS>>(f), fa, fb)
       .Apply((ff, a, b) => ff(a, b));
}
