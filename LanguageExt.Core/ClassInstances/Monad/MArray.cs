using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Array type-class instance
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public struct MArray<A> :
        Monad<A[], A>,
        Foldable<A[], A>,
        Eq<A[]>,
        Monoid<A[]>
   {
        public static readonly MArray<A> Inst = default(MArray<A>);

        [Pure]
        public A[] Append(A[] x, A[] y) =>
            x.Concat(y).ToArray();

        [Pure]
        public MB Bind<MONADB, MB, B>(A[] ma, Func<A, MB> f) where MONADB : struct, Monad<MB, B> =>
            ma.Fold(default(MONADB).Zero(), (s, a) => default(MONADB).Plus(s, f(a)));

        [Pure]
        public int Count(A[] fa) =>
            fa.Count();

        [Pure]
        public A[] Subtract(A[] x, A[] y) =>
            Enumerable.Except(x, y).ToArray();

        [Pure]
        public A[] Empty() =>
            new A[0];

        [Pure]
        public bool Equals(A[] x, A[] y) =>
            Enumerable.SequenceEqual(x, y);

        [Pure]
        public A[] Fail(object err) =>
            Empty();

        [Pure]
        public A[] Fail(Exception err = null) =>
            Empty();

        [Pure]
        public S Fold<S>(A[] fa, S state, Func<S, A, S> f) =>
            fa.Fold(state, f);

        [Pure]
        public S FoldBack<S>(A[] fa, S state, Func<S, A, S> f) =>
            fa.FoldBack(state, f);

        [Pure]
        public A[] Plus(A[] ma, A[] mb) =>
            PlusSeq(ma, mb).ToArray();

        [Pure]
        IEnumerable<A> PlusSeq(A[] ma, A[] mb)
        {
            foreach (var a in ma) yield return a;
            foreach (var b in mb) yield return b;
        }

        [Pure]
        public A[] Return(A x) =>
            new[] { x };

        [Pure]
        public A[] Zero() =>
            Empty();

        [Pure]
        public int GetHashCode(A[] x) =>
            hash(x);
    }
}
