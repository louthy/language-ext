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
    /// MLst type-class instance
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    public struct MLst<A> :
        Monad<Lst<A>, A>,
        Foldable<Lst<A>, A>,
        Eq<Lst<A>>,
        Monoid<Lst<A>>
   {
        public static readonly MLst<A> Inst = default(MLst<A>);

        [Pure]
        public Lst<A> Append(Lst<A> x, Lst<A> y) =>
            x.Concat(y).Freeze();

        [Pure]
        public MB Bind<MONADB, MB, B>(Lst<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            traverse<MLst<A>, MONADB, Lst<A>, MB, A, B>(ma, f);

        [Pure]
        public Func<Unit, int> Count(Lst<A> fa) => _ =>
            fa.Count();

        [Pure]
        public Lst<A> Subtract(Lst<A> x, Lst<A> y) =>
            Enumerable.Except(x, y).Freeze();

        [Pure]
        public Lst<A> Empty() =>
            List.empty<A>();

        [Pure]
        public bool Equals(Lst<A> x, Lst<A> y) =>
            Enumerable.SequenceEqual(x, y);

        [Pure]
        public Lst<A> Fail(object err) =>
            Empty();

        [Pure]
        public Lst<A> Fail(Exception err = null) =>
            Empty();

        [Pure]
        public Func<Unit, S> Fold<S>(Lst<A> fa, S state, Func<S, A, S> f) => _ =>
             fa.Fold(state, f);

        [Pure]
        public Func<Unit, S> FoldBack<S>(Lst<A> fa, S state, Func<S, A, S> f) => _ =>
             fa.FoldBack(state, f);

        [Pure]
        public Lst<A> Plus(Lst<A> ma, Lst<A> mb) =>
            ma + mb;

        [Pure]
        public Lst<A> Return(Func<Unit, A> f) =>
            List(f(unit));

        [Pure]
        public Lst<A> Zero() =>
            Empty();

        [Pure]
        public int GetHashCode(Lst<A> x) =>
            x.GetHashCode();

        [Pure]
        public Lst<A> Id(Func<Unit, Lst<A>> ma) =>
            ma(unit);

        [Pure]
        public Lst<A> BindReturn(Unit maOutput, Lst<A> mb) =>
            mb;

        [Pure]
        public Lst<A> Return(A x) =>
            Return(_ => x);

        [Pure]
        public Lst<A> IdAsync(Func<Unit, Task<Lst<A>>> ma) =>
            ma(unit).Result;

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(Lst<A> fa, S state, Func<S, A, S> f) => _ =>
            Task.FromResult(Inst.Fold<S>(fa, state, f)(_));

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(Lst<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
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
        public Func<Unit, Task<S>> FoldBackAsync<S>(Lst<A> fa, S state, Func<S, A, S> f) => _ =>
             Task.FromResult(Inst.FoldBack<S>(fa, state, f)(_));

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(Lst<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
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
        public Func<Unit, Task<int>> CountAsync(Lst<A> fa) => _ =>
            Task.FromResult(Inst.Count(fa)(_));
    }
}
