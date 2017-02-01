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
    /// Set type-class instance
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public struct MSet<A> :
        MonadPlus<Set<A>, A>,
        Foldable<Set<A>, A>,
        Eq<Set<A>>,
        Monoid<Set<A>>
   {
        public static readonly MSet<A> Inst = default(MSet<A>);

        [Pure]
        public Set<A> Append(Set<A> x, Set<A> y) =>
            Set.createRange(x.Concat(y));

        [Pure]
        IEnumerable<B> BindSeq<MONADB, MB, B>(Set<A> ma, Func<A, MB> f)
            where MONADB : struct, Monad<MB, B>
        {
            foreach(var a in ma)
                foreach (var b in toSeq<MONADB, MB, B>(f(a)))
                    yield return b;
        }

        [Pure]
        public MB Bind<MONADB, MB, B>(Set<A> ma, Func<A, MB> f) where MONADB : struct, Monad<MB, B> =>
            default(MONADB).FromSeq(BindSeq<MONADB, MB, B>(ma, f));

        [Pure]
        public int Count(Set<A> fa) =>
            fa.Count();

        [Pure]
        public Set<A> Subtract(Set<A> x, Set<A> y) =>
            Set.createRange(Enumerable.Except(x, y));

        [Pure]
        public Set<A> Empty() =>
            Set.empty<A>();

        [Pure]
        public bool Equals(Set<A> x, Set<A> y) =>
            x == y;

        [Pure]
        public Set<A> Fail(object err) =>
            Empty();

        [Pure]
        public Set<A> Fail(Exception err = null) =>
            Empty();

        [Pure]
        public S Fold<S>(Set<A> fa, S state, Func<S, A, S> f) =>
            fa.Fold(state, f);

        [Pure]
        public S FoldBack<S>(Set<A> fa, S state, Func<S, A, S> f) =>
            fa.FoldBack(state, f);

        [Pure]
        public Set<A> Plus(Set<A> ma, Set<A> mb) =>
            ma + mb;

        [Pure]
        public Set<A> FromSeq(IEnumerable<A> xs) =>
            Set.createRange(xs);

        [Pure]
        public Set<A> Return(A x) =>
            Set(x);

        [Pure]
        public Set<A> Return(Func<A> f) =>
            Return(f());

        [Pure]
        public Set<A> Zero() =>
            Empty();
    }
}
