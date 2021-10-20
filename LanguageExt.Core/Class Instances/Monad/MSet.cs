using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using System;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Set type-class instance
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public struct MSet<A> :
        Monad<Set<A>, A>,
        Eq<Set<A>>,
        Monoid<Set<A>>
   {
        public static readonly MSet<A> Inst = default(MSet<A>);

        [Pure]
        public Set<A> Append(Set<A> x, Set<A> y) =>
            Set.createRange(x.ConcatFast(y));

        [Pure]
        public MB Bind<MONADB, MB, B>(Set<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            traverse<MSet<A>, MONADB, Set<A>, MB, A, B>(ma, f);

        [Pure]
        public MB BindAsync<MONADB, MB, B>(Set<A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            traverseSyncAsync<MSet<A>, MONADB, Set<A>, MB, A, B>(ma, f);

        [Pure]
        public Func<Unit, int> Count(Set<A> fa) => _ =>
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
        public Set<A> Fail(object err = null) =>
            Empty();

        [Pure]
        public Func<Unit, S> Fold<S>(Set<A> fa, S state, Func<S, A, S> f) => _ =>
            fa.Fold(state, f);

        [Pure]
        public Func<Unit, S> FoldBack<S>(Set<A> fa, S state, Func<S, A, S> f) => _ =>
            fa.FoldBack(state, f);

        [Pure]
        public Set<A> Plus(Set<A> ma, Set<A> mb) =>
            ma + mb;

        [Pure]
        public Set<A> Return(Func<Unit, A> f) =>
            Set(f(unit));

        [Pure]
        public Set<A> Zero() =>
            Empty();

        [Pure]
        public int GetHashCode(Set<A> x) =>
            x.GetHashCode();

        [Pure]
        public Set<A> Return(A x) =>
            Set(x);

        [Pure]
        public Set<A> Run(Func<Unit, Set<A>> ma) =>
            ma(unit);

        [Pure]
        public Set<A> BindReturn(Unit _, Set<A> mb) =>
            mb;

        [Pure]
        public Set<A> Apply(Func<A, A, A> f, Set<A> fa, Set<A> fb) =>
            from a in fa
            from b in fb
            select f(a, b);
 
        [Pure]
        public Task<bool> EqualsAsync(Set<A> x, Set<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        public Task<int> GetHashCodeAsync(Set<A> x) =>
            GetHashCode(x).AsTask();    
    }
}
