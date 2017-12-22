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
        OptionalAsync<Task<A>, A>,
        MonadAsync<Task<A>, A>,
        BiFoldableAsync<Task<A>, A, Unit>
    {
        public static readonly MTask<A> Inst = default(MTask<A>);

        [Pure]
        public Task<A> NoneAsync =>
            BottomException.Default.AsFailedTask<A>();

        [Pure]
        public MB BindAsync<MONADB, MB, B>(Task<A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MONADB).RunAsync(async _ => f(await ma));

        [Pure]
        public MB BindAsync<MONADB, MB, B>(Task<A> ma, Func<A, Task<MB>> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MONADB).RunAsync(async _ => await f(await ma));

        [Pure]
        public Task<A> FailAsync(object err = null) =>
            err != null && err is Exception
                ? ((Exception)err).AsFailedTask<A>()
                : NoneAsync;

        [Pure]
        public async Task<A> PlusAsync(Task<A> ma, Task<A> mb)
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
            return await NoneAsync;
        }

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="x">The bound monad value</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public Task<A> ReturnAsync(Task<A> x) =>
            x;

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <returns>Monad of A</returns>
        [Pure]
        public async Task<A> ReturnAsync(Func<Unit, Task<A>> f) =>
            await f(unit);

        [Pure]
        public Task<A> ZeroAsync() => 
            NoneAsync;

        [Pure]
        public Task<bool> IsNoneAsync(Task<A> ma) =>
            Task.FromResult(ma.IsFaulted);

        [Pure]
        public Task<bool> IsSomeAsync(Task<A> ma) =>
            from a in IsNoneAsync(ma)
            select !a;

        [Pure]
        public Task<bool> IsUnsafeAsync(Task<A> ma) =>
            Task.FromResult(true);

        [Pure]
        public async Task<B> MatchAsync<B>(Task<A> ma, Func<A, B> Some, Func<B> None)
        {
            if(ma.IsCanceled || ma.IsFaulted)
            {
                return None();
            }
            try
            {
                var a = await ma;
                return Some(a);
            }
            catch (Exception)
            {
                return None();
            }
        }

        [Pure]
        public Task<A> Some(A value) =>
            Task.FromResult(value);

        [Pure]
        public Task<A> Optional(A value) =>
            Task.FromResult(value);

        [Pure]
        public Task<A> BindReturnAsync(Unit _, Task<A> mb) =>
            mb;

        [Pure]
        public Task<A> RunAsync(Func<Unit, Task<Task<A>>> ma) =>
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
            default(MTask<A>).MatchAsync(fa,
                Some: x  => 1,
                None: () => 0);

        [Pure]
        public async Task<A> ApplyAsync(Func<A, A, A> f, Task<A> fa, Task<A> fb) 
        {
            await Task.WhenAll(fa, fb);
            return !fa.IsFaulted && !fb.IsFaulted
                ? f(fa.Result, fb.Result)
                : throw fa.Exception;
        }

        public async Task<B> MatchAsync<B>(Task<A> ma, Func<A, Task<B>> Some, Func<B> None)
        {
            if (ma.IsCanceled || ma.IsFaulted)
            {
                return None();
            }
            try
            {
                var a = await ma;
                return await Some(a);
            }
            catch (Exception)
            {
                return None();
            }
        }

        public async Task<B> MatchAsync<B>(Task<A> ma, Func<A, B> Some, Func<Task<B>> None)
        {
            if (ma.IsCanceled || ma.IsFaulted)
            {
                return await None();
            }
            try
            {
                var a = await ma;
                return Some(a);
            }
            catch (Exception)
            {
                return await None();
            }
        }

        public async Task<B> MatchAsync<B>(Task<A> ma, Func<A, Task<B>> Some, Func<Task<B>> None)
        {
            if (ma.IsCanceled || ma.IsFaulted)
            {
                return await None();
            }
            try
            {
                var a = await ma;
                return await Some(a);
            }
            catch (Exception)
            {
                return await None();
            }
        }

        public async Task<B> MatchUnsafeAsync<B>(Task<A> ma, Func<A, B> Some, Func<B> None)
        {
            if (ma.IsCanceled || ma.IsFaulted)
            {
                return None();
            }
            try
            {
                var a = await ma;
                return Some(a);
            }
            catch (Exception)
            {
                return None();
            }
        }

        public async Task<B> MatchUnsafeAsync<B>(Task<A> ma, Func<A, Task<B>> Some, Func<B> None)
        {
            if (ma.IsCanceled || ma.IsFaulted)
            {
                return None();
            }
            try
            {
                var a = await ma;
                return await Some(a);
            }
            catch (Exception)
            {
                return None();
            }
        }

        public async Task<B> MatchUnsafeAsync<B>(Task<A> ma, Func<A, B> Some, Func<Task<B>> None)
        {
            if (ma.IsCanceled || ma.IsFaulted)
            {
                return await None();
            }
            try
            {
                var a = await ma;
                return Some(a);
            }
            catch (Exception)
            {
                return await None();
            }
        }

        public async Task<B> MatchUnsafeAsync<B>(Task<A> ma, Func<A, Task<B>> Some, Func<Task<B>> None)
        {
            if (ma.IsCanceled || ma.IsFaulted)
            {
                return await None();
            }
            try
            {
                var a = await ma;
                return await Some(a);
            }
            catch (Exception)
            {
                return await None();
            }
        }

        public async Task<Unit> MatchAsync(Task<A> ma, Action<A> Some, Action None)
        {
            if (ma.IsCanceled || ma.IsFaulted)
            {
                None();
                return unit;
            }
            try
            {
                var a = await ma;
                Some(a);
            }
            catch (Exception)
            {
                None();
            }
            return unit;
        }

        public Task<A> SomeAsync(A value) =>
            Task.FromResult(value);

        public Task<A> OptionalAsync(A value) =>
            Task.FromResult(value);

        public Task<S> BiFoldAsync<S>(Task<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
            MatchAsync(ma,
                Some: x  => fa(state, x),
                None: () => fb(state, unit));

        public Task<S> BiFoldAsync<S>(Task<A> ma, S state, Func<S, A, Task<S>> fa, Func<S, Unit, S> fb) =>
            MatchAsync(ma,
                Some: x => fa(state, x),
                None: () => fb(state, unit));

        public Task<S> BiFoldAsync<S>(Task<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, Task<S>> fb) =>
            MatchAsync(ma,
                Some: x => fa(state, x),
                None: () => fb(state, unit));

        public Task<S> BiFoldAsync<S>(Task<A> ma, S state, Func<S, A, Task<S>> fa, Func<S, Unit, Task<S>> fb) =>
            MatchAsync(ma,
                Some: x => fa(state, x),
                None: () => fb(state, unit));

        public Task<S> BiFoldBackAsync<S>(Task<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
            MatchAsync(ma,
                Some: x => fa(state, x),
                None: () => fb(state, unit));

        public Task<S> BiFoldBackAsync<S>(Task<A> ma, S state, Func<S, A, Task<S>> fa, Func<S, Unit, S> fb) =>
            MatchAsync(ma,
                Some: x => fa(state, x),
                None: () => fb(state, unit));

        public Task<S> BiFoldBackAsync<S>(Task<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, Task<S>> fb) =>
            MatchAsync(ma,
                Some: x => fa(state, x),
                None: () => fb(state, unit));

        public Task<S> BiFoldBackAsync<S>(Task<A> ma, S state, Func<S, A, Task<S>> fa, Func<S, Unit, Task<S>> fb) =>
            MatchAsync(ma,
                Some: x => fa(state, x),
                None: () => fb(state, unit));
    }
}
