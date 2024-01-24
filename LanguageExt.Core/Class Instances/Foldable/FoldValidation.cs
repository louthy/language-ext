using LanguageExt.TypeClasses;
using System;

namespace LanguageExt.ClassInstances
{
    public struct FoldValidation<MonoidFail, FAIL, SUCCESS> :
        Choice<Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>,
        BiFoldable<Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>,
        Foldable<Validation<MonoidFail, FAIL, SUCCESS>, SUCCESS>
        where MonoidFail : Monoid<FAIL>, Eq<FAIL>
    {
        public static Validation<MonoidFail, FAIL, SUCCESS> Append(Validation<MonoidFail, FAIL, SUCCESS> x, Validation<MonoidFail, FAIL, SUCCESS> y) =>
            x.Match(
                Succ: _ => y.Match(
                    Succ: _ => x,
                    Fail: _ => y),
                Fail: xf => y.Match(
                    Succ: _ => x,
                    Fail: yf => Validation<MonoidFail, FAIL, SUCCESS>.Fail(MonoidFail.Append(xf, yf))));

        public static S BiFold<S>(Validation<MonoidFail, FAIL, SUCCESS> foldable, S state, Func<S, FAIL, S> fa, Func<S, SUCCESS, S> fb) =>
            foldable.Match(
                Fail:    f => fa(state, f),
                Succ:    s => fb(state, s));

        public static S BiFoldBack<S>(Validation<MonoidFail, FAIL, SUCCESS> foldable, S state, Func<S, FAIL, S> fa, Func<S, SUCCESS, S> fb) =>
            foldable.Match(
                Fail: f => fa(state, f),
                Succ: s => fb(state, s));

        public static Func<Unit, int> Count(Validation<MonoidFail, FAIL, SUCCESS> fa) => _ =>
            fa.Match(
                Fail: f => 0,
                Succ: s => 1);

        public static Validation<MonoidFail, FAIL, SUCCESS> Empty() =>
            Validation<MonoidFail, FAIL, SUCCESS>.Fail(MonoidFail.Empty());

        public static Func<Unit, S> Fold<S>(Validation<MonoidFail, FAIL, SUCCESS> fa, S state, Func<S, SUCCESS, S> f) => _ => 
            fa.Match(
                Fail: x => state,
                Succ: s => f(state, s));

        public static Func<Unit, S> FoldBack<S>(Validation<MonoidFail, FAIL, SUCCESS> fa, S state, Func<S, SUCCESS, S> f) => _ =>
            fa.Match(
                Fail: x => state,
                Succ: s => f(state, s));

        public static bool IsBottom(Validation<MonoidFail, FAIL, SUCCESS> choice) =>
            false;

        public static bool IsLeft(Validation<MonoidFail, FAIL, SUCCESS> choice) =>
            choice.IsFail;

        public static bool IsRight(Validation<MonoidFail, FAIL, SUCCESS> choice) =>
            choice.IsSuccess;

        public static C Match<C>(Validation<MonoidFail, FAIL, SUCCESS> choice, Func<FAIL, C> Left, Func<SUCCESS, C> Right, Func<C>? Bottom = null) =>
            choice.Match(Right, Left);

        public static Unit Match(Validation<MonoidFail, FAIL, SUCCESS> choice, Action<FAIL> Left, Action<SUCCESS> Right, Action? Bottom = null) =>
            choice.Match(Right, Left);
    }
}
