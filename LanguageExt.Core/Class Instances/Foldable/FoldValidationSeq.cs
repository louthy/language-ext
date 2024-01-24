using System;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct FoldValidation<FAIL, SUCCESS> :
        Choice<Validation<FAIL, SUCCESS>, Seq<FAIL>, SUCCESS>,
        BiFoldable<Validation<FAIL, SUCCESS>, FAIL, SUCCESS>,
        Foldable<Validation<FAIL, SUCCESS>, SUCCESS>
    {
        public static Validation<FAIL, SUCCESS> Append(Validation<FAIL, SUCCESS> x, Validation<FAIL, SUCCESS> y) =>
            y.Match(
                Succ: _ => x,
                Fail: yf => x.Match(
                    Succ: _ => y,
                    Fail: xf => Validation<FAIL, SUCCESS>.Fail(MSeq<FAIL>.Append(xf, yf))));

        public static S BiFold<S>(Validation<FAIL, SUCCESS> foldable, S state, Func<S, FAIL, S> fa, Func<S, SUCCESS, S> fb) =>
            foldable.Match(
                Fail: f => f.Fold(state, fa),
                Succ: s => fb(state, s));

        public static S BiFoldBack<S>(Validation<FAIL, SUCCESS> foldable, S state, Func<S, FAIL, S> fa, Func<S, SUCCESS, S> fb) =>
            foldable.Match(
                Fail: f => f.FoldBack(state, fa),
                Succ: s => fb(state, s));

        public static Func<Unit, int> Count(Validation<FAIL, SUCCESS> fa) => _ =>
            fa.Match(
                Fail: _ => 0,
                Succ: _ => 1);

        public static Validation<FAIL, SUCCESS> Empty() =>
            Validation<FAIL, SUCCESS>.Fail(Seq<FAIL>.Empty);

        public static Func<Unit, S> Fold<S>(Validation<FAIL, SUCCESS> fa, S state, Func<S, SUCCESS, S> f) => _ =>
            fa.Match(
                Fail: _ => state,
                Succ: s => f(state, s));

        public static Func<Unit, S> FoldBack<S>(Validation<FAIL, SUCCESS> fa, S state, Func<S, SUCCESS, S> f) => _ =>
            fa.Match(
                Fail: _ => state,
                Succ: s => f(state, s));

        public static bool IsBottom(Validation<FAIL, SUCCESS> choice) =>
            false;

        public static bool IsLeft(Validation<FAIL, SUCCESS> choice) =>
            choice.IsFail;

        public static bool IsRight(Validation<FAIL, SUCCESS> choice) =>
            choice.IsSuccess;

        public static C Match<C>(Validation<FAIL, SUCCESS> choice, Func<Seq<FAIL>, C> Left, Func<SUCCESS, C> Right, Func<C>? Bottom = null) =>
            choice.Match(Right, Left);

        public static Unit Match(Validation<FAIL, SUCCESS> choice, Action<Seq<FAIL>> Left, Action<SUCCESS> Right, Action? Bottom = null) =>
            choice.Match(Right, Left);
    }
}
