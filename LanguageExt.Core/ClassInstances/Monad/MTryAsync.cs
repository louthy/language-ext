using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct MTryAsync<A> :
        Alternative<TryAsync<A>, Unit, A>,
        OptionalAsync<TryAsync<A>, A>,
        MonadAsync<TryAsync<A>, A>,
        BiFoldableAsync<TryAsync<A>, A, Unit>
    {
        public static readonly MTryAsync<A> Inst = default(MTryAsync<A>);

        static TryAsync<A> none = () => throw new BottomException();

        [Pure]
        public TryAsync<A> NoneAsync => none;

        [Pure]
        public MB BindAsync<MONADB, MB, B>(TryAsync<A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MONADB).RunAsync(async _ => 
            {
                try
                {
                    var ra = await ma.Try();
                    if (ra.IsBottom) return default(MONADB).FailAsync(new TaskCanceledException());
                    if (ra.IsFaulted) return default(MONADB).FailAsync(ra.Exception);
                    return f(ra.Value);
                }
                catch (Exception e)
                {
                    return default(MONADB).FailAsync(e);
                }
            });

        [Pure]
        public MB BindAsync<MONADB, MB, B>(TryAsync<A> ma, Func<A, Task<MB>> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MONADB).RunAsync(async _ =>
            {
                try
                {
                    var ra = await ma.Try();
                    if (ra.IsBottom) return default(MONADB).FailAsync(new TaskCanceledException());
                    if (ra.IsFaulted) return default(MONADB).FailAsync(ra.Exception);
                    return await f(ra.Value);
                }
                catch (Exception e)
                {
                    return default(MONADB).FailAsync(e);
                }
            });

        [Pure]
        public TryAsync<A> RunAsync(Func<Unit, Task<TryAsync<A>>> ma) =>
            new TryAsync<A>(async () => await (await ma(unit)).Try());

        [Pure]
        public TryAsync<A> BindReturnAsync(Unit _, TryAsync<A> mb) =>
            mb;

        [Pure]
        public TryAsync<A> FailAsync(object err = null) =>
            err != null && err is Exception
                ? TryAsync<A>((Exception)err)
                : TryAsync<A>(BottomException.Default);

        [Pure]
        public TryAsync<A> PlusAsync(TryAsync<A> ma, TryAsync<A> mb) => async () =>
        {
            // Run in parallel
            var resA = ma.Try();
            var resB = mb.Try();

            var tasks = Set<OrdTask<Result<A>>, Task<Result<A>>>(resA, resB);

            while (tasks.Count > 0)
            {
                // Return first one that completes
                var completed = await Task.WhenAny(tasks);
                if (!completed.IsFaulted && !completed.Result.IsFaulted) return completed.Result;
                tasks = tasks.Remove(completed);
            }
            if (!resA.IsFaulted) return resA.Result;
            if (!resB.IsFaulted) return resB.Result;
            throw new BottomException();
        };

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <returns>Monad of A</returns>
        [Pure]
        public TryAsync<A> ReturnAsync(Func<Unit, Task<A>> f) =>
            new TryAsync<A>(async () =>
            {
                var a = await f(unit);
                return new Result<A>(a);
            });

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="x">The bound monad value</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public TryAsync<A> ReturnAsync(Task<A> x) =>
            new TryAsync<A>(async () =>
            {
                var a = await x;
                return new Result<A>(a);
            });

        [Pure]
        public TryAsync<A> ZeroAsync() => 
            none;

        [Pure]
        public async Task<B> MatchAsync<B>(TryAsync<A> opt, Func<A, B> Some, Func<B> None)
        {
            try
            {
                var res = await opt.Try();
                if (res.IsFaulted)
                    return None();
                else
                    return Some(res.Value);
            }
            catch
            {
                return None();
            }
        }

        [Pure]
        public async Task<B> MatchAsync<B>(TryAsync<A> opt, Func<A, Task<B>> Some, Func<B> None)
        {
            try
            {
                var res = await opt.Try();
                if (res.IsFaulted)
                    return None();
                else
                    return await Some(res.Value);
            }
            catch
            {
                return None();
            }
        }

        [Pure]
        public async Task<B> MatchAsync<B>(TryAsync<A> opt, Func<A, B> Some, Func<Task<B>> None)
        {
            try
            {
                var res = await opt.Try();
                if (res.IsFaulted)
                    return await None();
                else
                    return Some(res.Value);
            }
            catch
            {
                return await None();
            }
        }

        [Pure]
        public async Task<B> MatchAsync<B>(TryAsync<A> opt, Func<A, Task<B>> Some, Func<Task<B>> None)
        {
            try
            {
                var res = await opt.Try();
                if (res.IsFaulted)
                    return await None();
                else
                    return await Some(res.Value);
            }
            catch
            {
                return await None();
            }
        }

        public Task<Unit> MatchAsync(TryAsync<A> opt, Action<A> Some, Action None) =>
            MatchAsync(opt,
                x  => { Some(x); return unit; },
                () => { None(); return unit; });

        [Pure]
        public async Task<B> MatchUnsafeAsync<B>(TryAsync<A> opt, Func<A, B> Some, Func<B> None)
        {
            try
            {
                var res = await opt.Try();
                if (res.IsFaulted)
                    return None();
                else
                    return Some(res.Value);
            }
            catch
            {
                return None();
            }
        }

        [Pure]
        public async Task<B> MatchUnsafeAsync<B>(TryAsync<A> opt, Func<A, Task<B>> Some, Func<B> None)
        {
            try
            {
                var res = await opt.Try();
                if (res.IsFaulted)
                    return None();
                else
                    return await Some(res.Value);
            }
            catch
            {
                return None();
            }
        }

        [Pure]
        public async Task<B> MatchUnsafeAsync<B>(TryAsync<A> opt, Func<A, B> Some, Func<Task<B>> None)
        {
            try
            {
                var res = await opt.Try();
                if (res.IsFaulted)
                    return await None();
                else
                    return Some(res.Value);
            }
            catch
            {
                return await None();
            }
        }

        [Pure]
        public async Task<B> MatchUnsafeAsync<B>(TryAsync<A> opt, Func<A, Task<B>> Some, Func<Task<B>> None)
        {
            try
            {
                var res = await opt.Try();
                if (res.IsFaulted)
                    return await None();
                else
                    return await Some(res.Value);
            }
            catch
            {
                return await None();
            }
        }

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(TryAsync<A> ma, S state, Func<S, A, S> f) => _ =>
            default(MTryAsync<A>).MatchAsync(ma, x => f(state, x), () => state);

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(TryAsync<A> ma, S state, Func<S, A, Task<S>> f) => _ =>
            default(MTryAsync<A>).MatchAsync(ma, x => f(state, x), () => state);

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(TryAsync<A> ma, S state, Func<S, A, S> f) => _ =>
            default(MTryAsync<A>).MatchAsync(ma, x => f(state, x), () => state);

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(TryAsync<A> ma, S state, Func<S, A, Task<S>> f) => _ =>
            default(MTryAsync<A>).MatchAsync(ma, x => f(state, x), () => state);

        [Pure]
        public Func<Unit, Task<int>> CountAsync(TryAsync<A> ma) => _ =>
            default(MTryAsync<A>).MatchAsync(ma, x => 1, () => 0);

        [Pure]
        public Task<bool> IsNoneAsync(TryAsync<A> opt) =>
            MatchAsync(opt, Some: _ => false, None: () => true);

        [Pure]
        public Task<bool> IsSomeAsync(TryAsync<A> opt) =>
            MatchAsync(opt, Some: _ => true, None: () => false);

        [Pure]
        public Task<bool> IsUnsafeAsync(TryAsync<A> opt) =>
            Task.FromResult(true);

        [Pure]
        public TryAsync<A> SomeAsync(A value) =>
            new TryAsync<A>(() => Task.FromResult(new Result<A>(value)));

        [Pure]
        public TryAsync<A> OptionalAsync(A value) =>
            new TryAsync<A>(() => Task.FromResult(new Result<A>(value)));

        [Pure]
        public TryAsync<A> Empty() =>
            none;

        [Pure]
        public TryAsync<A> Append(TryAsync<A> x, TryAsync<A> y) =>
            PlusAsync(x, y);

        [Pure]
        public Task<S> BiFoldAsync<S>(TryAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
            default(MTryAsync<A>).MatchAsync(
                ma, 
                Some: x  => fa(state, x), 
                None: () => fb(state, unit));

        [Pure]
        public Task<S> BiFoldAsync<S>(TryAsync<A> ma, S state, Func<S, A, Task<S>> fa, Func<S, Unit, S> fb) =>
            default(MTryAsync<A>).MatchAsync(
                ma,
                Some: x => fa(state, x),
                None: () => fb(state, unit));

        [Pure]
        public Task<S> BiFoldAsync<S>(TryAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, Task<S>> fb) =>
            default(MTryAsync<A>).MatchAsync(
                ma,
                Some: x => fa(state, x),
                None: () => fb(state, unit));

        [Pure]
        public Task<S> BiFoldAsync<S>(TryAsync<A> ma, S state, Func<S, A, Task<S>> fa, Func<S, Unit, Task<S>> fb) =>
            default(MTryAsync<A>).MatchAsync(
                ma,
                Some: x => fa(state, x),
                None: () => fb(state, unit));

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
            default(MTryAsync<A>).MatchAsync(
                ma,
                Some: x => fa(state, x),
                None: () => fb(state, unit));

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryAsync<A> ma, S state, Func<S, A, Task<S>> fa, Func<S, Unit, S> fb) =>
            default(MTryAsync<A>).MatchAsync(
                ma,
                Some: x => fa(state, x),
                None: () => fb(state, unit));

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, Task<S>> fb) =>
            default(MTryAsync<A>).MatchAsync(
                ma,
                Some: x => fa(state, x),
                None: () => fb(state, unit));

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryAsync<A> ma, S state, Func<S, A, Task<S>> fa, Func<S, Unit, Task<S>> fb) =>
            default(MTryAsync<A>).MatchAsync(
                ma,
                Some: x => fa(state, x),
                None: () => fb(state, unit));

        [Pure]
        public TryAsync<A> ApplyAsync(Func<A, A, A> f, TryAsync<A> fa, TryAsync<A> fb) => async () =>
        {
            // Run in parallel
            var resA = fa.Try();
            var resB = fb.Try();
            var completed = await Task.WhenAll(resA, resB);

            return !completed[0].IsFaulted && !completed[1].IsFaulted
                ? f(completed[0].Value, completed[1].Value)
                : throw new AggregateException(Seq(completed[0].Exception, completed[1].Exception).Where(e => e != null));
        };
    }
}
