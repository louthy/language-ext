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
    /// MStack type-class instance
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    public struct MStck<A> :
        Monad<Stck<A>, A>,
        Foldable<Stck<A>, A>,
        Eq<Stck<A>>,
        Monoid<Stck<A>>
   {
        public static readonly MStck<A> Inst = default(MStck<A>);

        [Pure]
        public Stck<A> Append(Stck<A> x, Stck<A> y) =>
            new Stck<A>(x.Concat(y));

        [Pure]
        public MB Bind<MONADB, MB, B>(Stck<A> ma, Func<A, MB> f) where MONADB : struct, Monad<MB, B> =>
            traverse<MStck<A>, MONADB, Stck<A>, MB, A, B>(ma, f);

        [Pure]
        public int Count(Stck<A> fa) =>
            fa.Count();

        [Pure]
        public Stck<A> Subtract(Stck<A> x, Stck<A> y) =>
            x - y;

        [Pure]
        public Stck<A> Empty() =>
            Stck<A>.Empty;

        [Pure]
        public bool Equals(Stck<A> x, Stck<A> y) =>
            x == y;

        [Pure]
        public Stck<A> Fail(object err) =>
            Stck<A>.Empty;

        [Pure]
        public Stck<A> Fail(Exception err = null) =>
            Stck<A>.Empty;

        [Pure]
        public S Fold<S>(Stck<A> fa, S state, Func<S, A, S> f) =>
            fa.Fold(state, f);

        [Pure]
        public S FoldBack<S>(Stck<A> fa, S state, Func<S, A, S> f) =>
            fa.FoldBack(state, f);

        [Pure]
        public Stck<A> Plus(Stck<A> ma, Stck<A> mb) =>
            ma + mb;

        [Pure]
        public Stck<A> Return(A x) =>
            Stack(x);

        [Pure]
        public Stck<A> Zero() =>
            Stck<A>.Empty;

        [Pure]
        public int GetHashCode(Stck<A> x) =>
            x.GetHashCode();
    }
}
