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
            HashSet.createRange(x.Concat(y));

        [Pure]
        public MB Bind<MONADB, MB, B>(HashSet<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
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
        public HashSet<A> Fail(object err) =>
            Empty();

        [Pure]
        public HashSet<A> Fail(Exception err = null) =>
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
        public HashSet<A> Id(Func<Unit, HashSet<A>> ma) =>
            ma(unit);

        [Pure]
        public HashSet<A> BindReturn(Unit maOutput, HashSet<A> mb) =>
            mb;

        [Pure]
        public HashSet<A> Return(A x) =>
            Return(_ => x);

        [Pure]
        public HashSet<A> IdAsync(Func<Unit, Task<HashSet<A>>> ma) =>
            ma(unit).Result;

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(HashSet<A> fa, S state, Func<S, A, S> f) => _ =>
            Task.FromResult(Inst.Fold<S>(fa, state, f)(_));

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(HashSet<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
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
        public Func<Unit, Task<S>> FoldBackAsync<S>(HashSet<A> fa, S state, Func<S, A, S> f) => _ =>
             Task.FromResult(Inst.FoldBack<S>(fa, state, f)(_));

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(HashSet<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
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
        public Func<Unit, Task<int>> CountAsync(HashSet<A> fa) => _ =>
            Task.FromResult(Inst.Count(fa)(_));
    }
}
