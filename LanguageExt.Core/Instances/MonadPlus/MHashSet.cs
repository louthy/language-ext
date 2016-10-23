using LanguageExt.Instances;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Hash set type-class instance
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public struct MHashSet<A> :
        MonadPlus<HashSet<A>, A>,
        Foldable<HashSet<A>, A>,
        Eq<HashSet<A>>,
        Difference<HashSet<A>>,
        Monoid<HashSet<A>>
   {
        public HashSet<A> Append(HashSet<A> x, HashSet<A> y) =>
            HashSet.createRange(x.Concat(y));

        IEnumerable<B> BindSeq<MONADB, MB, B>(HashSet<A> ma, Func<A, MB> f)
            where MONADB : struct, Monad<MB, B>
        {
            foreach(var a in ma)
                foreach (var b in f(a).ToSeq<MONADB, MB, B>())
                    yield return b;
        }

        public MB Bind<MONADB, MB, B>(HashSet<A> ma, Func<A, MB> f) where MONADB : struct, Monad<MB, B> =>
            default(MONADB).Return(BindSeq<MONADB, MB, B>(ma, f));

        public int Count(HashSet<A> fa) =>
            fa.Count();

        public HashSet<A> Difference(HashSet<A> x, HashSet<A> y) =>
            HashSet.createRange(Enumerable.Except(x, y));

        public HashSet<A> Empty() =>
            HashSet.empty<A>();

        public bool Equals(HashSet<A> x, HashSet<A> y) =>
            x == y;

        public HashSet<A> Fail(object err) =>
            Empty();

        public HashSet<A> Fail(Exception err = null) =>
            Empty();

        public S Fold<S>(HashSet<A> fa, S state, Func<S, A, S> f) =>
            fa.Fold(state, f);

        public S FoldBack<S>(HashSet<A> fa, S state, Func<S, A, S> f) =>
            fa.FoldBack(state, f);

        public HashSet<A> Plus(HashSet<A> ma, HashSet<A> mb) =>
            ma + mb;

        public HashSet<A> Return(IEnumerable<A> xs) =>
            HashSet.createRange(xs);

        public HashSet<A> Return(A x, params A[] xs) =>
            HashSet.createRange(x.Cons(xs));

        public HashSet<A> Zero() =>
            Empty();
    }
}
