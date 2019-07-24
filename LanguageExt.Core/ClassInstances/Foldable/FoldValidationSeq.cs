using System;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct FoldValidation<FAIL, SUCCESS> :
        Choice<Validation<FAIL, SUCCESS>, Seq<FAIL>, SUCCESS>,
        Alternative<Validation<FAIL, SUCCESS>, FAIL, SUCCESS>,
        BiFoldable<Validation<FAIL, SUCCESS>, FAIL, SUCCESS>,
        Foldable<Validation<FAIL, SUCCESS>, SUCCESS>
    {
        public Validation<FAIL, SUCCESS> Append(Validation<FAIL, SUCCESS> x, Validation<FAIL, SUCCESS> y) =>
            y.Match(
                Succ: ys => x,
                Fail: yf => x.Match(
                    Succ: xs => y,
                    Fail: xf => Validation<FAIL, SUCCESS>.Fail(default(MSeq<FAIL>).Append(xf, yf))));

        public S BiFold<S>(Validation<FAIL, SUCCESS> foldable, S state, Func<S, FAIL, S> fa, Func<S, SUCCESS, S> fb) =>
            foldable.Match(
                Fail: f => f.Fold(state, fa),
                Succ: s => fb(state, s));

        public S BiFoldBack<S>(Validation<FAIL, SUCCESS> foldable, S state, Func<S, FAIL, S> fa, Func<S, SUCCESS, S> fb) =>
            foldable.Match(
                Fail: f => f.FoldBack(state, fa),
                Succ: s => fb(state, s));

        public Func<Unit, int> Count(Validation<FAIL, SUCCESS> fa) => _ =>
            fa.Match(
                Fail: f => 0,
                Succ: s => 1);

        public Validation<FAIL, SUCCESS> Empty() =>
            Validation<FAIL, SUCCESS>.Fail(Seq<FAIL>.Empty);

        public Func<Unit, S> Fold<S>(Validation<FAIL, SUCCESS> fa, S state, Func<S, SUCCESS, S> f) => _ =>
            fa.Match(
                Fail: x => state,
                Succ: s => f(state, s));

        public Func<Unit, S> FoldBack<S>(Validation<FAIL, SUCCESS> fa, S state, Func<S, SUCCESS, S> f) => _ =>
            fa.Match(
                Fail: x => state,
                Succ: s => f(state, s));

        public bool IsBottom(Validation<FAIL, SUCCESS> choice) =>
            false;

        public bool IsLeft(Validation<FAIL, SUCCESS> choice) =>
            choice.IsFail;

        public bool IsRight(Validation<FAIL, SUCCESS> choice) =>
            choice.IsSuccess;

        public C Match<C>(Validation<FAIL, SUCCESS> choice, Func<Seq<FAIL>, C> Left, Func<SUCCESS, C> Right, Func<C> Bottom = null) =>
            choice.Match(Right, Left);

        public Unit Match(Validation<FAIL, SUCCESS> choice, Action<Seq<FAIL>> Left, Action<SUCCESS> Right, Action Bottom = null) =>
            choice.Match(Right, Left);
    }
}
