using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct MEither<L, R> :
        Choice<Either<L, R>, L, R>,
        Monad<Either<L, R>, R>,
        Optional<Either<L, R>, R>,
        Foldable<Either<L, R>, R>,
        BiFoldable<Either<L, R>, L, R>
    {
        public static readonly MEither<L, R> Inst = default(MEither<L, R>);

        [Pure]
        public MB Bind<MONADB, MB, B>(Either<L, R> ma, Func<R, MB> f) where MONADB : struct, Monad<MB, B> =>
            Match(ma,
                Choice1: l => default(MONADB).Fail(l),
                Choice2: r => f(r),
                Bottom: () => default(MONADB).Fail(BottomException.Default));

        [Pure]
        public Either<L, R> Fail(object err) =>
            Either<L, R>.Left((L)err);

        [Pure]
        public Either<L, R> Fail(Exception err = null) =>
            Either<L, R>.Bottom;

        [Pure]
        public Either<L, R> Plus(Either<L, R> ma, Either<L, R> mb) =>
            Match(ma,
                Choice1: _ => mb,
                Choice2: _ => ma,
                Bottom: () => mb);

        [Pure]
        public Either<L, R> Return(R x) =>
            Either<L, R>.Right(x);

        [Pure]
        public Either<L, R> Return(Func<R> f) =>
            Return(f());

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
        public bool IsBottom(Either<L, R> choice) =>
            choice.IsBottom;

        [Pure]
        public bool IsUnsafe(Either<L, R> opt) =>
            false;

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
        public bool IsChoice1(Either<L, R> choice) =>
            choice.IsLeft;

        [Pure]
        public bool IsChoice2(Either<L, R> choice) =>
            choice.IsRight;

        [Pure]
        public C Match<C>(Either<L, R> choice, Func<L, C> Choice1, Func<R, C> Choice2, Func<C> Bottom = null) =>
            Check.NullReturn(choice.IsBottom
                ? (Bottom ?? DefaultBottom<C>)()
                : choice.IsRight
                    ? Choice2(choice.RightValue)
                    : Choice1(choice.LeftValue));

        /// <summary>
        /// Match the two states of the Choice and return a non-null C.
        /// </summary>
        /// <typeparam name="C">Return type</typeparam>
        public Unit Match(Either<L, R> choice, Action<L> Choice1, Action<R> Choice2, Action Bottom = null)
        {
            if(choice.IsBottom)
            {
                (Bottom ?? DefaultBottom)();
                return unit;
            }
            if (choice.IsRight) Choice2(choice.RightValue); else Choice1(choice.LeftValue);
            return unit;
        }

        [Pure]
        public C MatchUnsafe<C>(Either<L, R> choice, Func<L, C> Choice1, Func<R, C> Choice2, Func<C> Bottom = null) =>
            choice.IsBottom
                ? (Bottom ?? DefaultBottom<C>)()
                : choice.IsRight
                    ? Choice2(choice.RightValue)
                    : Choice1(choice.LeftValue);

        [Pure]
        public S Fold<S>(Either<L, R> foldable, S state, Func<S, R, S> f) =>
            Check.NullReturn(
                Match(foldable,
                    Choice1: _ => state,
                    Choice2: _ => f(state, foldable.RightValue),
                    Bottom: () => state));

        [Pure]
        public S FoldBack<S>(Either<L, R> foldable, S state, Func<S, R, S> f) =>
            Check.NullReturn(
                Match(foldable,
                    Choice1: _ => state,
                    Choice2: _ => f(state, foldable.RightValue),
                    Bottom: () => state));

        [Pure]
        public S BiFold<S>(Either<L, R> foldable, S state, Func<S, L, S> fa, Func<S, R, S> fb) =>
            Check.NullReturn(
                Match(foldable,
                    Choice1: _ => fa(state, foldable.LeftValue),
                    Choice2: _ => fb(state, foldable.RightValue),
                    Bottom: () => state));

        [Pure]
        public S BiFoldBack<S>(Either<L, R> foldable, S state, Func<S, L, S> fa, Func<S, R, S> fb) =>
            Check.NullReturn(
                Match(foldable,
                    Choice1: _ => fa(state, foldable.LeftValue),
                    Choice2: _ => fb(state, foldable.RightValue),
                    Bottom: () => state));

        [Pure]
        public int Count(Either<L, R> ma) =>
            Match(ma,
                Choice1: _ => 0,
                Choice2: _ => 1,
                Bottom: () => 0);

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
    }
}
