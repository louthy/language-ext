using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
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
            Set.createRange(x.Concat(y));

        [Pure]
        public MB Bind<MONADB, MB, B>(Set<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            traverse<MSet<A>, MONADB, Set<A>, MB, A, B>(ma, f);

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
        public Set<A> Fail(object err) =>
            Empty();

        [Pure]
        public Set<A> Fail(Exception err = null) =>
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
        public Set<A> Id(Func<Unit, Set<A>> ma) =>
            ma(unit);

        [Pure]
        public Set<A> BindReturn(Unit _, Set<A> mb) =>
            mb;

        [Pure]
        public Set<A> IdAsync(Func<Unit, Task<Set<A>>> ma) =>
            ma(unit).Result;

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(Set<A> fa, S state, Func<S, A, S> f) => _ =>
            Task.FromResult(Inst.Fold<S>(fa, state, f)(_));

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(Set<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
        {
            Task<S> s = Task.FromResult(state);
            foreach (var item in fa)
            {
                s = from x in s
                    from y in f(x, item)
                    select y;
            }
            return s;
        };

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(Set<A> fa, S state, Func<S, A, S> f) => _ =>
             Task.FromResult(Inst.FoldBack<S>(fa, state, f)(_));

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(Set<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
        {
            Task<S> s = Task.FromResult(state);
            foreach (var item in fa.Reverse())
            {
                s = from x in s
                    from y in f(x, item)
                    select y;
            }
            return s;
        };

        [Pure]
        public Func<Unit, Task<int>> CountAsync(Set<A> fa) => _ =>
            Task.FromResult(Inst.Count(fa)(_));
    }
}
