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
    /// Enumerable type-class instance
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public struct MSeq<A> :
        MonadPlus<IEnumerable<A>, A>,
        Foldable<IEnumerable<A>, A>,
        Eq<IEnumerable<A>>,
        Monoid<IEnumerable<A>>
    {
        public static readonly MSeq<A> Inst = default(MSeq<A>);

        public IEnumerable<A> Append(IEnumerable<A> x, IEnumerable<A> y) =>
            x.Concat(y);

        IEnumerable<B> BindSeq<MONADB, MB, B>(IEnumerable<A> ma, Func<A, MB> f) where MONADB : struct, Monad<MB, B>
        {
            foreach (var a in ma)
                foreach (var b in toSeq<MONADB, MB, B>(f(a)))
                    yield return b;
        }

        public MB Bind<MONADB, MB, B>(IEnumerable<A> ma, Func<A, MB> f) where MONADB : struct, Monad<MB, B> =>
            default(MONADB).Return(BindSeq<MONADB, MB, B>(ma, f));

        public int Count(IEnumerable<A> fa) =>
            fa.Count();

        public IEnumerable<A> Subtract(IEnumerable<A> x, IEnumerable<A> y) =>
            Enumerable.Except(x, y);

        public IEnumerable<A> Empty() =>
            new A[0];

        public bool Equals(IEnumerable<A> x, IEnumerable<A> y) =>
            Enumerable.SequenceEqual(x, y);

        public IEnumerable<A> Fail(object err) =>
            Empty();

        public IEnumerable<A> Fail(Exception err = null) =>
            Empty();

        public S Fold<S>(IEnumerable<A> fa, S state, Func<S, A, S> f) =>
            fa.Fold(state, f);

        public S FoldBack<S>(IEnumerable<A> fa, S state, Func<S, A, S> f) =>
            fa.FoldBack(state, f);

        public IEnumerable<A> Plus(IEnumerable<A> ma, IEnumerable<A> mb)
        {
            foreach (var a in ma) yield return a;
            foreach (var b in mb) yield return b;
        }

        public IEnumerable<A> Return(IEnumerable<A> xs) =>
            xs;

        public IEnumerable<A> Return(A x, params A[] xs) =>
            x.Cons(xs);

        public IEnumerable<A> Zero() =>
            Empty();
    }
}
