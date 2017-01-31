using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Array type-class instance
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public struct MArray<A> :
        MonadPlus<A[], A>,
        Foldable<A[], A>,
        Eq<A[]>,
        Monoid<A[]>
   {
        public static readonly MArray<A> Inst = default(MArray<A>);

        public A[] Append(A[] x, A[] y) =>
            x.Concat(y).ToArray();

        IEnumerable<B> BindSeq<MONADB, MB, B>(A[] ma, Func<A, MB> f)
            where MONADB : struct, Monad<MB, B>
        {
            foreach(var a in ma)
                foreach (var b in toSeq<MONADB, MB, B>(f(a)))
                    yield return b;
        }

        public MB Bind<MONADB, MB, B>(A[] ma, Func<A, MB> f) where MONADB : struct, Monad<MB, B> =>
            default(MONADB).FromSeq(BindSeq<MONADB, MB, B>(ma, f));

        public int Count(A[] fa) =>
            fa.Count();

        public A[] Subtract(A[] x, A[] y) =>
            Enumerable.Except(x, y).ToArray();

        public A[] Empty() =>
            new A[0];

        public bool Equals(A[] x, A[] y) =>
            Enumerable.SequenceEqual(x, y);

        public A[] Fail(object err) =>
            Empty();

        public A[] Fail(Exception err = null) =>
            Empty();

        public S Fold<S>(A[] fa, S state, Func<S, A, S> f) =>
            fa.Fold(state, f);

        public S FoldBack<S>(A[] fa, S state, Func<S, A, S> f) =>
            fa.FoldBack(state, f);

        public A[] Plus(A[] ma, A[] mb) =>
            PlusSeq(ma, mb).ToArray();

        IEnumerable<A> PlusSeq(A[] ma, A[] mb)
        {
            foreach (var a in ma) yield return a;
            foreach (var b in mb) yield return b;
        }

        public A[] FromSeq(IEnumerable<A> xs) =>
            xs.ToArray();

        public A[] Return(A x) =>
            new[] { x };

        public A[] Zero() =>
            Empty();
    }
}
