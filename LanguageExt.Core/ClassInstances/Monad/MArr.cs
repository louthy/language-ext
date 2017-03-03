using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;
using LanguageExt;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Array type-class instance
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    public struct MArr<A> :
        Monad<Arr<A>, A>,
        Foldable<Arr<A>, A>,
        Eq<Arr<A>>,
        Monoid<Arr<A>>
   {
        public static readonly MArr<A> Inst = default(MArr<A>);

        [Pure]
        public Arr<A> Append(Arr<A> x, Arr<A> y) =>
            x.Concat(y).ToArray();

        [Pure]
        public MB Bind<MONADB, MB, B>(Arr<A> ma, Func<A, MB> f) where MONADB : struct, Monad<MB, B> =>
            traverse<MArr<A>, MONADB, Arr<A>, MB, A, B>(ma, f);

        [Pure]
        public int Count(Arr<A> fa) =>
            fa.Count();

        [Pure]
        public Arr<A> Empty() =>
            Arr<A>.Empty;

        [Pure]
        public bool Equals(Arr<A> x, Arr<A> y) =>
            Enumerable.SequenceEqual(x, y);

        [Pure]
        public Arr<A> Fail(object err) =>
            Empty();

        [Pure]
        public Arr<A> Fail(Exception err = null) =>
            Empty();

        [Pure]
        public S Fold<S>(Arr<A> fa, S state, Func<S, A, S> f) =>
            fa.Fold(state, f);

        [Pure]
        public S FoldBack<S>(Arr<A> fa, S state, Func<S, A, S> f) =>
            fa.FoldBack(state, f);

        [Pure]
        public Arr<A> Plus(Arr<A> ma, Arr<A> mb) =>
            ma + mb;

        [Pure]
        public Arr<A> Return(A x) =>
            new[] { x };

        [Pure]
        public Arr<A> Zero() =>
            Empty();

        [Pure]
        public int GetHashCode(Arr<A> x) =>
            x.GetHashCode();

        [Pure]
        public FunctorAB ToFunctor<FunctorAB, MB, B>() where FunctorAB : struct, Functor<Arr<A>, MB, A, B> =>
            default(FunctorAB);
    }
}
