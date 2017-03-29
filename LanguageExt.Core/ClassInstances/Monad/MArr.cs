using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;
using LanguageExt;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Array type-class instance
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    public struct MArr<A> :
        Monad<Arr<A>, A>,
        Foldable<Arr<A>, A>,
        Eq<Arr<A>>,
        Monoid<Arr<A>>
   {
        public static readonly MArr<A> Inst = default(MArr<A>);

        [Pure]
        public Arr<A> Append(Arr<A> x, Arr<A> y) =>
            x.Concat(y).ToArray();

        [Pure]
        public MB Bind<MONADB, MB, B>(Arr<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            traverse<MArr<A>, MONADB, Arr<A>, MB, A, B>(ma, f);

        [Pure]
        public Func<Unit, int> Count(Arr<A> fa) =>
            _ => fa.Count();

        [Pure]
        public Arr<A> Empty() =>
            Arr<A>.Empty;

        [Pure]
        public bool Equals(Arr<A> x, Arr<A> y) =>
            Enumerable.SequenceEqual(x, y);

        [Pure]
        public Arr<A> Fail(object err) =>
            Empty();

        [Pure]
        public Arr<A> Fail(Exception err = null) =>
            Empty();

        [Pure]
        public Func<Unit, S> Fold<S>(Arr<A> fa, S state, Func<S, A, S> f) =>
            _ => fa.Fold(state, f);

        [Pure]
        public Func<Unit, S> FoldBack<S>(Arr<A> fa, S state, Func<S, A, S> f) =>
            _ => fa.FoldBack(state, f);

        [Pure]
        public Arr<A> Plus(Arr<A> ma, Arr<A> mb) =>
            ma + mb;

        [Pure]
        public Arr<A> Return(Func<Unit, A> f) =>
            Arr.create(f(unit));

        [Pure]
        public Arr<A> Return(A x) =>
            Return(_ => x);

        [Pure]
        public Arr<A> Zero() =>
            Empty();

        [Pure]
        public int GetHashCode(Arr<A> x) =>
            x.GetHashCode();

        [Pure]
        public FunctorAB ToFunctor<FunctorAB, MB, B>() where FunctorAB : struct, Functor<Arr<A>, MB, A, B> =>
            default(FunctorAB);

        [Pure]
        public Arr<A> Id(Func<Unit, Arr<A>> f) =>
            f(unit);

        [Pure]
        public Arr<A> BindReturn(Unit _, Arr<A> fmb) =>
            fmb;

        [Pure]
        public Arr<A> IdAsync(Func<Unit, Task<Arr<A>>> ma) =>
            ma(unit).Result;

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(Arr<A> fa, S state, Func<S, A, S> f) => _ =>
            Task.FromResult(Inst.Fold<S>(fa, state, f)(_));

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(Arr<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
        {
            Task<S> s = Task.FromResult(state);
            foreach(var item in fa)
            {
                s = from x in s
                    from y in f(x, item)
                    select y;
            }
            return s;
        };

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(Arr<A> fa, S state, Func<S, A, S> f) => _ =>
             Task.FromResult(Inst.FoldBack<S>(fa, state, f)(_));

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(Arr<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
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
        public Func<Unit, Task<int>> CountAsync(Arr<A> fa) => _ =>
            Task.FromResult(Inst.Count(fa)(_));
    }
}
