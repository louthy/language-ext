using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances;

public struct MOption<A> :
    Optional<Option<A>, A>,
    Monad<Option<A>, A>,
    BiFoldable<Option<A>, A, Unit>,
    Ord<Option<A>>,
    Monoid<Option<A>>
{
    [Pure]
    public static Option<A> None => 
        default;
 
    [Pure]
    public static MB Bind<MonadB, MB, B>(Option<A> ma, Func<A, MB> f) where MonadB : Monad<Unit, Unit, MB, B> =>
        ma is { IsSome: true, Value: not null } 
            ? f(ma.Value)
            : MonadB.Fail(ValueIsNoneException.Default);

    [Pure]
    public static Option<A> Fail(object? err = null) =>
        Option<A>.None;

    [Pure]
    public static Option<A> Plus(Option<A> a, Option<A> b) =>
        a.IsSome ? a : b;

    [Pure]
    public static Option<A> Return(Func<Unit, A> f) =>
        Option<A>.Some(f(unit));

    [Pure]
    public static Option<A> Zero() =>
        Option<A>.None;

    [Pure]
    public static bool IsNone(Option<A> opt) =>
        opt.IsNone;

    [Pure]
    public static bool IsSome(Option<A> opt) =>
        opt.IsSome;

    [Pure]
    public static B Match<B>(Option<A> opt, Func<A, B> Some, Func<B> None) =>
        opt.Match(Some, None);

    [Pure]
    public static B Match<B>(Option<A> opt, Func<A, B> Some, B None) =>
        opt.Match(Some, None);

    public static Unit Match(Option<A> opt, Action<A> Some, Action None) =>
        opt.Match(Some, None);

    [Pure]
    public static Func<Unit, S> Fold<S>(Option<A> ma, S state, Func<S, A, S> f) => _ =>
        ma.Fold(state, f);

    [Pure]
    public static Func<Unit, S> FoldBack<S>(Option<A> ma, S state, Func<S, A, S> f) => _ =>
        ma.FoldBack(state, f);

    [Pure]
    public static S BiFold<S>(Option<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
        ma.BiFold(state, fa, fb);

    [Pure]
    public static S BiFoldBack<S>(Option<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
        ma.BiFold(state, fa, fb);

    [Pure]
    public static Func<Unit, int> Count(Option<A> ma) => _ =>
        ma.IsSome ? 1 : 0;

    [Pure]
    public static Option<A> Some(A x) =>
        Option<A>.Some(x);

    [Pure]
    public static Option<A> MkOptional(A x) =>
        isnull(x)
            ? default
            : Option<A>.Some(x);

    [Pure]
    public static Option<A> Run(Func<Unit, Option<A>> ma) =>
        new (ma(unit));

    [Pure]
    public static Option<A> BindReturn(Unit _, Option<A> mb) =>
        mb;

    [Pure]
    public static Option<A> Return(A x) =>
        Some(x);

    [Pure]
    public static Option<A> Empty() =>
        default;

    [Pure]
    public static Option<A> Append(Option<A> x, Option<A> y) =>
        Plus(x, y);

    [Pure]
    public static bool Equals(Option<A> x, Option<A> y) =>
        x.Equals(y);

    [Pure]
    public static int GetHashCode(Option<A> x) =>
        x.GetHashCode();

    [Pure]
    public static Option<A> Apply(Func<A, A, A> f, Option<A> fa, Option<A> fb) =>
        fa is { IsSome: true, Value: not null } && fb is { IsSome: true, Value: not null }
            ? Some(f(fa.Value, fb.Value))
            : default;

    [Pure]
    public static int Compare(Option<A> x, Option<A> y) =>
        compare<OrdDefault<A>, A>(x, y);
}
