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
    public struct MEnumerable<A> :
        Monad<IEnumerable<A>, A>,
        Eq<IEnumerable<A>>,
        Ord<IEnumerable<A>>,
        Monoid<IEnumerable<A>>
    {
        public static readonly MEnumerable<A> Inst = default(MEnumerable<A>);

        [Pure]
        public IEnumerable<A> Append(IEnumerable<A> x, IEnumerable<A> y) =>
            x.Concat(y);

        [Pure]
        public MB Bind<MONADB, MB, B>(IEnumerable<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            traverse<MEnumerable<A>, MONADB, IEnumerable<A>, MB, A, B>(ma, f);

        [Pure]
        public MB BindAsync<MONADB, MB, B>(IEnumerable<A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            traverseSyncAsync<MEnumerable<A>, MONADB, IEnumerable<A>, MB, A, B>(ma, f);

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
        public int Compare(IEnumerable<A> x, IEnumerable<A> y)
        {
            var iterA = x.GetEnumerator();
            var iterB = y.GetEnumerator();
            while (true)
            {
                var hasMovedA = iterA.MoveNext();
                var hasMovedB = iterB.MoveNext();

                if (hasMovedA && hasMovedB)
                {
                    var cmp = default(OrdDefault<A>).Compare(iterA.Current, iterB.Current);
                    if (cmp != 0) return cmp;
                }
                else if(hasMovedA)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        [Pure]
        public IEnumerable<A> Fail(object err = null) =>
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
        public IEnumerable<A> Run(Func<Unit, IEnumerable<A>> ma) =>
            ma(unit);

        [Pure]
        public IEnumerable<A> BindReturn(Unit maOutput, IEnumerable<A> mb) =>
            mb;

        [Pure]
        public IEnumerable<A> Return(A x) =>
            Return(_ => x);

        [Pure]
        public MB Apply<MonadB, MB, B>(Func<A, A, B> faab, IEnumerable<A> fa, IEnumerable<A> fb) where MonadB : struct, Monad<Unit, Unit, MB, B> =>
            default(MEnumerable<A>).Bind<MonadB, MB, B>(fa, a =>
            default(MEnumerable<A>).Bind<MonadB, MB, B>(fb, b =>
            default(MonadB).Return(_ => faab(a, b))));

        [Pure]
        public IEnumerable<A> Apply(Func<A, A, A> f, IEnumerable<A> fa, IEnumerable<A> fb) =>
            from a in fa
            from b in fb
            select f(a, b);
    }
}
