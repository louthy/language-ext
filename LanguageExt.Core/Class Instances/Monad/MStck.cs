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
    /// MStack type-class instance
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    public struct MStck<A> :
        Monad<Stck<A>, A>,
        Eq<Stck<A>>,
        Monoid<Stck<A>>
   {
        public static readonly MStck<A> Inst = default(MStck<A>);

        [Pure]
        public Stck<A> Append(Stck<A> x, Stck<A> y) =>
            new Stck<A>(x.ConcatFast(y));

        [Pure]
        public MB Bind<MONADB, MB, B>(Stck<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            traverse<MStck<A>, MONADB, Stck<A>, MB, A, B>(ma, f);

        [Pure]
        public MB BindAsync<MONADB, MB, B>(Stck<A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            traverseSyncAsync<MStck<A>, MONADB, Stck<A>, MB, A, B>(ma, f);

        [Pure]
        public Func<Unit, int> Count(Stck<A> fa) => _ => 
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
        public Stck<A> Fail(object err = null) =>
            Stck<A>.Empty;

        [Pure]
        public Func<Unit, S> Fold<S>(Stck<A> fa, S state, Func<S, A, S> f) => _ =>
            fa.Fold(state, f);

        [Pure]
        public Func<Unit, S> FoldBack<S>(Stck<A> fa, S state, Func<S, A, S> f) => _ =>
            fa.FoldBack(state, f);

        [Pure]
        public Stck<A> Plus(Stck<A> ma, Stck<A> mb) =>
            ma + mb;

        [Pure]
        public Stck<A> Return(Func<Unit, A> f) =>
            Stack(f(unit));

        [Pure]
        public Stck<A> Zero() =>
            Stck<A>.Empty;

        [Pure]
        public int GetHashCode(Stck<A> x) =>
            x.GetHashCode();

        [Pure]
        public Stck<A> Run(Func<Unit, Stck<A>> ma) =>
            ma(unit);

        [Pure]
        public Stck<A> BindReturn(Unit _, Stck<A> mb) =>
            mb;

        [Pure]
        public Stck<A> Return(A x) =>
            Stack(x);

        [Pure]
        public Stck<A> Apply(Func<A, A, A> f, Stck<A> fa, Stck<A> fb) =>
            toStack(
                from a in fa
                from b in fb
                select f(a, b));
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(Stck<A> x, Stck<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Stck<A> x) =>
            GetHashCode(x).AsTask();       
    }
}
