using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Hash set type-class instance
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public struct MHashSet<A> :
        Monad<HashSet<A>, A>,
        Eq<HashSet<A>>,
        Monoid<HashSet<A>>
   {
        public static readonly MHashSet<A> Inst = default(MHashSet<A>);

        [Pure]
        public HashSet<A> Append(HashSet<A> x, HashSet<A> y) =>
            HashSet.createRange(x.ConcatFast(y));

        [Pure]
        public MB Bind<MONADB, MB, B>(HashSet<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            ma.Fold(default(MONADB).Zero(), (s, a) => default(MONADB).Plus(s, f(a)));

        [Pure]
        public MB BindAsync<MONADB, MB, B>(HashSet<A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            ma.Fold(default(MONADB).Zero(), (s, a) => default(MONADB).Plus(s, f(a)));

        [Pure]
        public Func<Unit, int> Count(HashSet<A> fa) =>
            _ => fa.Count();

        [Pure]
        public HashSet<A> Subtract(HashSet<A> x, HashSet<A> y) =>
            HashSet.createRange(Enumerable.Except(x, y));

        [Pure]
        public HashSet<A> Empty() =>
            HashSet.empty<A>();

        [Pure]
        public bool Equals(HashSet<A> x, HashSet<A> y) =>
            x == y;

        [Pure]
        public HashSet<A> Fail(object err = null) =>
            Empty();

        [Pure]
        public Func<Unit, S> Fold<S>(HashSet<A> fa, S state, Func<S, A, S> f) =>
            _ => fa.Fold(state, f);

        [Pure]
        public Func<Unit, S> FoldBack<S>(HashSet<A> fa, S state, Func<S, A, S> f) =>
            _ => fa.FoldBack(state, f);

        [Pure]
        public HashSet<A> Plus(HashSet<A> ma, HashSet<A> mb) =>
            ma + mb;

        [Pure]
        public HashSet<A> Return(Func<Unit, A> f) =>
            HashSet(f(unit));

        [Pure]
        public HashSet<A> Zero() =>
            Empty();

        [Pure]
        public int GetHashCode(HashSet<A> x) =>
            x.GetHashCode();

        [Pure]
        public HashSet<A> Run(Func<Unit, HashSet<A>> ma) =>
            ma(unit);

        [Pure]
        public HashSet<A> BindReturn(Unit maOutput, HashSet<A> mb) =>
            mb;

        [Pure]
        public HashSet<A> Return(A x) =>
            Return(_ => x);

        [Pure]
        public HashSet<A> Apply(Func<A, A, A> f, HashSet<A> fa, HashSet<A> fb) =>
            from a in fa
            from b in fb
            select f(a, b);
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(HashSet<A> x, HashSet<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(HashSet<A> x) =>
            GetHashCode(x).AsTask();         
    }
}
