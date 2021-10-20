using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using System;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// MLst type-class instance
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    public struct MLst<A> :
        Monad<Lst<A>, A>,
        Eq<Lst<A>>,
        Ord<Lst<A>>,
        Monoid<Lst<A>>
   {
        public static readonly MLst<A> Inst = default(MLst<A>);

        [Pure]
        public Lst<A> Append(Lst<A> x, Lst<A> y) =>
            x.ConcatFast(y).Freeze();

        [Pure]
        public MB Bind<MONADB, MB, B>(Lst<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            traverse<MLst<A>, MONADB, Lst<A>, MB, A, B>(ma, f);

        [Pure]
        public MB BindAsync<MONADB, MB, B>(Lst<A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            traverseSyncAsync<MLst<A>, MONADB, Lst<A>, MB, A, B>(ma, f);

        [Pure]
        public Func<Unit, int> Count(Lst<A> fa) => _ =>
            fa.Count();

        [Pure]
        public Lst<A> Subtract(Lst<A> x, Lst<A> y) =>
            Enumerable.Except(x, y).Freeze();

        [Pure]
        public Lst<A> Empty() =>
            List.empty<A>();

        [Pure]
        public bool Equals(Lst<A> x, Lst<A> y) =>
            default(EqEnumerable<A>).Equals(x, y);

        [Pure]
        public int Compare(Lst<A> x, Lst<A> y)
        {
            int cmp = x.Count.CompareTo(y.Count);
            if (cmp != 0) return cmp;

            using var iterA = x.GetEnumerator();
            using var iterB = y.GetEnumerator();
            while (iterA.MoveNext() && iterB.MoveNext())
            {
                cmp = default(OrdDefault<A>).Compare(iterA.Current, iterB.Current);
                if (cmp != 0) return cmp;
            }
            return 0;
        }

        [Pure]
        public Lst<A> Fail(object err = null) =>
            Empty();

        [Pure]
        public Func<Unit, S> Fold<S>(Lst<A> fa, S state, Func<S, A, S> f) => _ =>
             fa.Fold(state, f);

        [Pure]
        public Func<Unit, S> FoldBack<S>(Lst<A> fa, S state, Func<S, A, S> f) => _ =>
             fa.FoldBack(state, f);

        [Pure]
        public Lst<A> Plus(Lst<A> ma, Lst<A> mb) =>
            ma + mb;

        [Pure]
        public Lst<A> Return(Func<Unit, A> f) =>
            List(f(unit));

        [Pure]
        public Lst<A> Zero() =>
            Empty();

        [Pure]
        public int GetHashCode(Lst<A> x) =>
            x.GetHashCode();

        [Pure]
        public Lst<A> Run(Func<Unit, Lst<A>> ma) =>
            ma(unit);

        [Pure]
        public Lst<A> BindReturn(Unit maOutput, Lst<A> mb) =>
            mb;

        [Pure]
        public Lst<A> Return(A x) =>
            Return(_ => x);

        [Pure]
        public Lst<A> Apply(Func<A, A, A> f, Lst<A> fa, Lst<A> fb) =>
            from a in fa
            from b in fb
            select f(a, b);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(Lst<A> x, Lst<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Lst<A> x) =>
            GetHashCode(x).AsTask();         
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(Lst<A> x, Lst<A> y) =>
            Compare(x, y).AsTask();
    }
}
