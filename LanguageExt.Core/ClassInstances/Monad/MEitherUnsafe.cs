using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct MEitherUnsafe<L, R> :
        Choice<EitherUnsafe<L, R>, L, R>,
        Monad<EitherUnsafe<L, R>, R>,
        Optional<EitherUnsafe<L, R>, R>,
        Foldable<EitherUnsafe<L, R>, R>,
        BiFoldable<EitherUnsafe<L, R>, L, R>
    {
        public static readonly MEitherUnsafe<L, R> Inst = default(MEitherUnsafe<L, R>);

        [Pure]
        public MB Bind<MONADB, MB, B>(EitherUnsafe<L, R> ma, Func<R, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            Match(ma,
                Choice1: l => default(MONADB).Fail(l),
                Choice2: r => f(r),
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
                Choice1: _ => mb,
                Choice2: _ => ma,
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
        public bool IsBottom(EitherUnsafe<L, R> choice) =>
            choice.IsBottom;

        [Pure]
        public bool IsUnsafe(EitherUnsafe<L, R> opt) =>
            false;

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
        public bool IsChoice1(EitherUnsafe<L, R> choice) =>
            choice.IsLeft;

        [Pure]
        public bool IsChoice2(EitherUnsafe<L, R> choice) =>
            choice.IsRight;

        [Pure]
        public C Match<C>(EitherUnsafe<L, R> choice, Func<L, C> Choice1, Func<R, C> Choice2, Func<C> Bottom = null) =>
            choice.IsBottom
                ? (Bottom ?? DefaultBottom<C>)()
                : choice.IsRight
                    ? Choice2(choice.RightValue)
                    : Choice1(choice.LeftValue);

        /// <summary>
        /// Match the two states of the Choice and return a non-null C.
        /// </summary>
        /// <typeparam name="C">Return type</typeparam>
        public Unit Match(EitherUnsafe<L, R> choice, Action<L> Choice1, Action<R> Choice2, Action Bottom = null)
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
        public C MatchUnsafe<C>(EitherUnsafe<L, R> choice, Func<L, C> Choice1, Func<R, C> Choice2, Func<C> Bottom = null) =>
            choice.IsBottom
                ? (Bottom ?? DefaultBottom<C>)()
                : choice.IsRight
                    ? Choice2(choice.RightValue)
                    : Choice1(choice.LeftValue);

        [Pure]
        public Func<Unit, S> Fold<S>(EitherUnsafe<L, R> foldable, S state, Func<S, R, S> f)
        {
            var self = this;
            return u => 
                self.Match(foldable,
                    Choice1: _ => state,
                    Choice2: _ => f(state, foldable.RightValue),
                    Bottom: () => state);
        }

        [Pure]
        public Func<Unit, S> FoldBack<S>(EitherUnsafe<L, R> foldable, S state, Func<S, R, S> f)
        {
            var self = this;
            return u => 
                self.Match(foldable,
                    Choice1: _ => state,
                    Choice2: _ => f(state, foldable.RightValue),
                    Bottom: () => state);
        }

        [Pure]
        public S BiFold<S>(EitherUnsafe<L, R> foldable, S state, Func<S, L, S> fa, Func<S, R, S> fb) =>
            Match(foldable,
                Choice1: _ => fa(state, foldable.LeftValue),
                Choice2: _ => fb(state, foldable.RightValue),
                Bottom: () => state);

        [Pure]
        public S BiFoldBack<S>(EitherUnsafe<L, R> foldable, S state, Func<S, L, S> fa, Func<S, R, S> fb) =>
            Match(foldable,
                Choice1: _ => fa(state, foldable.LeftValue),
                Choice2: _ => fb(state, foldable.RightValue),
                Bottom: () => state);

        [Pure]
        public Func<Unit, int> Count(EitherUnsafe<L, R> ma)
        {
            var self = this;
            return u => self.Match(ma,
                Choice1: _ => 0,
                Choice2: _ => 1,
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
    }
}
