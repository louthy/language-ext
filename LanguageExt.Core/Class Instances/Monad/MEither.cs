using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances;

public struct MEither<L, R> :
    Choice<Either<L, R>, L, R>,
    Monad<Either<L, R>, R>,
    BiFoldable<Either<L, R>, L, R>
{
    [Pure]
    public static MB Bind<MONADB, MB, B>(Either<L, R> ma, Func<R, MB> f) where MONADB : Monad<Unit, Unit, MB, B> =>
        ma.Match(
            Left: l => MONADB.Fail(l),
            Right: f,
            Bottom: () => MONADB.Fail(BottomException.Default));

    [Pure]
    public static Either<L, R> Fail(object? err = null) =>
        Error
           .Convert<L>(err)
           .Map(Either<L, R>.Left)
           .IfNone(Either<L, R>.Bottom);                

    [Pure]
    public static Either<L, R> Plus(Either<L, R> ma, Either<L, R> mb) =>
        ma.Match(
            Left: _ => mb,
            Right: _ => ma,
            Bottom: () => mb);

    [Pure]
    public static Either<L, R> Return(Func<Unit, R> f) =>
        f(unit);

    [Pure]
    public static Either<L, R> Zero() =>
        default;

    [Pure]
    public static Func<Unit, S> Fold<S>(Either<L, R> foldable, S state, Func<S, R, S> f) =>
        _ => Check.NullReturn(
            foldable.Match(
                Left:  _ => state,
                Right: _ => f(state, foldable.RightValue),
                Bottom: () => state));

    [Pure]
    public static Func<Unit, S> FoldBack<S>(Either<L, R> foldable, S state, Func<S, R, S> f) =>
        _ => Check.NullReturn(
            foldable.Match(
                Left:  _ => state,
                Right: _ => f(state, foldable.RightValue),
                Bottom: () => state));

    [Pure]
    public static S BiFold<S>(Either<L, R> foldable, S state, Func<S, L, S> fa, Func<S, R, S> fb) =>
        Check.NullReturn(
            foldable.Match(
                Left: _ => fa(state, foldable.LeftValue),
                Right: _ => fb(state, foldable.RightValue),
                Bottom: () => state));

    [Pure]
    public static S BiFoldBack<S>(Either<L, R> foldable, S state, Func<S, L, S> fa, Func<S, R, S> fb) =>
        Check.NullReturn(
            foldable.Match(
                Left: _ => fa(state, foldable.LeftValue),
                Right: _ => fb(state, foldable.RightValue),
                Bottom: () => state));

    [Pure]
    public static Func<Unit, int> Count(Either<L, R> ma) =>
        _ => ma.Match(
            Left: _ => 0,
            Right: _ => 1,
            Bottom: () => 0);

    [Pure]
    public static Either<L, R> None =>
        Either<L, R>.Bottom;

    [Pure]
    public static Either<L, R> Some(R value) =>
        value;

    [Pure]
    public static Either<L, R> Optional(R value) =>
        value;

    [Pure]
    public static Either<L, R> Run(Func<Unit, Either<L, R>> ma) =>
        ma(unit);

    [Pure]
    public static Either<L, R> BindReturn(Unit _, Either<L, R> mb) =>
        mb;

    [Pure]
    public static Either<L, R> Return(R x) =>
        Return(_ => x);

    [Pure]
    public static Either<L, R> Empty() =>
        Either<L, R>.Bottom;

    [Pure]
    public static Either<L, R> Append(Either<L, R> x, Either<L, R> y) =>
        Plus(x, y);

    [Pure]
    public static bool IsLeft(Either<L, R> choice) =>
        choice.State == EitherStatus.IsLeft;

    [Pure]
    public static bool IsRight(Either<L, R> choice) =>
        choice.State == EitherStatus.IsRight;

    [Pure]
    public static bool IsBottom(Either<L, R> choice) =>
        choice.State == EitherStatus.IsBottom;


    [Pure]
    public static C Match<C>(Either<L, R> choice, Func<L, C> Left, Func<R, C> Right, Func<C>? Bottom = null) =>
        choice.State == EitherStatus.IsBottom
            ? Bottom == null
                  ? throw new BottomException()
                  : Check.NullReturn(Bottom())
            : choice.State == EitherStatus.IsLeft
                ? Check.NullReturn(Left(choice.left))
                : Check.NullReturn(Right(choice.right));

    [Pure]
    public static Unit Match(Either<L, R> choice, Action<L> Left, Action<R> Right, Action? Bottom = null)
    {
        if (choice.State == EitherStatus.IsRight) Right(choice.right);
        if (choice.State == EitherStatus.IsLeft) Left(choice.left);
        if (Bottom       == null) throw new BottomException();
        Bottom();
        return unit;
    }

    [Pure]
    public static C MatchUnsafe<C>(Either<L, R> choice, Func<L, C> Left, Func<R, C> Right, Func<C>? Bottom = null) =>
        choice.State == EitherStatus.IsBottom
            ? Bottom == null
                  ? throw new BottomException()
                  : Bottom()
            : choice.State == EitherStatus.IsLeft
                ? Left(choice.left)
                : Right(choice.right);

    [Pure]
    public static Either<L, R> Apply(Func<R, R, R> f, Either<L, R> fa, Either<L, R> fb) =>
        from a in fa
        from b in fb
        select f(a, b);
}
