using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Class instance to give `Task<A>` the following traits: 
    ///     
    ///     MonadAsync
    ///     FoldableAsync
    ///     BiFoldableAsync
    ///     OptionalAsymc
    ///     OptionalUnsafeAsync
    /// </summary>
    /// <remarks>
    /// The `Plus` function will allow `ma` and `mb` to run in parallel and 
    /// will return the result of the first to complete successfully.
    /// </remarks>
    /// <typeparam name="A">Bound value type</typeparam>
    public struct MTaskFirst<A> :
        OptionalAsync<Task<A>, A>,
        OptionalUnsafeAsync<Task<A>, A>,
        MonadAsync<Task<A>, A>,
        FoldableAsync<Task<A>, A>,
        BiFoldableAsync<Task<A>, A, Unit>
    {
        public static readonly MTaskFirst<A> Inst = default(MTaskFirst<A>);

        [Pure]
        public Task<A> None =>
            BottomException.Default.AsFailedTask<A>();

        [Pure]
        public MB Bind<MONADB, MB, B>(Task<A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MONADB).RunAsync(async _ => f(await ma));

        [Pure]
        public MB BindAsync<MONADB, MB, B>(Task<A> ma, Func<A, Task<MB>> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MONADB).RunAsync(async _ => await f(await ma));

        [Pure]
        public Task<A> Fail(object err = null) =>
            err != null && err is Exception
                ? ((Exception)err).AsFailedTask<A>()
                : None;

        /// <summary>
        /// The `Plus` function will allow `ma` and `mb` to run in parallel and 
        /// will return the result of the first to complete successfully.
        /// </summary>
        [Pure]
        public async Task<A> Plus(Task<A> ma, Task<A> mb)
        {
            var tasks = HashSet<OrdTask<A>, Task<A>>(ma, mb);

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
        public Task<A> Zero() => 
            None;

        [Pure]
        public Task<bool> IsNone(Task<A> ma) =>
            ma.IsFaulted.AsTask();

        [Pure]
        public Task<bool> IsSome(Task<A> ma) =>
            from a in IsNone(ma)
            select !a;

        [Pure]
        public async Task<B> Match<B>(Task<A> ma, Func<A, B> Some, Func<B> None)
        {
            if(ma.IsCanceled || ma.IsFaulted)
            {
                return Check.NullReturn(None());
            }
            try
            {
                var a = await ma;
                return Check.NullReturn(Some(a));
            }
            catch (Exception)
            {
                return Check.NullReturn(None());
            }
        }

        [Pure]
        public Task<A> Some(A value) =>
            value.AsTask();

        [Pure]
        public Task<A> Optional(A value) =>
            value.AsTask();

        [Pure]
        public Task<A> BindReturn(Unit _, Task<A> mb) =>
            mb;

        [Pure]
        public Task<A> RunAsync(Func<Unit, Task<Task<A>>> ma) =>
            from ta in ma(unit)
            from a in ta
            select a;

        [Pure]
        public Func<Unit, Task<S>> Fold<S>(Task<A> fa, S state, Func<S, A, S> f) => _ =>
            from a in fa
            select f(state, a);

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(Task<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
            from a in fa
            from s in f(state, a)
            select s;

        [Pure]
        public Func<Unit, Task<S>> FoldBack<S>(Task<A> fa, S state, Func<S, A, S> f) => _ =>
            from a in fa
            select f(state, a);

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(Task<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
            from a in fa
            from s in f(state, a)
            select s;

        [Pure]
        public Func<Unit, Task<int>> Count(Task<A> fa) => _ =>
            default(MTaskFirst<A>).Match(fa,
                Some: x  => 1,
                None: () => 0);

        [Pure]
        public async Task<A> Apply(Func<A, A, A> f, Task<A> fa, Task<A> fb) 
        {
            await Task.WhenAll(fa, fb);
            return f(fa.Result, fb.Result);
        }

        public async Task<B> MatchAsync<B>(Task<A> ma, Func<A, Task<B>> SomeAsync, Func<B> None)
        {
            if (ma.IsCanceled || ma.IsFaulted)
            {
                return Check.NullReturn(None());
            }
            try
            {
                var a = await ma;
                return Check.NullReturn(await SomeAsync(a));
            }
            catch (Exception)
            {
                return Check.NullReturn(None());
            }
        }

        public async Task<B> MatchAsync<B>(Task<A> ma, Func<A, B> Some, Func<Task<B>> NoneAsync)
        {
            if (ma.IsCanceled || ma.IsFaulted)
            {
                return Check.NullReturn(await NoneAsync());
            }
            try
            {
                var a = await ma;
                return Check.NullReturn(Some(a));
            }
            catch (Exception)
            {
                return Check.NullReturn(await NoneAsync());
            }
        }

        public async Task<B> MatchAsync<B>(Task<A> ma, Func<A, Task<B>> SomeAsync, Func<Task<B>> NoneAsync)
        {
            if (ma.IsCanceled || ma.IsFaulted)
            {
                return Check.NullReturn(await NoneAsync());
            }
            try
            {
                var a = await ma;
                return Check.NullReturn(await SomeAsync(a));
            }
            catch (Exception)
            {
                return Check.NullReturn(await NoneAsync());
            }
        }

        public async Task<Unit> Match(Task<A> ma, Action<A> Some, Action None)
        {
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

        public async Task<Unit> MatchAsync(Task<A> ma, Func<A, Task> SomeAsync, Action None)
        {
            try
            {
                var a = await ma;
                await SomeAsync(a);
            }
            catch (Exception)
            {
                None();
            }
            return unit;
        }

        public async Task<Unit> MatchAsync(Task<A> ma, Action<A> Some, Func<Task> None)
        {
            try
            {
                var a = await ma;
                Some(a);
            }
            catch (Exception)
            {
                await None();
            }
            return unit;
        }

        public async Task<Unit> MatchAsync(Task<A> ma, Func<A, Task> SomeAsync, Func<Task> NoneAsync)
        {
            try
            {
                var a = await ma;
                await SomeAsync(a);
            }
            catch (Exception)
            {
                await NoneAsync();
            }
            return unit;
        }

        public Task<A> SomeAsync(Task<A> value) =>
            value;

        public Task<A> OptionalAsync(Task<A> value) =>
            value;

        public Task<S> BiFold<S>(Task<A> ma, S state, Func<S, A, S> Some, Func<S, Unit, S> None) =>
            Match(ma,
                Some: x  => Some(state, x),
                None: () => None(state, unit));

        public Task<S> BiFoldAsync<S>(Task<A> ma, S state, Func<S, A, Task<S>> SomeAsync, Func<S, Unit, S> None) =>
            MatchAsync(ma,
                SomeAsync: x => SomeAsync(state, x),
                None: () => None(state, unit));

        public Task<S> BiFoldAsync<S>(Task<A> ma, S state, Func<S, A, S> Some, Func<S, Unit, Task<S>> NoneAsync) =>
            MatchAsync(ma,
                Some: x => Some(state, x),
                NoneAsync: () => NoneAsync(state, unit));

        public Task<S> BiFoldAsync<S>(Task<A> ma, S state, Func<S, A, Task<S>> SomeAsync, Func<S, Unit, Task<S>> NoneAsync) =>
            MatchAsync(ma,
                SomeAsync: x => SomeAsync(state, x),
                NoneAsync: () => NoneAsync(state, unit));

        public Task<S> BiFoldBack<S>(Task<A> ma, S state, Func<S, A, S> Some, Func<S, Unit, S> None) =>
            Match(ma,
                Some: x => Some(state, x),
                None: () => None(state, unit));

        public Task<S> BiFoldBackAsync<S>(Task<A> ma, S state, Func<S, A, Task<S>> SomeAsync, Func<S, Unit, S> None) =>
            MatchAsync(ma,
                SomeAsync: x => SomeAsync(state, x),
                None: () => None(state, unit));

        public Task<S> BiFoldBackAsync<S>(Task<A> ma, S state, Func<S, A, S> Some, Func<S, Unit, Task<S>> NoneAsync) =>
            MatchAsync(ma,
                Some: x => Some(state, x),
                NoneAsync: () => NoneAsync(state, unit));

        public Task<S> BiFoldBackAsync<S>(Task<A> ma, S state, Func<S, A, Task<S>> SomeAsync, Func<S, Unit, Task<S>> NoneAsync) =>
            MatchAsync(ma,
                SomeAsync: x => SomeAsync(state, x),
                NoneAsync: () => NoneAsync(state, unit));

        public async Task<B> MatchUnsafe<B>(Task<A> ma, Func<A, B> Some, Func<B> None)
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

        public async Task<B> MatchUnsafeAsync<B>(Task<A> ma, Func<A, Task<B>> SomeAsync, Func<B> None)
        {
            if (ma.IsCanceled || ma.IsFaulted)
            {
                return None();
            }
            try
            {
                var a = await ma;
                return await SomeAsync(a);
            }
            catch (Exception)
            {
                return None();
            }
        }

        public async Task<B> MatchUnsafeAsync<B>(Task<A> ma, Func<A, B> Some, Func<Task<B>> NoneAsync)
        {
            if (ma.IsCanceled || ma.IsFaulted)
            {
                return await NoneAsync();
            }
            try
            {
                var a = await ma;
                return Some(a);
            }
            catch (Exception)
            {
                return await NoneAsync();
            }
        }

        public async Task<B> MatchUnsafeAsync<B>(Task<A> ma, Func<A, Task<B>> SomeAsync, Func<Task<B>> NoneAsync)
        {
            if (ma.IsCanceled || ma.IsFaulted)
            {
                return await NoneAsync();
            }
            try
            {
                var a = await ma;
                return await SomeAsync(a);
            }
            catch (Exception)
            {
                return await NoneAsync();
            }
        }
    }
}
