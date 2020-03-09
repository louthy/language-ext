using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct MOptionUnsafe<A> :
        Alternative<OptionUnsafe<A>, Unit, A>,
        Monad<OptionUnsafe<A>, A>,
        OptionalUnsafe<OptionUnsafe<A>, A>,
        Foldable<OptionUnsafe<A>, A>,
        BiFoldable<OptionUnsafe<A>, A, Unit>,
        Eq<OptionUnsafe<A>>
    {
        public static readonly MOptionUnsafe<A> Inst = default(MOptionUnsafe<A>);

        [Pure]
        public OptionUnsafe<A> None => OptionUnsafe<A>.None;

        [Pure]
        public MB Bind<MONADB, MB, B>(OptionUnsafe<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            ma.IsSome
                ? (f ?? throw new ArgumentNullException(nameof(f)))(ma.Value)
                : default(MONADB).Fail(ValueIsNoneException.Default);

        [Pure]
        public MB BindAsync<MONADB, MB, B>(OptionUnsafe<A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            ma.IsSome 
                ? (f ?? throw new ArgumentNullException(nameof(f)))(ma.Value)
                : default(MONADB).Fail(ValueIsNoneException.Default);

        [Pure]
        public OptionUnsafe<A> Fail(object err = null) =>
            OptionUnsafe<A>.None;

        [Pure]
        public OptionUnsafe<A> Plus(OptionUnsafe<A> a, OptionUnsafe<A> b) =>
            a.IsSome
                ? a
                : b;

        [Pure]
        public OptionUnsafe<A> Return(Func<Unit, A> f) =>
            OptionUnsafe<A>.Some(f(unit));

        [Pure]
        public OptionUnsafe<A> Zero() =>
            default;

        [Pure]
        public bool IsNone(OptionUnsafe<A> opt) =>
            opt.IsNone;

        [Pure]
        public bool IsSome(OptionUnsafe<A> opt) =>
            opt.IsSome;

        [Pure]
        public B MatchUnsafe<B>(OptionUnsafe<A> opt, Func<A, B> Some, Func<B> None) =>
            opt.MatchUnsafe(Some, None);

        [Pure]
        public B MatchUnsafe<B>(OptionUnsafe<A> opt, Func<A, B> Some, B None) =>
            opt.MatchUnsafe(Some, None);

        [Pure]
        public Unit Match(OptionUnsafe<A> opt, Action<A> Some, Action None) =>
            opt.MatchUnsafe(Some, None);

        [Pure]
        public Func<Unit, S> Fold<S>(OptionUnsafe<A> ma, S state, Func<S, A, S> f) => _ =>
            ma.Fold(state, f);

        [Pure]
        public Func<Unit, S> FoldBack<S>(OptionUnsafe<A> ma, S state, Func<S, A, S> f) => _ =>
            ma.FoldBack(state, f);

        [Pure]
        public S BiFold<S>(OptionUnsafe<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
            ma.BiFold(state, fa, fb);

        [Pure]
        public S BiFoldBack<S>(OptionUnsafe<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
            ma.BiFold(state, fa, fb);

        [Pure]
        public Func<Unit, int> Count(OptionUnsafe<A> ma) => _ =>
            ma.IsSome
                ? 1
                : 0;

        [Pure]
        public OptionUnsafe<A> Some(A x) =>
            OptionUnsafe<A>.Some(x);

        [Pure]
        public OptionUnsafe<A> Optional(A x) =>
            OptionUnsafe<A>.Some(x);

        [Pure]
        public OptionUnsafe<A> Run(Func<Unit, OptionUnsafe<A>> ma) =>
            ma(unit);

        [Pure]
        public OptionUnsafe<A> BindReturn(Unit _, OptionUnsafe<A> mb) =>
            mb;

        [Pure]
        public OptionUnsafe<A> Return(A x) =>
            OptionUnsafe<A>.Some(x);

        [Pure]
        public OptionUnsafe<A> Empty() =>
            default;

        [Pure]
        public OptionUnsafe<A> Append(OptionUnsafe<A> x, OptionUnsafe<A> y) =>
            Plus(x, y);

        [Pure]
        public bool Equals(OptionUnsafe<A> x, OptionUnsafe<A> y) =>
            x.Equals(y);

        [Pure]
        public int GetHashCode(OptionUnsafe<A> x) =>
            x.GetHashCode();

        [Pure]
        public OptionUnsafe<A> Apply(Func<A, A, A> f, OptionUnsafe<A> fa, OptionUnsafe<A> fb) =>
            fa.IsSome && fb.IsSome
                ? Some(f(fa.Value, fb.Value))
                : default;
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(OptionUnsafe<A> x, OptionUnsafe<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(OptionUnsafe<A> x) =>
            GetHashCode(x).AsTask();         
    }
}
