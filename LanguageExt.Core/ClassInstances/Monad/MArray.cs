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
    /// Array type-class instance
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public struct MArray<A> :
        Monad<A[], A>,
        Foldable<A[], A>,
        Eq<A[]>,
        Monoid<A[]>
   {
        public static readonly MArray<A> Inst = default(MArray<A>);

        [Pure]
        public A[] Append(A[] x, A[] y) =>
            x.Concat(y).ToArray();

        [Pure]
        public MB Bind<MONADB, MB, B>(A[] ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            ma.Fold(default(MONADB).Zero(), (s, a) => default(MONADB).Plus(s, f(a)));

        [Pure]
        public Func<Unit, int> Count(A[] fa) =>
            _ => fa.Count();

        [Pure]
        public A[] Subtract(A[] x, A[] y) =>
            Enumerable.Except(x, y).ToArray();

        [Pure]
        public A[] Empty() =>
            new A[0];

        [Pure]
        public bool Equals(A[] x, A[] y) =>
            Enumerable.SequenceEqual(x, y);

        [Pure]
        public A[] Fail(object err) =>
            Empty();

        [Pure]
        public A[] Fail(Exception err = null) =>
            Empty();

        [Pure]
        public Func<Unit, S> Fold<S>(A[] fa, S state, Func<S, A, S> f) =>
            _ => fa.Fold(state, f);

        [Pure]
        public Func<Unit, S> FoldBack<S>(A[] fa, S state, Func<S, A, S> f) =>
            _ => fa.FoldBack(state, f);

        [Pure]
        public A[] Plus(A[] ma, A[] mb) =>
            PlusSeq(ma, mb).ToArray();

        [Pure]
        IEnumerable<A> PlusSeq(A[] ma, A[] mb)
        {
            foreach (var a in ma) yield return a;
            foreach (var b in mb) yield return b;
        }

        [Pure]
        public A[] Return(Func<Unit, A> x) =>
            new[] { x(unit) };

        [Pure]
        public A[] Return(A x) =>
            Return(_ => x);

        [Pure]
        public A[] Zero() =>
            Empty();

        [Pure]
        public int GetHashCode(A[] x) =>
            hash(x);

        [Pure]
        public A[] Id(Func<Unit, A[]> ma) =>
            ma(unit);

        [Pure]
        public A[] BindReturn(Unit _, A[] mb) =>
            mb;

        [Pure]
        public A[] IdAsync(Func<Unit, Task<A[]>> ma) =>
            ma(unit).Result;

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(A[] fa, S state, Func<S, A, S> f) => _ =>
            Task.FromResult(Inst.Fold<S>(fa, state, f)(_));

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(A[] fa, S state, Func<S, A, Task<S>> f) => _ =>
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
        public Func<Unit, Task<S>> FoldBackAsync<S>(A[] fa, S state, Func<S, A, S> f) => _ =>
             Task.FromResult(Inst.FoldBack<S>(fa, state, f)(_));

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(A[] fa, S state, Func<S, A, Task<S>> f) => _ =>
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
        public Func<Unit, Task<int>> CountAsync(A[] fa) => _ =>
            Task.FromResult(Inst.Count(fa)(_));
    }
}
