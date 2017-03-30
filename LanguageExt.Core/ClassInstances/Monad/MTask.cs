using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using System.Reflection;

namespace LanguageExt.ClassInstances
{
    public struct MTask<A> :
        Optional<Task<A>, A>,
        Monad<Task<A>, A>,
        BiFoldable<Task<A>, A, Unit>
    {
        public static readonly MTask<A> Inst = default(MTask<A>);

        [Pure]
        public Task<A> None => BottomException.Default.AsFailedTask<A>();

        [Pure]
        public MB Bind<MONADB, MB, B>(Task<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            default(MONADB).IdAsync(_ => ma.ContinueWith(task =>
                task.IsFaulted || task.IsCanceled
                    ? default(MONADB).Fail()
                    : f(task.Result)));

        [Pure]
        public Task<A> Fail(object err) =>
            None;

        [Pure]
        public Task<A> Fail(Exception err = null) =>
            (err ?? new BottomException()).AsFailedTask<A>();

        [Pure]
        public async Task<A> Plus(Task<A> ma, Task<A> mb)
        {
            var tasks = Set<OrdTask<A>, Task<A>>(ma, mb);

            // Run in parallel
            while(tasks.Count > 0)
            {
                // Return first one that completes
                var completed = await Task.WhenAny(tasks);
                if (!completed.IsFaulted) return completed.Result;
                tasks = tasks.Remove(completed);
            }
            return await None;
        }

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="x">The bound monad value</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public Task<A> Return(Func<Unit, A> f) =>
            Task.Run(() => f(unit));

        [Pure]
        public Task<A> Zero() => 
            None;

        [Pure]
        public bool IsNone(Task<A> ma) =>
            ma.IsFaulted;

        [Pure]
        public bool IsSome(Task<A> ma) =>
            !IsNone(ma);

        [Pure]
        public bool IsUnsafe(Task<A> ma) =>
            true;

        [Pure]
        public B Match<B>(Task<A> ma, Func<A, B> Some, Func<B> None)
        {
            if (ma.IsFaulted)
                return None();
            else
                return Some(ma.Result);
        }

        public Unit Match(Task<A> ma, Action<A> Some, Action None)
        {
            if (ma.IsFaulted) None(); else Some(ma.Result);
            return unit;
        }

        [Pure]
        public B MatchUnsafe<B>(Task<A> ma, Func<A, B> Some, Func<B> None)
        {
            if (ma.IsFaulted)
                return None();
            else
                return Some(ma.Result);
        }

        [Pure]
        public Func<Unit, S> Fold<S>(Task<A> ma, S state, Func<S, A, S> f) => _ =>
        {
            if (ma.IsFaulted) return state;
            return f(state, ma.Result);
        };

        [Pure]
        public Func<Unit, S> FoldBack<S>(Task<A> ma, S state, Func<S, A, S> f) => _ =>
        {
            if (ma.IsFaulted) return state;
            return f(state, ma.Result);
        };

        [Pure]
        public S BiFold<S>(Task<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb)
        {
            if (ma.IsFaulted)
                return fb(state, unit);
            else
                return fa(state, ma.Result);
        }

        [Pure]
        public S BiFoldBack<S>(Task<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb)
        {
            if (ma.IsFaulted)
                return fb(state, unit);
            else
                return fa(state, ma.Result);
        }

        [Pure]
        public Func<Unit, int> Count(Task<A> ma) => _ =>
            ma.IsFaulted
                ? 0
                : 1;

        [Pure]
        public Task<A> Some(A value) =>
            Task.FromResult(value);

        [Pure]
        public Task<A> Optional(A value) =>
            Task.FromResult(value);

        [Pure]
        public Task<A> Id(Func<Unit, Task<A>> ma) =>
            ma(unit);

        [Pure]
        public Task<A> BindReturn(Unit _, Task<A> mb) =>
            mb;

        [Pure]
        public Task<A> Return(A x) =>
            Return(_ => x);

        [Pure]
        public Task<A> IdAsync(Func<Unit, Task<Task<A>>> ma) =>
            from ta in ma(unit)
            from a in ta
            select a;

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(Task<A> fa, S state, Func<S, A, S> f) => _ =>
            from a in fa
            select f(state, a);

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(Task<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
            from a in fa
            from s in f(state, a)
            select s;

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(Task<A> fa, S state, Func<S, A, S> f) => _ =>
            from a in fa
            select f(state, a);

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(Task<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
            from a in fa
            from s in f(state, a)
            select s;

        [Pure]
        public Func<Unit, Task<int>> CountAsync(Task<A> fa) => _ =>
            Task.FromResult(Inst.Count(fa)(_));
    }
}
