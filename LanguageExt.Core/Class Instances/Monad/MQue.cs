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
    /// MQue type-class instance
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    public struct MQue<A> :
        Monad<Que<A>, A>,
        Foldable<Que<A>, A>,
        Eq<Que<A>>,
        Monoid<Que<A>>
   {
        public static readonly MQue<A> Inst = default(MQue<A>);

        [Pure]
        public Que<A> Append(Que<A> x, Que<A> y) =>
            x + y;

        [Pure]
        public MB Bind<MONADB, MB, B>(Que<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            ma.Fold(default(MONADB).Zero(), (s, a) => default(MONADB).Plus(s, f(a)));

        [Pure]
        public MB BindAsync<MONADB, MB, B>(Que<A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            ma.Fold(default(MONADB).Zero(), (s, a) => default(MONADB).Plus(s, f(a)));

        [Pure]
        public Func<Unit, int> Count(Que<A> fa) => _ =>
            fa.Count();

        [Pure]
        public Que<A> Subtract(Que<A> x, Que<A> y) =>
            x - y;

        [Pure]
        public Que<A> Empty() =>
            Que<A>.Empty;

        [Pure]
        public bool Equals(Que<A> x, Que<A> y) =>
            x == y;

        [Pure]
        public Que<A> Fail(object err = null) =>
            Que<A>.Empty;

        [Pure]
        public Func<Unit, S> Fold<S>(Que<A> fa, S state, Func<S, A, S> f) => _ =>
            fa.Fold(state, f);

        [Pure]
        public Func<Unit, S> FoldBack<S>(Que<A> fa, S state, Func<S, A, S> f) => _ =>
            fa.FoldBack(state, f);

        [Pure]
        public Que<A> Plus(Que<A> ma, Que<A> mb) =>
            ma + mb;

        [Pure]
        public Que<A> Return(Func<Unit, A> f) =>
            Queue(f(unit));

        [Pure]
        public Que<A> Zero() =>
            Que<A>.Empty;

        [Pure]
        public int GetHashCode(Que<A> x) =>
            x.GetHashCode();

        [Pure]
        public Que<A> Run(Func<Unit, Que<A>> ma) =>
            ma(unit);

        [Pure]
        public Que<A> BindReturn(Unit _, Que<A> mb) =>
            mb;

        [Pure]
        public Que<A> Return(A x) =>
            Queue(x);

        [Pure]
        public Que<A> Apply(Func<A, A, A> f, Que<A> fa, Que<A> fb) =>
            toQueue(
                from a in fa
                from b in fb
                select f(a, b));
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(Que<A> x, Que<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Que<A> x) =>
            GetHashCode(x).AsTask();       
    }
}
