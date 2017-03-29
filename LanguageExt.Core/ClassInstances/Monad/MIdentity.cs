using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public struct MIdentity<A> : Monad<Identity<A>, A>
    {
        public static readonly MIdentity<A> Inst = new MIdentity<A>();

        [Pure]
        public MB Bind<MONADB, MB, B>(Identity<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            f(ma.Value);

        [Pure]
        public Identity<A> BindReturn(Unit maOutput, Identity<A> mb) =>
            mb;

        [Pure]
        public Func<Unit, int> Count(Identity<A> fa) => _ =>
            1;

        [Pure]
        public Identity<A> Fail(Exception err = null) =>
            Identity<A>.Bottom;

        [Pure]
        public Identity<A> Fail(object err) =>
            Identity<A>.Bottom;

        [Pure]
        public Func<Unit, S> Fold<S>(Identity<A> fa, S state, Func<S, A, S> f) =>
            _ => f(state, fa.Value);

        [Pure]
        public Func<Unit, S> FoldBack<S>(Identity<A> fa, S state, Func<S, A, S> f) =>
            _ => f(state, fa.Value);

        [Pure]
        public Identity<A> Id(Func<Unit, Identity<A>> ma) =>
            ma(unit);

        [Pure]
        public Identity<A> Plus(Identity<A> a, Identity<A> b) =>
            a.IsBottom
                ? b
                : a;

        [Pure]
        public Identity<A> Return(Func<Unit, A> f) =>
            new Identity<A>(f(unit));

        [Pure]
        public Identity<A> Zero() =>
            Identity<A>.Bottom;

        [Pure]
        public Identity<A> Return(A x) =>
            Return(_ => x);

        [Pure]
        public Identity<A> IdAsync(Func<Unit, Task<Identity<A>>> ma) =>
            ma(unit).Result;

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(Identity<A> fa, S state, Func<S, A, S> f) => _ =>
            Task.FromResult(Inst.Fold<S>(fa, state, f)(_));

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(Identity<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
            f(state, fa.Value);

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(Identity<A> fa, S state, Func<S, A, S> f) => _ =>
             Task.FromResult(Inst.FoldBack<S>(fa, state, f)(_));

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(Identity<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
            f(state, fa.Value);

        [Pure]
        public Func<Unit, Task<int>> CountAsync(Identity<A> fa) => _ =>
            Task.FromResult(Inst.Count(fa)(_));
    }
}
