using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{
    public struct MFin<A> :
        Alternative<Fin<A>, Error, A>,
        Monad<Fin<A>, A>,
        BiFoldable<Fin<A>, A, Error>,
        Eq<Fin<A>>,
        Ord<Fin<A>>
    {
        public static readonly MFin<A> Inst = default(MFin<A>);

        [Pure]
        public Fin<A> None => 
            default;
 
        [Pure]
        public MB Bind<MonadB, MB, B>(Fin<A> ma, Func<A, MB> f) where MonadB : struct, Monad<Unit, Unit, MB, B> =>
            ma.IsSucc
                ? (f ?? throw new ArgumentNullException(nameof(f)))(ma.Value)
                : default(MonadB).Fail(ma.Error);
 
        [Pure]
        public MB BindAsync<MonadB, MB, B>(Fin<A> ma, Func<A, MB> f) where MonadB : struct, MonadAsync<Unit, Unit, MB, B> =>
            ma.IsSucc
                ? (f ?? throw new ArgumentNullException(nameof(f)))(ma.Value)
                : default(MonadB).Fail(ma.Error);

        [Pure]
        public Fin<A> Fail(object err = null) =>
            Common.Error.FromObject(err);

        [Pure]
        public Fin<A> Plus(Fin<A> a, Fin<A> b) =>
            a.IsSucc ? a : b;

        [Pure]
        public Fin<A> Return(Func<Unit, A> f) =>
            Fin<A>.Succ(f(unit));

        [Pure]
        public Fin<A> Zero() =>
            default;

        [Pure]
        public Func<Unit, S> Fold<S>(Fin<A> ma, S state, Func<S, A, S> f) => _ =>
            ma.Fold(state, f);

        [Pure]
        public Func<Unit, S> FoldBack<S>(Fin<A> ma, S state, Func<S, A, S> f) => _ =>
            ma.Fold(state, f);

        [Pure]
        public S BiFold<S>(Fin<A> ma, S state, Func<S, A, S> fa, Func<S, Error, S> fb) =>
            ma.BiFold(state, fa, fb);

        [Pure]
        public S BiFoldBack<S>(Fin<A> ma, S state, Func<S, A, S> fa, Func<S, Error, S> fb) =>
            ma.BiFold(state, fa, fb);

        [Pure]
        public Func<Unit, int> Count(Fin<A> ma) => _ =>
        ma.IsSucc 
            ? 1 
            : 0;

        [Pure]
        public Fin<A> Run(Func<Unit, Fin<A>> ma) =>
            ma(unit);

        [Pure]
        public Fin<A> BindReturn(Unit _, Fin<A> mb) =>
            mb;

        [Pure]
        public Fin<A> Return(A x) =>
            x;

        [Pure]
        public Fin<A> Empty() =>
            default;

        [Pure]
        public Fin<A> Append(Fin<A> x, Fin<A> y) =>
            Plus(x, y);

        [Pure]
        public bool Equals(Fin<A> x, Fin<A> y) =>
            x.Equals(y);

        [Pure]
        public int GetHashCode(Fin<A> x) =>
            x.GetHashCode();

        [Pure]
        public Fin<A> Apply(Func<A, A, A> f, Fin<A> fa, Fin<A> fb) =>
            fa.IsSucc
                ? fb.IsSucc
                    ? FinSucc(f(fa.Value, fb.Value))
                    : fb
                : fa;
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(Fin<A> x, Fin<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Fin<A> x) =>
            GetHashCode(x).AsTask();         
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(Fin<A> x, Fin<A> y) =>
            x.CompareTo(y);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(Fin<A> x, Fin<A> y) =>
            Compare(x, y).AsTask();
    }
}
