using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public struct MNullable<A> :
        Optional<A?, A>,
        MonadPlus<A?, A>,
        Foldable<A?, A>,
        BiFoldable<A?, Unit, A>
        where A : struct
    {
        public static readonly MNullable<A> Inst = default(MNullable<A>);

        public MB Bind<MONADB, MB, B>(A? ma, Func<A, MB> f) where MONADB : struct, Monad<MB, B> =>
            ma.HasValue && f != null
                ? f(ma.Value)
                : default(MONADB).Fail(ValueIsNoneException.Default);

        [Pure]
        public A? Fail(object err) =>
            null;

        [Pure]
        public A? Fail(Exception err = null) =>
            null;

        [Pure]
        public A? Plus(A? a, A? b) =>
            a.HasValue
                ? a
                : b;

        [Pure]
        public A? FromSeq(IEnumerable<A> xs)
        {
            var x = xs.Take(1).ToArray();
            return x.Length == 0 
                ? null
                : Return(x[0]);
        }

        [Pure]
        public A? Return(A x) =>
            isnull(x)
                ? null
                : (A?)x;

        [Pure]
        public A? Return(Func<A> f) =>
            Return(f());

        [Pure]
        public A? Zero() =>
            null;

        [Pure]
        public bool IsNone(A? ma) =>
            !ma.HasValue;

        [Pure]
        public bool IsSome(A? ma) =>
            ma.HasValue;

        [Pure]
        public bool IsUnsafe(A? ma) =>
            true;

        [Pure]
        public B Match<B>(A? ma, Func<A, B> Some, Func<B> None) =>
            ma.HasValue
                ? Check.NullReturn(Some(ma.Value))
                : Check.NullReturn(None());

        public Unit Match(A? ma, Action<A> Some, Action None)
        {
            if (ma.HasValue) Some(ma.Value); else None();
            return Unit.Default;
        }

        [Pure]
        public B MatchUnsafe<B>(A? ma, Func<A, B> Some, Func<B> None) =>
            ma.HasValue
                ? Some(ma.Value)
                : None();

        [Pure]
        public S Fold<S>(A? ma, S state, Func<S, A, S> f) =>
            Check.NullReturn(ma.HasValue
                ? f(state, ma.Value)
                : state);

        [Pure]
        public S FoldBack<S>(A? ma, S state, Func<S, A, S> f) =>
            Check.NullReturn(ma.HasValue
                ? f(state, ma.Value)
                : state);

        [Pure]
        public S BiFold<S>(A? ma, S state, Func<S, Unit, S> fa, Func<S, A, S> fb) =>
            Check.NullReturn(!ma.HasValue
                ? fa(state, unit)
                : fb(state, ma.Value));

        [Pure]
        public S BiFoldBack<S>(A? ma, S state, Func<S, Unit, S> fa, Func<S, A, S> fb) =>
            Check.NullReturn(!ma.HasValue
                ? fa(state, unit)
                : fb(state, ma.Value));

        [Pure]
        public int Count(A? ma) =>
            ma.HasValue
                ? 1
                : 0;
    }
}
