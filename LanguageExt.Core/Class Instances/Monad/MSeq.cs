using LanguageExt.ClassInstances;
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
    /// Seq type-class instance
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public struct MSeq<A> :
        Monad<Seq<A>, A>,
        Eq<Seq<A>>,
        Monoid<Seq<A>>
    {
        public static readonly MSeq<A> Inst = default(MSeq<A>);

        [Pure]
        public Seq<A> Append(Seq<A> x, Seq<A> y) =>
            x.Concat(y);

        [Pure]
        public MB Bind<MONADB, MB, B>(Seq<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            traverse<MSeq<A>, MONADB, Seq<A>, MB, A, B>(ma, f);

        [Pure]
        public MB BindAsync<MONADB, MB, B>(Seq<A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            traverseSyncAsync<MSeq<A>, MONADB, Seq<A>, MB, A, B>(ma, f);

        [Pure]
        public Func<Unit, int> Count(Seq<A> fa) => _ =>
            fa.Count;

        [Pure]
        public Seq<A> Subtract(Seq<A> x, Seq<A> y) =>
            toSeq(Enumerable.Except(x, y));

        [Pure]
        public Seq<A> Empty() =>
            Seq<A>.Empty;

        [Pure]
        public bool Equals(Seq<A> x, Seq<A> y) =>
            default(EqEnumerable<A>).Equals(x, y);

        [Pure]
        public Seq<A> Fail(object err = null) =>
            Empty();

        [Pure]
        public Func<Unit, S> Fold<S>(Seq<A> fa, S state, Func<S, A, S> f) => _ =>
            fa.Fold(state, f);

        [Pure]
        public Func<Unit, S> FoldBack<S>(Seq<A> fa, S state, Func<S, A, S> f) => _ => 
            fa.FoldBack(state, f);

        [Pure]
        public Seq<A> Plus(Seq<A> ma, Seq<A> mb)
        {
            IEnumerable<A> Yield()
            {
                foreach (var a in ma) yield return a;
                foreach (var b in mb) yield return b;
            }
            return toSeq(Yield());
        }

        [Pure]
        public Seq<A> Zero() =>
            Empty();

        [Pure]
        public Seq<A> Return(Func<Unit, A> f) =>
            f(unit).Cons();

        [Pure]
        public int GetHashCode(Seq<A> x) =>
            hash(x);

        [Pure]
        public Seq<A> Run(Func<Unit, Seq<A>> ma) =>
            ma(unit);

        [Pure]
        public Seq<A> BindReturn(Unit maOutput, Seq<A> mb) =>
            mb;

        [Pure]
        public Seq<A> Return(A x) =>
            x.Cons();

        [Pure]
        public Seq<A> Apply(Func<A, A, A> f, Seq<A> fa, Seq<A> fb) =>
            from a in fa
            from b in fb
            select f(a, b);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(Seq<A> x, Seq<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Seq<A> x) =>
            GetHashCode(x).AsTask();         
    }
}
