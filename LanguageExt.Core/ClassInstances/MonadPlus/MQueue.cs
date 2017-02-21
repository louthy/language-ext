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
    /// MQueue type-class instance
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    public struct MQueue<A> :
        MonadPlus<Que<A>, A>,
        Foldable<Que<A>, A>,
        Eq<Que<A>>,
        Monoid<Que<A>>
   {
        public static readonly MQueue<A> Inst = default(MQueue<A>);

        [Pure]
        public Que<A> Append(Que<A> x, Que<A> y) =>
            x + y;

        [Pure]
        IEnumerable<B> BindSeq<MONADB, MB, B>(Que<A> ma, Func<A, MB> f)
            where MONADB : struct, Monad<MB, B>
        {
            foreach(var a in ma)
                foreach (var b in toSeq<MONADB, MB, B>(f(a)))
                    yield return b;
        }

        [Pure]
        public MB Bind<MONADB, MB, B>(Que<A> ma, Func<A, MB> f) where MONADB : struct, Monad<MB, B> =>
            default(MONADB).FromSeq(BindSeq<MONADB, MB, B>(ma, f));

        [Pure]
        public int Count(Que<A> fa) =>
            fa.Count();

        [Pure]
        public Que<A> Subtract(Que<A> x, Que<A> y) =>
            x - y;

        [Pure]
        public Que<A> Empty() =>
            Que<A>.Empty;

        [Pure]
        public bool Equals(Que<A> x, Que<A> y) =>
            x == y;

        [Pure]
        public Que<A> Fail(object err) =>
            Que<A>.Empty;

        [Pure]
        public Que<A> Fail(Exception err = null) =>
            Que<A>.Empty;

        [Pure]
        public S Fold<S>(Que<A> fa, S state, Func<S, A, S> f) =>
            fa.Fold(state, f);

        [Pure]
        public S FoldBack<S>(Que<A> fa, S state, Func<S, A, S> f) =>
            fa.FoldBack(state, f);

        [Pure]
        public Que<A> Plus(Que<A> ma, Que<A> mb) =>
            ma + mb;

        [Pure]
        public Que<A> FromSeq(IEnumerable<A> xs) =>
            new Que<A>(xs);

        [Pure]
        public Que<A> Return(A x) =>
            Queue(x);

        [Pure]
        public Que<A> Return(Func<A> f) =>
            Return(f());

        [Pure]
        public Que<A> Zero() =>
            Que<A>.Empty;

        [Pure]
        public int GetHashCode(Que<A> x) =>
            x.GetHashCode();
    }
}
