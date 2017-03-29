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
        Monad<TryAsync<A>, A>,
        BiFoldable<TryAsync<A>, A, Unit>,
        BiFoldableAsync<TryAsync<A>, A, Unit>
    {
        public static readonly MTryAsync<A> Inst = default(MTryAsync<A>);

        static TryAsync<A> none = () => throw new BottomException();

        [Pure]
        public TryAsync<A> NoneAsync => none;

        [Pure]
        public MB Bind<MONADB, MB, B>(TryAsync<A> ma, Func<A, MB> f) where MONADB : struct, Monad<Unit, Unit, MB, B> =>
            default(MONADB).IdAsync(_ => ma.Try().ContinueWith(task =>
            {
                try
                {
                    return task.IsFaulted
                        ? default(MONADB).Fail(task.Exception)
                        : task.IsCanceled
                            ? default(MONADB).Fail()
                            : task.Result.IsFaulted
                                ? default(MONADB).Fail(task.Result.Exception)
                                : task.Result.IsBottom
                                    ? default(MONADB).Fail()
                                    : f(task.Result.Value);
                }
                catch (Exception e)
                {
                    return default(MONADB).Fail(e);
                }
            }));

        [Pure]
        public TryAsync<A> Id(Func<Unit, TryAsync<A>> ma) =>
            ma(unit);

        [Pure]
        public TryAsync<A> IdAsync(Func<Unit, Task<TryAsync<A>>> ma) =>
            new TryAsync<A>(() =>
            {
                try
                {
                    return from a in ma(unit)
                           let b = a()
                           from c in b
                           select c;
                }
                catch(Exception e)
                {
                    return Task.FromResult(new Result<A>(e));
                }
            });

        [Pure]
        public TryAsync<A> BindReturn(Unit _, TryAsync<A> mb) =>
            mb;

        [Pure]
        public TryAsync<A> Fail(object err) =>
            TryAsync<A>(BottomException.Default);

        [Pure]
        public TryAsync<A> Fail(Exception err = null) =>
            TryAsync<A>(err ?? BottomException.Default);

        [Pure]
        public TryAsync<A> Plus(TryAsync<A> ma, TryAsync<A> mb) => async () =>
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
        /// <param name="x">The bound monad value</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public TryAsync<A> Return(Func<Unit, A> f) =>
            () => Task.FromResult(new Result<A>(f(unit)));

        [Pure]
        public TryAsync<A> Zero() => 
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
        public Func<Unit, S> Fold<S>(TryAsync<A> ma, S state, Func<S, A, S> f) => _ =>
            default(MTryAsync<A>).MatchAsync(ma, x => f(state, x), () => state).Result;

        [Pure]
        public Func<Unit, S> FoldBack<S>(TryAsync<A> ma, S state, Func<S, A, S> f) => _ =>
            default(MTryAsync<A>).MatchAsync(ma, x => f(state, x), () => state).Result;

        [Pure]
        public S BiFold<S>(TryAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
            MatchAsync(ma, x => fa(state, x), () => fb(state, unit)).Result;

        [Pure]
        public S BiFoldBack<S>(TryAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
            MatchAsync(ma, x => fa(state, x), () => fb(state, unit)).Result;

        [Pure]
        public Func<Unit, int> Count(TryAsync<A> ma) => _ =>
            default(MTryAsync<A>).MatchAsync(ma, x => 1, () => 0).Result;

        [Pure]
        public TryAsync<A> Some(A value) =>
            Return(_ => value);

        [Pure]
        public TryAsync<A> Optional(A value) =>
            Return(_ => value);

        [Pure]
        public TryAsync<A> Return(A x) =>
            () => Task.FromResult(new Result<A>(x));

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
            Plus(x, y);

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
    }
}
