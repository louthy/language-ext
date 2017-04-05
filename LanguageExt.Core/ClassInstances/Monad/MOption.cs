using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct MOption<A> :
        Alternative<Option<A>, Unit, A>,
        Optional<Option<A>, A>,
        Monad<Option<A>, A>,
        BiFoldable<Option<A>, A, Unit>,
        Eq<Option<A>>
    {
        public static readonly MOption<A> Inst = default(MOption<A>);

        [Pure]
        public Option<A> None => Option<A>.None;

        [Pure]
        public MB Bind<MonadB, MB, B>(Option<A> ma, Func<A, MB> f) where MonadB : struct, Monad<Unit, Unit, MB, B> =>
            ma.IsLazy
                ? default(MonadB).Id(_ =>
                    ma.IsSome && f != null
                        ? f(ma.Value)
                        : default(MonadB).Fail())
                : ma.IsSome && f != null
                    ? f(ma.Value)
                    : default(MonadB).Fail();

        [Pure]
        public Option<A> Fail(object err) =>
            Option<A>.None;

        [Pure]
        public Option<A> Fail(Exception err = null) =>
            Option<A>.None;

        [Pure]
        public Option<A> Plus(Option<A> a, Option<A> b) =>
            a.IsLazy
                ? default(MOption<A>).Id(_ =>
                    a.IsSome
                        ? a
                        : b)
                : a.IsSome
                    ? a
                    : b;

        [Pure]
        public Option<A> Return(Func<Unit, A> f) =>
            new Option<A>(OptionData.Lazy(() =>
            {
                var a = f(unit);
                return (!a.IsNull(), a);
            }));

        [Pure]
        public Option<A> Zero() =>
            Option<A>.None;

        [Pure]
        public bool IsNone(Option<A> opt) =>
            opt.IsNone;

        [Pure]
        public bool IsSome(Option<A> opt) =>
            opt.IsSome;

        [Pure]
        public bool IsUnsafe(Option<A> opt) =>
            false;

        [Pure]
        public B Match<B>(Option<A> opt, Func<A, B> Some, Func<B> None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));
            return opt.IsSome
                ? Check.NullReturn(Some(opt.Value))
                : Check.NullReturn(None());
        }

        public Unit Match(Option<A> opt, Action<A> Some, Action None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));
            if (opt.IsSome) Some(opt.Value); else None();
            return Unit.Default;
        }

        [Pure]
        public B MatchUnsafe<B>(Option<A> opt, Func<A, B> Some, Func<B> None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));
            return opt.IsSome
                ? Some(opt.Value)
                : None();
        }

        [Pure]
        public Func<Unit, S> Fold<S>(Option<A> ma, S state, Func<S, A, S> f) => _ =>
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (f == null) throw new ArgumentNullException(nameof(f));
            return Check.NullReturn(ma.IsSome
                ? f(state, ma.Value)
                : state);
        };

        [Pure]
        public Func<Unit, S> FoldBack<S>(Option<A> ma, S state, Func<S, A, S> f) => _ =>
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (f == null) throw new ArgumentNullException(nameof(f));
            return Check.NullReturn(ma.IsSome
                ? f(state, ma.Value)
                : state);
        };

        [Pure]
        public S BiFold<S>(Option<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb)
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (fb == null) throw new ArgumentNullException(nameof(fb));
            return Check.NullReturn(ma.IsNone
                ? fa(state, ma.Value)
                : fb(state, unit));
        }

        [Pure]
        public S BiFoldBack<S>(Option<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb)
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (fb == null) throw new ArgumentNullException(nameof(fb));
            return Check.NullReturn(ma.IsNone
                ? fa(state, ma.Value)
                : fb(state, unit));
        }

        [Pure]
        public Func<Unit, int> Count(Option<A> ma) => _ =>
            ma.IsSome
                ? 1
                : 0;

        [Pure]
        public Option<A> Some(A x) =>
            x.IsNull()
                ? throw new ArgumentNullException("Option doesn't support null values.  Use OptionUnsafe if this is desired behaviour")
                : new Option<A>(OptionData.Some(x));

        [Pure]
        public Option<A> Optional(A x) =>
            new Option<A>(OptionData.Optional(x));

        [Pure]
        public Option<A> Id(Func<Unit, Option<A>> ma) =>
            new Option<A>(OptionData.Lazy(() =>
            {
                var a = ma(unit);
                return (a.IsSome, a.Value);
            }));

        [Pure]
        public Option<A> BindReturn(Unit _, Option<A> mb) =>
            mb;

        [Pure]
        public Option<A> Return(A x) =>
            Optional(x);

        [Pure]
        public Option<A> IdAsync(Func<Unit, Task<Option<A>>> ma) =>
            ma(unit).Result;

        [Pure]
        public Option<A> Empty() =>
            None;

        [Pure]
        public Option<A> Append(Option<A> x, Option<A> y) =>
            Plus(x, y);

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(Option<A> fa, S state, Func<S, A, S> f) => _ =>
            Task.FromResult(Inst.Fold<S>(fa, state, f)(_));

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(Option<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
            fa.Match(
                Some: r => f(state, r),
                None: () => Task.FromResult(state));

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(Option<A> fa, S state, Func<S, A, S> f) => _ =>
             Task.FromResult(Inst.FoldBack<S>(fa, state, f)(_));

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(Option<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
            fa.Match(
                Some: r => f(state, r),
                None: () => Task.FromResult(state));

        [Pure]
        public Func<Unit, Task<int>> CountAsync(Option<A> fa) => _ =>
            Task.FromResult(Inst.Count(fa)(_));

        [Pure]
        public bool Equals(Option<A> x, Option<A> y) =>
            equals<EqDefault<A>, A>(x, y);

        [Pure]
        public int GetHashCode(Option<A> x) =>
            EqOpt<EqDefault<A>, MOption<A>, Option<A>, A>.Inst.GetHashCode(x);
    }
}
