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

        [Pure]
        public Lst<A> Append(Lst<A> x, Lst<A> y) =>
            x.Concat(y).Freeze();

        [Pure]
        IEnumerable<B> BindSeq<MONADB, MB, B>(Lst<A> ma, Func<A, MB> f)
            where MONADB : struct, Monad<MB, B>
        {
            foreach(var a in ma)
                foreach (var b in toSeq<MONADB, MB, B>(f(a)))
                    yield return b;
        }

        [Pure]
        public MB Bind<MONADB, MB, B>(Lst<A> ma, Func<A, MB> f) where MONADB : struct, Monad<MB, B> =>
            default(MONADB).FromSeq(BindSeq<MONADB, MB, B>(ma, f));

        [Pure]
        public int Count(Lst<A> fa) =>
            fa.Count();

        [Pure]
        public Lst<A> Subtract(Lst<A> x, Lst<A> y) =>
            Enumerable.Except(x, y).Freeze();

        [Pure]
        public Lst<A> Empty() =>
            List.empty<A>();

        [Pure]
        public bool Equals(Lst<A> x, Lst<A> y) =>
            Enumerable.SequenceEqual(x, y);

        [Pure]
        public Lst<A> Fail(object err) =>
            Empty();

        [Pure]
        public Lst<A> Fail(Exception err = null) =>
            Empty();

        [Pure]
        public S Fold<S>(Lst<A> fa, S state, Func<S, A, S> f) =>
            fa.Fold(state, f);

        [Pure]
        public S FoldBack<S>(Lst<A> fa, S state, Func<S, A, S> f) =>
            fa.FoldBack(state, f);

        [Pure]
        public Lst<A> Plus(Lst<A> ma, Lst<A> mb) =>
            PlusSeq(ma, mb).Freeze();

        [Pure]
        IEnumerable<A> PlusSeq(Lst<A> ma, Lst<A> mb)
        {
            foreach (var a in ma) yield return a;
            foreach (var b in mb) yield return b;
        }

        [Pure]
        public Lst<A> FromSeq(IEnumerable<A> xs) =>
            xs.Freeze();

        [Pure]
        public Lst<A> Return(A x) =>
            List(x);

        [Pure]
        public Lst<A> Return(Func<A> f) =>
            Return(f());

        [Pure]
        public Lst<A> Zero() =>
            Empty();
    }
}
