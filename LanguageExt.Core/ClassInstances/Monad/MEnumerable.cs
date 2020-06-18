using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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
            x.ConcatFast(y);

        [Pure]
        public MB Bind<MONADB, MB, B>(IEnumerable<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B>
        {
            if (typeof(Func<A, MB>) == typeof(Func<A, IEnumerable<B>>))
            {
                // The casts are not ideal, but it should still work reliably
                return (MB)(object)BindLazy(ma, (Func<A, IEnumerable<B>>)(object)f);
            }
            else if (typeof(Func<A, MB>) == typeof(Func<A, Seq<B>>) && ma is Seq<A> seqA)
            {
                // The casts are not ideal, but it should still work reliably
                return (MB)(object)BindLazy(seqA, (Func<A, Seq<B>>)(object)f);
            }

            var b = default(MONADB).Zero();
            foreach (var a in ma)
            {
                b = default(MONADB).Plus(b, f(a));
            }
            return b;
        }

        static IEnumerable<B> BindLazy<B>(IEnumerable<A> ma, Func<A, IEnumerable<B>> f) =>
            EnumerableOptimal.BindFast(ma, f);

        static Seq<B> BindLazy<B>(Seq<A> ma, Func<A, Seq<B>> f) =>
            Seq(EnumerableOptimal.BindFast(ma, a => f(a).AsEnumerable()));

        [Pure]
        public MB BindAsync<MONADB, MB, B>(IEnumerable<A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B>
        {
            var b = default(MONADB).Zero();
            foreach (var a in ma)
            {
                b = default(MONADB).Plus(b, f(a));
            }
            return b;
        }

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
            default(EqEnumerable<A>).Equals(x, y);

        [Pure]
        public int Compare(IEnumerable<A> x, IEnumerable<A> y)
        {
            using var iterA = x.GetEnumerator();
            using var iterB = y.GetEnumerator();
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
        public IEnumerable<A> Plus(IEnumerable<A> ma, IEnumerable<A> mb) =>
            EnumerableOptimal.ConcatFast(ma, mb);

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
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(IEnumerable<A> x, IEnumerable<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(IEnumerable<A> x) =>
            GetHashCode(x).AsTask();         
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(IEnumerable<A> x, IEnumerable<A> y) =>
            Compare(x, y).AsTask();
    }
}
