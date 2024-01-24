using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct MOptionUnsafe<A> :
    Monad<OptionUnsafe<A>, A?>,
    OptionalUnsafe<OptionUnsafe<A>, A>,
    BiFoldable<OptionUnsafe<A>, A?, Unit>,
    Eq<OptionUnsafe<A>>
{
    [Pure]
    public static OptionUnsafe<A> None => OptionUnsafe<A>.None;

    [Pure]
    public static MB Bind<MONADB, MB, B>(OptionUnsafe<A> ma, Func<A, MB> f) where MONADB : Monad<Unit, Unit, MB, B> =>
        ma.IsSome
            ? (f ?? throw new ArgumentNullException(nameof(f)))(ma.Value)
            : MONADB.Fail(ValueIsNoneException.Default);

    [Pure]
    public static OptionUnsafe<A> Fail(object? err = null) =>
        OptionUnsafe<A>.None;

    [Pure]
    public static OptionUnsafe<A> Plus(OptionUnsafe<A> a, OptionUnsafe<A> b) =>
        a.IsSome
            ? a
            : b;

    [Pure]
    public static OptionUnsafe<A> Return(Func<Unit, A?> f) =>
        OptionUnsafe<A>.Some(f(unit));

    [Pure]
    public static OptionUnsafe<A> Zero() =>
        default;

    [Pure]
    public static bool IsNone(OptionUnsafe<A> opt) =>
        opt.IsNone;

    [Pure]
    public static bool IsSome(OptionUnsafe<A> opt) =>
        opt.IsSome;

    [Pure]
    public static B? MatchUnsafe<B>(OptionUnsafe<A> opt, Func<A?, B?> Some, Func<B?> None) =>
        opt.MatchUnsafe(Some, None);

    [Pure]
    public static B? MatchUnsafe<B>(OptionUnsafe<A> opt, Func<A?, B?> Some, B? None) =>
        opt.MatchUnsafe(Some, None);

    [Pure]
    public static Unit Match(OptionUnsafe<A> opt, Action<A> Some, Action None) =>
        opt.MatchUnsafe(Some, None);

    [Pure]
    public static Func<Unit, S> Fold<S>(OptionUnsafe<A> ma, S state, Func<S, A?, S> f) => _ =>
        ma.Fold(state, f);

    [Pure]
    public static Func<Unit, S> FoldBack<S>(OptionUnsafe<A> ma, S state, Func<S, A?, S> f) => _ =>
        ma.FoldBack(state, f);

    [Pure]
    public static S BiFold<S>(OptionUnsafe<A> ma, S state, Func<S, A?, S> fa, Func<S, Unit, S> fb) =>
        ma.BiFold(state, fa, fb);

    [Pure]
    public static S BiFoldBack<S>(OptionUnsafe<A> ma, S state, Func<S, A?, S> fa, Func<S, Unit, S> fb) =>
        ma.BiFold(state, fa, fb);

    [Pure]
    public static Func<Unit, int> Count(OptionUnsafe<A> ma) => _ =>
        ma.IsSome
            ? 1
            : 0;

    [Pure]
    public static OptionUnsafe<A> Some(A? x) =>
        OptionUnsafe<A>.Some(x);

    [Pure]
    public static OptionUnsafe<A> Run(Func<Unit, OptionUnsafe<A>> ma) =>
        ma(unit);

    [Pure]
    public static OptionUnsafe<A> BindReturn(Unit _, OptionUnsafe<A> mb) =>
        mb;

    [Pure]
    public static OptionUnsafe<A> Return(A? x) =>
        OptionUnsafe<A>.Some(x);

    [Pure]
    public static OptionUnsafe<A> Empty() =>
        default;

    [Pure]
    public static OptionUnsafe<A> Append(OptionUnsafe<A> x, OptionUnsafe<A> y) =>
        Plus(x, y);

    [Pure]
    public static bool Equals(OptionUnsafe<A> x, OptionUnsafe<A> y) =>
        x.Equals(y);

    [Pure]
    public static int GetHashCode(OptionUnsafe<A> x) =>
        x.GetHashCode();

    [Pure]
    public static OptionUnsafe<A> Apply(Func<A?, A?, A?> f, OptionUnsafe<A> fa, OptionUnsafe<A> fb) =>
        fa.IsSome && fb.IsSome
            ? Some(f(fa.Value, fb.Value))
            : default;
}
