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
    public struct MSeq<A> :
        Monad<IEnumerable<A>, A>,
        Eq<IEnumerable<A>>,
        Monoid<IEnumerable<A>>
    {
        public static readonly MSeq<A> Inst = default(MSeq<A>);

        [Pure]
        public IEnumerable<A> Append(IEnumerable<A> x, IEnumerable<A> y) =>
            x.Concat(y);

        [Pure]
        public MB Bind<MONADB, MB, B>(IEnumerable<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            traverse<MSeq<A>, MONADB, IEnumerable<A>, MB, A, B>(ma, f);

        [Pure]
        public Func<Unit, int> Count(IEnumerable<A> fa) => _ =>
            fa.Count();

        [Pure]
        public IEnumerable<A> Subtract(IEnumerable<A> x, IEnumerable<A> y) =>
            Enumerable.Except(x, y);

        [Pure]
        public IEnumerable<A> Empty() =>
            new A[0];

        [Pure]
        public bool Equals(IEnumerable<A> x, IEnumerable<A> y) =>
            Enumerable.SequenceEqual(x, y);

        [Pure]
        public IEnumerable<A> Fail(object err) =>
            Empty();

        [Pure]
        public IEnumerable<A> Fail(Exception err = null) =>
            Empty();

        [Pure]
        public Func<Unit, S> Fold<S>(IEnumerable<A> fa, S state, Func<S, A, S> f) => _ =>
            fa.Fold(state, f);

        [Pure]
        public Func<Unit, S> FoldBack<S>(IEnumerable<A> fa, S state, Func<S, A, S> f) => _ => 
            fa.FoldBack(state, f);

        [Pure]
        public IEnumerable<A> Plus(IEnumerable<A> ma, IEnumerable<A> mb)
        {
            foreach (var a in ma) yield return a;
            foreach (var b in mb) yield return b;
        }

        [Pure]
        public IEnumerable<A> Zero() =>
            Empty();

        [Pure]
        public IEnumerable<A> Return(Func<Unit, A> f) =>
            new[] { f(unit) };

        [Pure]
        public int GetHashCode(IEnumerable<A> x) =>
            hash(x);

        [Pure]
        public IEnumerable<A> Id(Func<Unit, IEnumerable<A>> ma) =>
            ma(unit);

        [Pure]
        public IEnumerable<A> BindOutput(Unit maOutput, IEnumerable<A> mb) =>
            mb;

        [Pure]
        public IEnumerable<A> Return(A x) =>
            Return(_ => x);
    }
}
