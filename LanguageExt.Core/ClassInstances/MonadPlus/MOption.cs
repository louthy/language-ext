using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public struct MOption<A> :
        Optional<Option<A>, A>,
        MonadPlus<Option<A>, A>,
        Foldable<Option<A>, A>,
        BiFoldable<Option<A>, Unit, A>,
        Liftable<Option<A>, A>
    {
        public static readonly MOption<A> Inst = default(MOption<A>);

        [Pure]
        public MB Bind<MONADB, MB, B>(Option<A> ma, Func<A, MB> f) where MONADB : struct, Monad<MB, B> =>
            ma.IsSome && f != null
                ? f(ma.Value)
                : default(MONADB).Fail(ValueIsNoneException.Default);

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
        public Option<A> FromSeq(IEnumerable<A> xs)
        {
            var x = xs.Take(1).ToArray();
            return x.Length == 0 
                ? Option<A>.None
                : Return(x[0]);
        }

        [Pure]
        public Option<A> Return(A x) =>
            isnull(x)
                ? Option<A>.None
                : new Option<A>(new SomeValue<A>(x));

        [Pure]
        public Option<A> Lift(A x) =>
            Return(x);

        [Pure]
        public Option<A> Return(Func<A> f) =>
            Return(f());

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
        public B Match<B>(Option<A> opt, Func<A, B> Some, Func<B> None) =>
            opt.IsSome
                ? Check.NullReturn(Some(opt.Value))
                : Check.NullReturn(None());

        public Unit Match(Option<A> opt, Action<A> Some, Action None)
        {
            if (opt.IsSome) Some(opt.Value); else None();
            return Unit.Default;
        }

        [Pure]
        public B MatchUnsafe<B>(Option<A> opt, Func<A, B> Some, Func<B> None) =>
            opt.IsSome
                ? Some(opt.Value)
                : None();

        [Pure]
        public S Fold<S>(Option<A> ma, S state, Func<S, A, S> f) =>
            Check.NullReturn(ma.IsSome
                ? f(state, ma.Value)
                : state);

        [Pure]
        public S FoldBack<S>(Option<A> ma, S state, Func<S, A, S> f) =>
            Check.NullReturn(ma.IsSome
                ? f(state, ma.Value)
                : state);

        [Pure]
        public S BiFold<S>(Option<A> ma, S state, Func<S, Unit, S> fa, Func<S, A, S> fb) =>
            Check.NullReturn(ma.IsNone
                ? fa(state, unit)
                : fb(state, ma.Value));

        [Pure]
        public S BiFoldBack<S>(Option<A> ma, S state, Func<S, Unit, S> fa, Func<S, A, S> fb) =>
            Check.NullReturn(ma.IsNone
                ? fa(state, unit)
                : fb(state, ma.Value));

        [Pure]
        public int Count(Option<A> ma) =>
            ma.IsSome
                ? 1
                : 0;
    }
}
