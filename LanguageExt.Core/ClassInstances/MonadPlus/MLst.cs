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
    public struct MLst<A> :
        MonadPlus<Lst<A>, A>,
        Foldable<Lst<A>, A>,
        Eq<Lst<A>>,
        Monoid<Lst<A>>
   {
        public static readonly MLst<A> Inst = default(MLst<A>);

        public Lst<A> Append(Lst<A> x, Lst<A> y) =>
            x.Concat(y).Freeze();

        IEnumerable<B> BindSeq<MONADB, MB, B>(Lst<A> ma, Func<A, MB> f)
            where MONADB : struct, Monad<MB, B>
        {
            foreach(var a in ma)
                foreach (var b in toSeq<MONADB, MB, B>(f(a)))
                    yield return b;
        }

        public MB Bind<MONADB, MB, B>(Lst<A> ma, Func<A, MB> f) where MONADB : struct, Monad<MB, B> =>
            default(MONADB).Return(BindSeq<MONADB, MB, B>(ma, f));

        public int Count(Lst<A> fa) =>
            fa.Count();

        public Lst<A> Subtract(Lst<A> x, Lst<A> y) =>
            Enumerable.Except(x, y).Freeze();

        public Lst<A> Empty() =>
            List.empty<A>();

        public bool Equals(Lst<A> x, Lst<A> y) =>
            Enumerable.SequenceEqual(x, y);

        public Lst<A> Fail(object err) =>
            Empty();

        public Lst<A> Fail(Exception err = null) =>
            Empty();

        public S Fold<S>(Lst<A> fa, S state, Func<S, A, S> f) =>
            fa.Fold(state, f);

        public S FoldBack<S>(Lst<A> fa, S state, Func<S, A, S> f) =>
            fa.FoldBack(state, f);

        public Lst<A> Plus(Lst<A> ma, Lst<A> mb) =>
            PlusSeq(ma, mb).Freeze();

        IEnumerable<A> PlusSeq(Lst<A> ma, Lst<A> mb)
        {
            foreach (var a in ma) yield return a;
            foreach (var b in mb) yield return b;
        }

        public Lst<A> Return(IEnumerable<A> xs) =>
            xs.Freeze();

        public Lst<A> Return(A x, params A[] xs) =>
            x.Cons(xs).Freeze();

        public Lst<A> Zero() =>
            Empty();
    }
}
