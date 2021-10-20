using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{
    public struct SemiOption<SemigroupA, A> :
        Alternative<Option<A>, Unit, A>,
        Optional<Option<A>, A>,
        OptionalUnsafe<Option<A>, A>,
        Monad<Option<A>, A>,
        BiFoldable<Option<A>, A, Unit>,
        Eq<Option<A>>,
        Ord<Option<A>>,
        AsyncPair<Option<A>, OptionAsync<A>>,
        Monoid<Option<A>>
        where SemigroupA : struct, Semigroup<A>
    {
        public static readonly SemiOption<SemigroupA, A> Inst = default(SemiOption<SemigroupA, A>);

        [Pure]
        public Option<A> None => 
            default;
 
        [Pure]
        public MB Bind<MonadB, MB, B>(Option<A> ma, Func<A, MB> f) where MonadB : struct, Monad<Unit, Unit, MB, B> =>
            ma.IsSome
                ? (f ?? throw new ArgumentNullException(nameof(f)))(ma.Value)
                : default(MonadB).Fail(ValueIsNoneException.Default);

        [Pure]
        public MB BindAsync<MonadB, MB, B>(Option<A> ma, Func<A, MB> f) where MonadB : struct, MonadAsync<Unit, Unit, MB, B> =>
            ma.IsSome
                ? (f ?? throw new ArgumentNullException(nameof(f)))(ma.Value)
                : default(MonadB).Fail(ValueIsNoneException.Default);

        [Pure]
        public Option<A> Fail(object err = null) =>
            Option<A>.None;

        [Pure]
        public Option<A> Plus(Option<A> a, Option<A> b) =>
            a.IsSome && b.IsSome 
                ? Some(default(SemigroupA).Append(a.Value, b.Value)) 
                : a.IsSome
                    ? a
                    : b;

        [Pure]
        public Option<A> Return(Func<Unit, A> f) =>
            Option<A>.Some(f(unit));

        [Pure]
        public Option<A> Zero() =>
            Option<A>.None;

        [Pure]
        public bool IsNone(Option<A> opt) =>
            opt.IsNone;

        [Pure]
        public bool IsSome(Option<A> opt) =>
            opt.IsSome;

        [Pure]
        public B Match<B>(Option<A> opt, Func<A, B> Some, Func<B> None) =>
            opt.Match(Some, None);

        [Pure]
        public B Match<B>(Option<A> opt, Func<A, B> Some, B None) =>
            opt.Match(Some, None);

        public Unit Match(Option<A> opt, Action<A> Some, Action None) =>
            opt.Match(Some, None);

        [Pure]
        public B MatchUnsafe<B>(Option<A> opt, Func<A, B> Some, Func<B> None) =>
            opt.MatchUnsafe(Some, None);

        [Pure]
        public B MatchUnsafe<B>(Option<A> opt, Func<A, B> Some, B None) =>
            opt.MatchUnsafe(Some, None);

        [Pure]
        public Func<Unit, S> Fold<S>(Option<A> ma, S state, Func<S, A, S> f) => _ =>
            ma.Fold(state, f);

        [Pure]
        public Func<Unit, S> FoldBack<S>(Option<A> ma, S state, Func<S, A, S> f) => _ =>
            ma.FoldBack(state, f);

        [Pure]
        public S BiFold<S>(Option<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
            ma.BiFold(state, fa, fb);

        [Pure]
        public S BiFoldBack<S>(Option<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
            ma.BiFold(state, fa, fb);

        [Pure]
        public Func<Unit, int> Count(Option<A> ma) => _ =>
            ma.IsSome ? 1 : 0;

        [Pure]
        public Option<A> Some(A x) =>
            Option<A>.Some(x);

        [Pure]
        public Option<A> Optional(A x) =>
            isnull(x)
                ? default
                : Option<A>.Some(x);

        [Pure]
        public Option<A> Run(Func<Unit, Option<A>> ma) =>
            new Option<A>(ma(unit));

        [Pure]
        public Option<A> BindReturn(Unit _, Option<A> mb) =>
            mb;

        [Pure]
        public Option<A> Return(A x) =>
            Some(x);

        [Pure]
        public Option<A> Empty() =>
            default;

        [Pure]
        public Option<A> Append(Option<A> x, Option<A> y) =>
            Plus(x, y);

        [Pure]
        public bool Equals(Option<A> x, Option<A> y) =>
            x.Equals(y);

        [Pure]
        public int GetHashCode(Option<A> x) =>
            x.GetHashCode();

        [Pure]
        public Option<A> Apply(Func<A, A, A> f, Option<A> fa, Option<A> fb) =>
            fa.IsSome && fb.IsSome
                ? Some(f(fa.Value, fb.Value))
                : default;

        [Pure]
        public int Compare(Option<A> x, Option<A> y) =>
            compare<OrdDefault<A>, A>(x, y);

        [Pure]
        public OptionAsync<A> ToAsync(Option<A> sa) =>
            sa.ToAsync();
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(Option<A> x, Option<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Option<A> x) =>
            GetHashCode(x).AsTask();         
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(Option<A> x, Option<A> y) =>
            Compare(x, y).AsTask();
    }
}
