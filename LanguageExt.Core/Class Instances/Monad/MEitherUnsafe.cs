using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances;

public struct MEitherUnsafe<L, R> :
    ChoiceUnsafe<EitherUnsafe<L, R>, L, R>,
    Monad<EitherUnsafe<L, R>, R?>,
    BiFoldable<EitherUnsafe<L, R>, L?, R?>
{
    [Pure]
    public static MB Bind<MONADB, MB, B>(EitherUnsafe<L, R> ma, Func<R?, MB> f) where MONADB : Monad<Unit, Unit, MB, B> =>
        MatchUnsafe(ma,
                    Left: l => MONADB.Fail(l),
                    Right: r => f(r),
                    Bottom: () => MONADB.Fail(BottomException.Default)) ?? throw new ValueIsNullException();

    [Pure]
    public static EitherUnsafe<L, R> Fail(object? err = null) =>
        Error
              .Convert<L>(err)
              .Map(EitherUnsafe<L, R>.Left)
              .IfNone(EitherUnsafe<L, R>.Bottom);                
        
    [Pure]
    public static EitherUnsafe<L, R> Plus(EitherUnsafe<L, R> ma, EitherUnsafe<L, R> mb) =>
        MatchUnsafe(ma,
                    Left: _ => mb,
                    Right: _ => ma,
                    Bottom: () => mb);

    [Pure]
    public static EitherUnsafe<L, R> Return(Func<Unit, R?> f) =>
        f(unit);

    [Pure]
    public static EitherUnsafe<L, R> Zero() =>
        default;

    [Pure]
    public static bool IsNone(EitherUnsafe<L, R> opt) =>
        !opt.IsRight;

    [Pure]
    public static bool IsSome(EitherUnsafe<L, R> opt) =>
        opt.IsRight;

    [Pure]
    public static Unit Match(EitherUnsafe<L, R> choice, Action<R?> Right, Action Left)
    {
        if (choice.State == EitherStatus.IsRight) Right(choice.right);
        if (choice.State == EitherStatus.IsLeft) Left();
        return unit;
    }

    [Pure]
    public static Func<Unit, S> Fold<S>(EitherUnsafe<L, R> foldable, S state, Func<S, R?, S> f) =>
        _ =>
            foldable.MatchUnsafe(
                Left: _ => state,
                Right: _ => f(state, foldable.RightValue),
                Bottom: () => state) ?? throw new ValueIsNullException();

    [Pure]
    public static Func<Unit, S> FoldBack<S>(EitherUnsafe<L, R> foldable, S state, Func<S, R?, S> f) =>
        _ =>
            foldable.MatchUnsafe(
                Left: _ => state,
                Right: _ => f(state, foldable.RightValue),
                Bottom: () => state) ?? throw new ValueIsNullException();

    [Pure]
    public static S BiFold<S>(EitherUnsafe<L, R> foldable, S state, Func<S, L?, S> fa, Func<S, R?, S> fb) =>
        MatchUnsafe(foldable,
                    Left: _ => fa(state, foldable.LeftValue),
                    Right: _ => fb(state, foldable.RightValue),
                    Bottom: () => state) ?? throw new ValueIsNullException();

    [Pure]
    public static S BiFoldBack<S>(EitherUnsafe<L, R> foldable, S state, Func<S, L?, S> fa, Func<S, R?, S> fb) =>
        MatchUnsafe(foldable,
                    Left: _ => fa(state, foldable.LeftValue),
                    Right: _ => fb(state, foldable.RightValue),
                    Bottom: () => state) ?? throw new ValueIsNullException();

    [Pure] 
    public static Func<Unit, int> Count(EitherUnsafe<L, R> ma) =>
        _ => MatchUnsafe(ma, Left: _ => 0, Right: _ => 1, Bottom: () => 0);

    [Pure]
    public static EitherUnsafe<L, R> Some(R value) =>
        value;

    [Pure]
    public static EitherUnsafe<L, R> Optional(R value) =>
        value;

    [Pure]
    public static EitherUnsafe<L, R> None =>
        EitherUnsafe<L, R>.Bottom;

    [Pure]
    public static EitherUnsafe<L, R> Run(Func<Unit, EitherUnsafe<L, R>> ma) =>
        ma(unit);

    [Pure]
    public static EitherUnsafe<L, R> BindReturn(Unit _, EitherUnsafe<L, R> mb) =>
        mb;

    [Pure]
    public static EitherUnsafe<L, R> Return(R? x) =>
        Return(_ => x);

    [Pure]
    public static EitherUnsafe<L, R> Empty() =>
        EitherUnsafe<L, R>.Bottom;

    [Pure]
    public static EitherUnsafe<L, R> Append(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) =>
        Plus(x, y);

    [Pure]
    public static bool IsLeft(EitherUnsafe<L, R> choice) =>
        choice.State == EitherStatus.IsLeft;

    [Pure]
    public static bool IsRight(EitherUnsafe<L, R> choice) =>
        choice.State == EitherStatus.IsRight;

    [Pure]
    public static bool IsBottom(EitherUnsafe<L, R> choice) =>
        choice.State == EitherStatus.IsBottom;

    [Pure]
    public static C? MatchUnsafe<C>(EitherUnsafe<L, R> choice, Func<L?, C?> Left, Func<R?, C?> Right, Func<C?>? Bottom = null) =>
        choice.State == EitherStatus.IsBottom
            ? Bottom == null
                  ? throw new BottomException()
                  : Bottom()
            : choice.State == EitherStatus.IsLeft
                ? Left(choice.left)
                : Right(choice.right);

    [Pure]
    public static Unit Match(EitherUnsafe<L, R> choice, Action<L?> Left, Action<R?> Right, Action? Bottom = null)
    {
        if (choice.State == EitherStatus.IsRight)
        {
            Right(choice.right);
            return default;
        }

        if (choice.State == EitherStatus.IsLeft)
        {
            Left(choice.left);
            return default;
        }

        if (Bottom == null) throw new BottomException();
        Bottom();
        return default;
    }

    [Pure]
    public static EitherUnsafe<L, R> Apply(Func<R?, R?, R?> f, EitherUnsafe<L, R> fa, EitherUnsafe<L, R> fb) =>
        from a in fa
        from b in fb
        select f(a, b);
}
