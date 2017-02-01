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
    /// Hash set type-class instance
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public struct MHashSet<A> :
        MonadPlus<HashSet<A>, A>,
        Foldable<HashSet<A>, A>,
        Eq<HashSet<A>>,
        Monoid<HashSet<A>>
   {
        public static readonly MHashSet<A> Inst = default(MHashSet<A>);

        [Pure]
        public HashSet<A> Append(HashSet<A> x, HashSet<A> y) =>
            HashSet.createRange(x.Concat(y));

        [Pure]
        IEnumerable<B> BindSeq<MONADB, MB, B>(HashSet<A> ma, Func<A, MB> f)
            where MONADB : struct, Monad<MB, B>
        {
            foreach(var a in ma)
                foreach (var b in toSeq<MONADB, MB, B>(f(a)))
                    yield return b;
        }

        [Pure]
        public MB Bind<MONADB, MB, B>(HashSet<A> ma, Func<A, MB> f) where MONADB : struct, Monad<MB, B> =>
            default(MONADB).FromSeq(BindSeq<MONADB, MB, B>(ma, f));

        [Pure]
        public int Count(HashSet<A> fa) =>
            fa.Count();

        [Pure]
        public HashSet<A> Subtract(HashSet<A> x, HashSet<A> y) =>
            HashSet.createRange(Enumerable.Except(x, y));

        [Pure]
        public HashSet<A> Empty() =>
            HashSet.empty<A>();

        [Pure]
        public bool Equals(HashSet<A> x, HashSet<A> y) =>
            x == y;

        [Pure]
        public HashSet<A> Fail(object err) =>
            Empty();

        [Pure]
        public HashSet<A> Fail(Exception err = null) =>
            Empty();

        [Pure]
        public S Fold<S>(HashSet<A> fa, S state, Func<S, A, S> f) =>
            fa.Fold(state, f);

        [Pure]
        public S FoldBack<S>(HashSet<A> fa, S state, Func<S, A, S> f) =>
            fa.FoldBack(state, f);

        [Pure]
        public HashSet<A> Plus(HashSet<A> ma, HashSet<A> mb) =>
            ma + mb;

        [Pure]
        public HashSet<A> FromSeq(IEnumerable<A> xs) =>
            HashSet.createRange(xs);

        [Pure]
        public HashSet<A> Return(A x) =>
            HashSet(x);

        [Pure]
        public HashSet<A> Return(Func<A> f) =>
            Return(f());

        [Pure]
        public HashSet<A> Zero() =>
            Empty();
    }
}
