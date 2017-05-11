using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct MOptionUnsafe<A> :
        Alternative<OptionUnsafe<A>, Unit, A>,
        Monad<OptionUnsafe<A>, A>,
        Optional<OptionUnsafe<A>, A>,
        Foldable<OptionUnsafe<A>, A>,
        BiFoldable<OptionUnsafe<A>, A, Unit>,
        Eq<OptionUnsafe<A>>
    {
        public static readonly MOptionUnsafe<A> Inst = default(MOptionUnsafe<A>);

        [Pure]
        public OptionUnsafe<A> None => OptionUnsafe<A>.None;

        [Pure]
        public MB Bind<MONADB, MB, B>(OptionUnsafe<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            ma.IsLazy
                ? default(MONADB).Id(_ =>
                    ma.IsSome && f != null
                        ? f(ma.Value)
                        : default(MONADB).Fail(ValueIsNoneException.Default))
                : ma.IsSome && f != null
                    ? f(ma.Value)
                    : default(MONADB).Fail(ValueIsNoneException.Default);

        [Pure]
        public OptionUnsafe<A> Fail(object err) =>
            OptionUnsafe<A>.None;

        [Pure]
        public OptionUnsafe<A> Fail(Exception err = null) =>
            OptionUnsafe<A>.None;

        [Pure]
        public OptionUnsafe<A> Plus(OptionUnsafe<A> a, OptionUnsafe<A> b) =>
            a.IsLazy
                ? Id(_ =>
                      a.IsSome
                          ? a
                          : b)
                : a.IsSome
                    ? a
                    : b;

        [Pure]
        public OptionUnsafe<A> Return(Func<Unit, A> f) =>
            new OptionUnsafe<A>(OptionData.Lazy(() => (true, f(unit))));

        [Pure]
        public OptionUnsafe<A> Zero() =>
            OptionUnsafe<A>.None;

        [Pure]
        public bool IsNone(OptionUnsafe<A> opt) =>
            opt.IsNone;

        [Pure]
        public bool IsSome(OptionUnsafe<A> opt) =>
            opt.IsSome;

        [Pure]
        public bool IsUnsafe(OptionUnsafe<A> opt) =>
            true;

        [Pure]
        public B Match<B>(OptionUnsafe<A> opt, Func<A, B> Some, Func<B> None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));
            return opt.IsSome
                ? Some(opt.Value)
                : None();
        }

        public Unit Match(OptionUnsafe<A> opt, Action<A> Some, Action None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));
            if (opt.IsSome) Some(opt.Value); else None();
            return unit;
        }

        [Pure]
        public B MatchUnsafe<B>(OptionUnsafe<A> opt, Func<A, B> Some, Func<B> None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));
            return opt.IsSome
                ? Some(opt.Value)
                : None();
        }

        [Pure]
        public Func<Unit, S> Fold<S>(OptionUnsafe<A> ma, S state, Func<S, A, S> f) => _ =>
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (f == null) throw new ArgumentNullException(nameof(f));
            return ma.IsSome
                ? f(state, ma.Value)
                : state;
        };

        [Pure]
        public Func<Unit, S> FoldBack<S>(OptionUnsafe<A> ma, S state, Func<S, A, S> f) => _ =>
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (f == null) throw new ArgumentNullException(nameof(f));
            return ma.IsSome
                ? f(state, ma.Value)
                : state;
        };

        [Pure]
        public S BiFold<S>(OptionUnsafe<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb)
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (fb == null) throw new ArgumentNullException(nameof(fb));
            return ma.IsNone
                ? fa(state, ma.Value)
                : fb(state, unit);
        }

        [Pure]
        public S BiFoldBack<S>(OptionUnsafe<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb)
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (fb == null) throw new ArgumentNullException(nameof(fb));
            return ma.IsNone
                ? fa(state, ma.Value)
                : fb(state, unit);
        }

        [Pure]
        public Func<Unit, int> Count(OptionUnsafe<A> ma) => _ =>
            ma.IsSome
                ? 1
                : 0;

        [Pure]
        public bool IsLeft(OptionUnsafe<A> choice) =>
            choice.IsNone;

        [Pure]
        public bool IsRight(OptionUnsafe<A> choice) =>
            choice.IsSome;

        [Pure]
        public bool IsBottom(OptionUnsafe<A> choice) =>
            false;

        [Pure]
        public C Match<C>(OptionUnsafe<A> choice, Func<Unit, C> Left, Func<A, C> Right, Func<C> Bottom = null) =>
            choice.MatchUnsafe(
                Some: Right,
                None: () => Left(unit));

        [Pure]
        public Unit Match(OptionUnsafe<A> choice, Action<Unit> Left, Action<A> Right, Action Bottom = null) =>
            choice.MatchUnsafe(
                Some: Right,
                None: () => Left(unit));

        [Pure]
        public C MatchUnsafe<C>(OptionUnsafe<A> choice, Func<Unit, C> Left, Func<A, C> Right, Func<C> Bottom = null) =>
            choice.MatchUnsafe(
                Some: Right,
                None: () => Left(unit));

        [Pure]
        public OptionUnsafe<A> Some(A x) =>
            new OptionUnsafe<A>(OptionData.Some(x));

        [Pure]
        public OptionUnsafe<A> Optional(A x) =>
            new OptionUnsafe<A>(OptionData.Some(x));

        [Pure]
        public OptionUnsafe<A> Id(Func<Unit, OptionUnsafe<A>> ma) =>
            new OptionUnsafe<A>(OptionData.Lazy(() =>
            {
                var a = ma(unit);
                return (a.IsSome, a.Value);
            }));

        [Pure]
        public OptionUnsafe<A> BindReturn(Unit _, OptionUnsafe<A> mb) =>
            mb;

        [Pure]
        public OptionUnsafe<A> Return(A x) =>
            Optional(x);

        [Pure]
        public OptionUnsafe<A> IdAsync(Func<Unit, Task<OptionUnsafe<A>>> ma) =>
            ma(unit).Result;

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(OptionUnsafe<A> fa, S state, Func<S, A, S> f) => _ =>
            Task.FromResult(Inst.Fold<S>(fa, state, f)(_));

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(OptionUnsafe<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
            fa.MatchUnsafe(
                Some: r => f(state, r),
                None: () => Task.FromResult(state));

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(OptionUnsafe<A> fa, S state, Func<S, A, S> f) => _ =>
             Task.FromResult(Inst.FoldBack<S>(fa, state, f)(_));

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(OptionUnsafe<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
            fa.MatchUnsafe(
                Some: r => f(state, r),
                None: () => Task.FromResult(state));

        [Pure]
        public Func<Unit, Task<int>> CountAsync(OptionUnsafe<A> fa) => _ =>
            Task.FromResult(Inst.Count(fa)(_));

        [Pure]
        public OptionUnsafe<A> Empty() =>
            OptionUnsafe<A>.None;

        [Pure]
        public OptionUnsafe<A> Append(OptionUnsafe<A> x, OptionUnsafe<A> y) =>
            Plus(x, y);

        [Pure]
        public bool Equals(OptionUnsafe<A> x, OptionUnsafe<A> y) =>
            equals<EqDefault<A>, A>(x, y);

        [Pure]
        public int GetHashCode(OptionUnsafe<A> x) =>
            EqOpt<EqDefault<A>, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Inst.GetHashCode(x);
    }
}
