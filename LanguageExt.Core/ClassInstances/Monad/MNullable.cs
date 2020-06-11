using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct MNullable<A> :
        Optional<A?, A>,
        OptionalUnsafe<A?, A>,
        Monad<A?, A>,
        BiFoldable<A?, A, Unit>,
        Eq<A?>,
        Ord<A?>
        where A : struct
    {
        public static readonly MNullable<A> Inst = default(MNullable<A>);

        [Pure]
        public A? None => (A?)null;

        [Pure]
        public MB Bind<MONADB, MB, B>(A? ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            ma.HasValue && f != null
                ? f(ma.Value)
                : default(MONADB).Fail(ValueIsNoneException.Default);

        [Pure]
        public MB BindAsync<MONADB, MB, B>(A? ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            ma.HasValue && f != null
                ? f(ma.Value)
                : default(MONADB).Fail(ValueIsNoneException.Default);

        [Pure]
        public A? Fail(object err = null) =>
            null;

        [Pure]
        public A? Plus(A? a, A? b) =>
            a.HasValue
                ? a
                : b;

        [Pure]
        public A? Return(Func<Unit, A> f)
        {
            var x = f(unit);
            return isnull(x)
                ? null
                : (A?)f(unit);
        }

        [Pure]
        public A? Zero() =>
            null;

        [Pure]
        public bool IsNone(A? ma) =>
            !ma.HasValue;

        [Pure]
        public bool IsSome(A? ma) =>
            ma.HasValue;

        [Pure]
        public Unit Match(A? ma, Action<A> Some, Action None)
        {
            if (ma.HasValue) Some(ma.Value); else None();
            return unit;
        }

        [Pure]
        public B Match<B>(A? ma, Func<A, B> Some, Func<B> None) =>
            ma.HasValue
                ? Some(ma.Value)
                : None();

        [Pure]
        public B Match<B>(A? ma, Func<A, B> Some, B None) =>
            Check.NullReturn(ma.HasValue
                ? Some(ma.Value)
                : None);

        [Pure]
        public B MatchUnsafe<B>(A? ma, Func<A, B> Some, Func<B> None) =>
            ma.HasValue
                ? Some(ma.Value)
                : None();

        [Pure]
        public B MatchUnsafe<B>(A? ma, Func<A, B> Some, B None) =>
            ma.HasValue
                ? Some(ma.Value)
                : None;

        [Pure]
        public Func<Unit, S> Fold<S>(A? ma, S state, Func<S, A, S> f) => _ =>
            Check.NullReturn(ma.HasValue
                ? f(state, ma.Value)
                : state);

        [Pure]
        public Func<Unit, S> FoldBack<S>(A? ma, S state, Func<S, A, S> f) => _ =>
            Check.NullReturn(ma.HasValue
                ? f(state, ma.Value)
                : state);

        [Pure]
        public S BiFold<S>(A? ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
            Check.NullReturn(ma.HasValue
                ? fa(state, ma.Value)
                : fb(state, unit));

        [Pure]
        public S BiFoldBack<S>(A? ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
            Check.NullReturn(ma.HasValue
                ? fa(state, ma.Value)
                : fb(state, unit));

        [Pure]
        public Func<Unit, int> Count(A? ma) => _ =>
            ma.HasValue
                ? 1
                : 0;

        [Pure]
        public A? Some(A value) =>
            value;

        [Pure]
        public A? Optional(A value) =>
            value;

        [Pure]
        public A? Run(Func<Unit, A?> ma) =>
            ma(unit);

        [Pure]
        public A? BindReturn(Unit _, A? mb) =>
            mb;

        [Pure]
        public A? Return(A x) =>
            Return(_ => x);

        [Pure]
        public A? Apply(Func<A, A, A> f, A? fa, A? fb) =>
            from a in fa
            from b in fb
            select f(a, b);

        [Pure]
        public int Compare(A? x, A? y) =>
            x.HasValue && y.HasValue ? default(OrdDefault<A>).Compare(x.Value, y.Value)
          : x.HasValue               ? 1
          : y.HasValue               ? -1
          : 0;

        [Pure]
        public bool Equals(A? x, A? y) =>
            x.HasValue && y.HasValue ? default(EqDefault<A>).Equals(x.Value, y.Value)
          : x.HasValue || y.HasValue ? false
          : true;

        [Pure]
        public int GetHashCode(A? x) =>
            x.GetHashCode();
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(A? x, A? y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(A? x) =>
            GetHashCode(x).AsTask();         
            
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(A? x, A? y) =>
            Compare(x, y).AsTask();      
    }
}
