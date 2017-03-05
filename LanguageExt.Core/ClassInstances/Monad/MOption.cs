using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct MOption<A> :
        Choice<Option<A>, Unit, A>,
        Optional<Option<A>, A>,
        Monad<Option<A>, A>,
        Foldable<Option<A>, A>,
        BiFoldable<Option<A>, Unit, A>
    {
        public static readonly MOption<A> Inst = default(MOption<A>);

        [Pure]
        public Option<A> None => Option<A>.None;

        [Pure]
        public MB Bind<MONADB, MB, B>(Option<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B>
        {
            if (f == null) throw new ArgumentNullException(nameof(f));
            return ma.IsSome && f != null
                ? f(ma.Value)
                : default(MONADB).Fail(ValueIsNoneException.Default);
        }

        [Pure]
        public Option<A> Fail(object err) =>
            Option<A>.None;

        [Pure]
        public Option<A> Fail(Exception err = null) =>
            Option<A>.None;

        [Pure]
        public Option<A> Plus(Option<A> a, Option<A> b) =>
            a.IsSome
                ? a
                : b;

        [Pure]
        public Option<A> Return(Func<Unit, A> f)
        {
            var x = f(unit);
            return isnull(x)
                ? Option<A>.None
                : new Option<A>(new SomeValue<A>(x));
        }

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
        public S BiFold<S>(Option<A> ma, S state, Func<S, Unit, S> fa, Func<S, A, S> fb)
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (fb == null) throw new ArgumentNullException(nameof(fb));
            return Check.NullReturn(ma.IsNone
                ? fa(state, unit)
                : fb(state, ma.Value));
        }

        [Pure]
        public S BiFoldBack<S>(Option<A> ma, S state, Func<S, Unit, S> fa, Func<S, A, S> fb)
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (fb == null) throw new ArgumentNullException(nameof(fb));
            return Check.NullReturn(ma.IsNone
                ? fa(state, unit)
                : fb(state, ma.Value));
        }

        [Pure]
        public Func<Unit, int> Count(Option<A> ma) => _ =>
            ma.IsSome
                ? 1
                : 0;

        [Pure]
        public bool IsChoice1(Option<A> choice) =>
            choice.IsNone;

        [Pure]
        public bool IsChoice2(Option<A> choice) =>
            choice.IsSome;

        [Pure]
        public bool IsBottom(Option<A> choice) =>
            false;

        [Pure]
        public C Match<C>(Option<A> choice, Func<Unit, C> Choice1, Func<A, C> Choice2, Func<C> Bottom = null) =>
            choice.Match(
                Some: Choice2,
                None: () => Choice1(unit));

        [Pure]
        public Unit Match(Option<A> choice, Action<Unit> Choice1, Action<A> Choice2, Action Bottom = null) =>
            choice.Match(
                Some: Choice2,
                None: () => Choice1(unit));

        [Pure]
        public C MatchUnsafe<C>(Option<A> choice, Func<Unit, C> Choice1, Func<A, C> Choice2, Func<C> Bottom = null) =>
            choice.Match(
                Some: Choice2,
                None: () => Choice1(unit));

        [Pure]
        public Option<A> Some(A value) =>
            Option<A>.Some(value);

        [Pure]
        public Option<A> Optional(A value) =>
            Prelude.Optional(value);

        [Pure]
        public Option<A> Id(Func<Unit, Option<A>> ma) =>
            ma(unit);

        [Pure]
        public Option<A> BindReturn(Unit _, Option<A> mb) =>
            mb;

        [Pure]
        public Option<A> Return(A x) =>
            Return(_ => x);

        [Pure]
        public Option<A> IdAsync(Func<Unit, Task<Option<A>>> ma) =>
            ma(unit).Result;
    }
}
