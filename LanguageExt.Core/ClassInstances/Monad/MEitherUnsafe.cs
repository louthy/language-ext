using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct MEitherUnsafe<L, R> :
        Choice<EitherUnsafe<L, R>, L, R>,
        Alternative<EitherUnsafe<L, R>, L, R>,
        Monad<EitherUnsafe<L, R>, R>,
        Optional<EitherUnsafe<L, R>, R>,
        BiFoldable<EitherUnsafe<L, R>, L, R>
    {
        public static readonly MEitherUnsafe<L, R> Inst = default(MEitherUnsafe<L, R>);

        [Pure]
        public MB Bind<MONADB, MB, B>(EitherUnsafe<L, R> ma, Func<R, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            Match(ma,
                Left: l => default(MONADB).Fail(l),
                Right: r => f(r),
                Bottom: () => default(MONADB).Fail(BottomException.Default));

        [Pure]
        public EitherUnsafe<L, R> Fail(object err) =>
            err is L
                ? EitherUnsafe<L, R>.Left((L)err)
                : EitherUnsafe<L, R>.Bottom;

        [Pure]
        public EitherUnsafe<L, R> Fail(Exception err = null) =>
            EitherUnsafe<L, R>.Bottom;

        [Pure]
        public EitherUnsafe<L, R> Plus(EitherUnsafe<L, R> ma, EitherUnsafe<L, R> mb) =>
            Match(ma,
                Left: _ => mb,
                Right: _ => ma,
                Bottom: () => mb);

        [Pure]
        public EitherUnsafe<L, R> Return(Func<Unit, R> f) =>
            f(unit);

        [Pure]
        public EitherUnsafe<L, R> Zero() =>
            default(EitherUnsafe<L, R>);

        [Pure]
        public bool IsNone(EitherUnsafe<L, R> opt) =>
            !opt.IsRight;

        [Pure]
        public bool IsSome(EitherUnsafe<L, R> opt) =>
            opt.IsRight;

        [Pure]
        public R2 Match<R2>(EitherUnsafe<L, R> opt, Func<R, R2> Some, Func<R2> None) =>
            opt.IsRight
                ? Some(opt.RightValue)
                : None();

        public Unit Match(EitherUnsafe<L, R> opt, Action<R> Some, Action None)
        {
            if (opt.IsRight) Some(opt.RightValue); else None();
            return Unit.Default;
        }

        [Pure]
        public R2 MatchUnsafe<R2>(EitherUnsafe<L, R> opt, Func<R, R2> Some, Func<R2> None) =>
            opt.IsRight
                ? Some(opt.RightValue)
                : None();

        [Pure]
        public Func<Unit, S> Fold<S>(EitherUnsafe<L, R> foldable, S state, Func<S, R, S> f) =>
            u =>
                foldable.MatchUnsafe(
                    Left: _ => state,
                    Right: _ => f(state, foldable.RightValue),
                    Bottom: () => state);

        [Pure]
        public Func<Unit, S> FoldBack<S>(EitherUnsafe<L, R> foldable, S state, Func<S, R, S> f) =>
            u =>
                foldable.MatchUnsafe(
                    Left: _ => state,
                    Right: _ => f(state, foldable.RightValue),
                    Bottom: () => state);

        [Pure]
        public S BiFold<S>(EitherUnsafe<L, R> foldable, S state, Func<S, L, S> fa, Func<S, R, S> fb) =>
            Match(foldable,
                Left: _ => fa(state, foldable.LeftValue),
                Right: _ => fb(state, foldable.RightValue),
                Bottom: () => state);

        [Pure]
        public S BiFoldBack<S>(EitherUnsafe<L, R> foldable, S state, Func<S, L, S> fa, Func<S, R, S> fb) =>
            Match(foldable,
                Left: _ => fa(state, foldable.LeftValue),
                Right: _ => fb(state, foldable.RightValue),
                Bottom: () => state);

        [Pure]
        public Func<Unit, int> Count(EitherUnsafe<L, R> ma)
        {
            var self = this;
            return u => self.Match(ma,
                Left: _ => 0,
                Right: _ => 1,
                Bottom: () => 0);
        }

        [Pure]
        public EitherUnsafe<L, R> Some(R value) =>
            value;

        [Pure]
        public EitherUnsafe<L, R> Optional(R value) =>
            value;

        [Pure]
        public EitherUnsafe<L, R> None =>
            default(R);

        static A DefaultBottom<A>() =>
            raise<A>(new BottomException());

        static void DefaultBottom()
        {
            throw new BottomException();
        }

        [Pure]
        public EitherUnsafe<L, R> Id(Func<Unit, EitherUnsafe<L, R>> ma) =>
            ma(unit);

        [Pure]
        public EitherUnsafe<L, R> BindReturn(Unit _, EitherUnsafe<L, R> mb) =>
            mb;

        [Pure]
        public EitherUnsafe<L, R> Return(R x) =>
            Return(_ => x);

        [Pure]
        public EitherUnsafe<L, R> IdAsync(Func<Unit, Task<EitherUnsafe<L, R>>> ma) =>
            ma(unit).Result;

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(EitherUnsafe<L, R> fa, S state, Func<S, R, S> f) => _ =>
            Task.FromResult(Inst.Fold<S>(fa, state, f)(_));

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(EitherUnsafe<L, R> fa, S state, Func<S, R, Task<S>> f) => _ =>
            fa.MatchUnsafe(
                Right: r   => f(state, r),
                Left: l    => Task.FromResult(state),
                Bottom: () => Task.FromResult(state));

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(EitherUnsafe<L, R> fa, S state, Func<S, R, S> f) => _ =>
             Task.FromResult(Inst.FoldBack<S>(fa, state, f)(_));

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(EitherUnsafe<L, R> fa, S state, Func<S, R, Task<S>> f) => _ =>
            fa.MatchUnsafe(
                Right: r   => f(state, r),
                Left: l    => Task.FromResult(state),
                Bottom: () => Task.FromResult(state));

        [Pure]
        public Func<Unit, Task<int>> CountAsync(EitherUnsafe<L, R> fa) => _ =>
            Task.FromResult(Inst.Count(fa)(_));

        [Pure]
        public EitherUnsafe<L, R> Empty() =>
            EitherUnsafe<L, R>.Bottom;

        [Pure]
        public EitherUnsafe<L, R> Append(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) =>
            Plus(x, y);

        [Pure]
        public bool IsUnsafe(EitherUnsafe<L, R> choice) =>
            true;

        [Pure]
        public bool IsLeft(EitherUnsafe<L, R> choice) =>
            choice.State == EitherStatus.IsLeft;

        [Pure]
        public bool IsRight(EitherUnsafe<L, R> choice) =>
            choice.State == EitherStatus.IsRight;

        [Pure]
        public bool IsBottom(EitherUnsafe<L, R> choice) =>
            choice.State == EitherStatus.IsBottom;

        [Pure]
        public C Match<C>(EitherUnsafe<L, R> choice, Func<L, C> Left, Func<R, C> Right, Func<C> Bottom = null) =>
            choice.State == EitherStatus.IsBottom
                ? Bottom == null
                    ? throw new BottomException()
                    : Bottom()
                : choice.State == EitherStatus.IsLeft
                    ? Left(choice.left)
                    : Right(choice.right);

        [Pure]
        public Unit Match(EitherUnsafe<L, R> choice, Action<L> Left, Action<R> Right, Action Bottom = null)
        {
            if (choice.State == EitherStatus.IsRight && Right != null)
            {
                Right(choice.right);
            }
            else if (choice.State == EitherStatus.IsLeft && Left != null)
            {
                Left(choice.left);
            }
            else if (choice.State == EitherStatus.IsBottom && Bottom != null)
            {
                Bottom();
            }
            else if (choice.State == EitherStatus.IsBottom && Bottom == null)
            {
                throw new BottomException();
            }

            return unit;
        }

        [Pure]
        public C MatchUnsafe<C>(EitherUnsafe<L, R> choice, Func<L, C> Left, Func<R, C> Right, Func<C> Bottom = null) =>
            choice.State == EitherStatus.IsBottom
                ? Bottom == null
                    ? throw new BottomException()
                    : Bottom()
                : choice.State == EitherStatus.IsLeft
                    ? Left(choice.left)
                    : Right(choice.right);
    }
}
