using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct MEither<L, R> :
        Choice<Either<L, R>, L, R>,
        Alternative<Either<L, R>, L, R>,
        Monad<Either<L, R>, R>,
        Optional<Either<L, R>, R>,
        BiFoldable<Either<L, R>, L, R>
    {
        public static readonly MEither<L, R> Inst = default(MEither<L, R>);

        [Pure]
        public MB Bind<MONADB, MB, B>(Either<L, R> ma, Func<R, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            ma.Match(
                Left: l => default(MONADB).Fail(l),
                Right: r => f(r),
                Bottom: () => default(MONADB).Fail(BottomException.Default));

        [Pure]
        public Either<L, R> Fail(object err) =>
            err is L
                ? Either<L, R>.Left((L)err)
                : Either<L, R>.Bottom;

        [Pure]
        public Either<L, R> Fail(Exception err = null) =>
            Either<L, R>.Bottom;

        [Pure]
        public Either<L, R> Plus(Either<L, R> ma, Either<L, R> mb) =>
            ma.Match(
                Left: _ => mb,
                Right: _ => ma,
                Bottom: () => mb);

        [Pure]
        public Either<L, R> Return(Func<Unit, R> f) =>
            f(unit);

        [Pure]
        public Either<L, R> Zero() =>
            default(Either<L, R>);

        [Pure]
        public bool IsNone(Either<L, R> opt) =>
            !opt.IsRight;

        [Pure]
        public bool IsSome(Either<L, R> opt) =>
            opt.IsRight;

        [Pure]
        public R2 Match<R2>(Either<L, R> opt, Func<R, R2> Some, Func<R2> None) =>
            opt.IsRight
                ? Check.NullReturn(Some(opt.RightValue))
                : Check.NullReturn(None());

        public Unit Match(Either<L, R> opt, Action<R> Some, Action None)
        {
            if (opt.IsRight) Some(opt.RightValue); else None();
            return Unit.Default;
        }

        [Pure]
        public R2 MatchUnsafe<R2>(Either<L, R> opt, Func<R, R2> Some, Func<R2> None) =>
            opt.IsRight
                ? Some(opt.RightValue)
                : None();

        [Pure]
        public Func<Unit, S> Fold<S>(Either<L, R> foldable, S state, Func<S, R, S> f) =>
            u => Check.NullReturn(
                foldable.Match(
                    Left:  _ => state,
                    Right: _ => f(state, foldable.RightValue),
                    Bottom: () => state));

        [Pure]
        public Func<Unit, S> FoldBack<S>(Either<L, R> foldable, S state, Func<S, R, S> f) =>
            u => Check.NullReturn(
                foldable.Match(
                    Left:  _ => state,
                    Right: _ => f(state, foldable.RightValue),
                    Bottom: () => state));

        [Pure]
        public S BiFold<S>(Either<L, R> foldable, S state, Func<S, L, S> fa, Func<S, R, S> fb) =>
            Check.NullReturn(
                foldable.Match(
                    Left: _ => fa(state, foldable.LeftValue),
                    Right: _ => fb(state, foldable.RightValue),
                    Bottom: () => state));

        [Pure]
        public S BiFoldBack<S>(Either<L, R> foldable, S state, Func<S, L, S> fa, Func<S, R, S> fb) =>
            Check.NullReturn(
                foldable.Match(
                    Left: _ => fa(state, foldable.LeftValue),
                    Right: _ => fb(state, foldable.RightValue),
                    Bottom: () => state));

        [Pure]
        public Func<Unit, int> Count(Either<L, R> ma)
        {
            return u => ma.Match(
                Left: _ => 0,
                Right: _ => 1,
                Bottom: () => 0);
        }

        [Pure]
        public Either<L, R> None =>
            default(R);

        static A DefaultBottom<A>() =>
            raise<A>(new BottomException());

        static void DefaultBottom()
        {
            throw new BottomException();
        }

        [Pure]
        public Either<L, R> Some(R value) =>
            value;

        [Pure]
        public Either<L, R> Optional(R value) =>
            value;

        [Pure]
        public Either<L, R> Id(Func<Unit, Either<L, R>> ma) =>
            ma(unit);

        [Pure]
        public Either<L, R> BindReturn(Unit _, Either<L, R> mb) =>
            mb;

        [Pure]
        public Either<L, R> Return(R x) =>
            Return(_ => x);

        [Pure]
        public Either<L, R> IdAsync(Func<Unit, Task<Either<L, R>>> ma) =>
            ma(unit).Result;

        [Pure]
        public Either<L, R> Empty() =>
            Either<L, R>.Bottom;

        [Pure]
        public Either<L, R> Append(Either<L, R> x, Either<L, R> y) =>
            Plus(x, y);

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(Either<L, R> fa, S state, Func<S, R, S> f) => _ =>
            Task.FromResult(Inst.Fold<S>(fa, state, f)(_));

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(Either<L, R> fa, S state, Func<S, R, Task<S>> f) => _ =>
            fa.Match(
                Right: r => f(state, r),
                Left: l => Task.FromResult(state),
                Bottom: () => Task.FromResult(state));

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(Either<L, R> fa, S state, Func<S, R, S> f) => _ =>
             Task.FromResult(Inst.FoldBack<S>(fa, state, f)(_));

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(Either<L, R> fa, S state, Func<S, R, Task<S>> f) => _ =>
            fa.Match(
                Right: r => f(state, r),
                Left: l => Task.FromResult(state),
                Bottom: () => Task.FromResult(state));

        [Pure]
        public Func<Unit, Task<int>> CountAsync(Either<L, R> fa) => _ =>
            Task.FromResult(Inst.Count(fa)(_));

        [Pure]
        public bool IsUnsafe(Either<L, R> choice) =>
            false;

        [Pure]
        public bool IsLeft(Either<L, R> choice) =>
            choice.State == EitherStatus.IsLeft;

        [Pure]
        public bool IsRight(Either<L, R> choice) =>
            choice.State == EitherStatus.IsRight;

        [Pure]
        public bool IsBottom(Either<L, R> choice) =>
            choice.State == EitherStatus.IsBottom;


        [Pure]
        public C Match<C>(Either<L, R> choice, Func<L, C> Left, Func<R, C> Right, Func<C> Bottom = null) =>
            choice.State == EitherStatus.IsBottom
                ? Bottom == null
                    ? throw new BottomException()
                    : Check.NullReturn(Bottom())
                : choice.State == EitherStatus.IsLeft
                    ? Check.NullReturn(Left(choice.left))
                    : Check.NullReturn(Right(choice.right));

        [Pure]
        public Unit Match(Either<L, R> choice, Action<L> Left, Action<R> Right, Action Bottom = null)
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
        public C MatchUnsafe<C>(Either<L, R> choice, Func<L, C> Left, Func<R, C> Right, Func<C> Bottom = null) =>
            choice.State == EitherStatus.IsBottom
                ? Bottom == null
                    ? throw new BottomException()
                    : Bottom()
                : choice.State == EitherStatus.IsLeft
                    ? Left(choice.left)
                    : Right(choice.right);

    }
}
