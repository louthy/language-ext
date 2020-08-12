using System;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct MTryFirstAsync<A> :
        Alternative<TryAsync<A>, Unit, A>,
        OptionalAsync<TryAsync<A>, A>,
        OptionalUnsafeAsync<TryAsync<A>, A>,
        MonadAsync<TryAsync<A>, A>,
        FoldableAsync<TryAsync<A>, A>,
        BiFoldableAsync<TryAsync<A>, A, Unit>
    {
        public static readonly MTryFirstAsync<A> Inst = default(MTryFirstAsync<A>);

        static TryAsync<A> none = () => throw new BottomException();

        [Pure]
        public TryAsync<A> None => none;

        [Pure]
        public MB Bind<MONADB, MB, B>(TryAsync<A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MONADB).RunAsync(async _ => 
            {
                try
                {
                    var ra = await ma.Try().ConfigureAwait(false);
                    if (ra.IsBottom) return default(MONADB).Fail(new TaskCanceledException());
                    if (ra.IsFaulted) return default(MONADB).Fail(ra.Exception);
                    return f(ra.Value);
                }
                catch (Exception e)
                {
                    return default(MONADB).Fail(e);
                }
            });

        [Pure]
        public MB BindAsync<MONADB, MB, B>(TryAsync<A> ma, Func<A, Task<MB>> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MONADB).RunAsync(async _ =>
            {
                try
                {
                    var ra = await ma.Try().ConfigureAwait(false);
                    if (ra.IsBottom) return default(MONADB).Fail(new TaskCanceledException());
                    if (ra.IsFaulted) return default(MONADB).Fail(ra.Exception);
                    return await f(ra.Value).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    return default(MONADB).Fail(e);
                }
            });

        [Pure]
        public TryAsync<A> RunAsync(Func<Unit, Task<TryAsync<A>>> ma) =>
            new TryAsync<A>(async () => await (await ma(unit).ConfigureAwait(false)).Try().ConfigureAwait(false));

        [Pure]
        public TryAsync<A> BindReturn(Unit _, TryAsync<A> mb) =>
            mb;

        [Pure]
        public TryAsync<A> Fail(object err = null) =>
            Common.Error
                  .Convert<Exception>(err)
                  .Map(f => TryAsync<A>(f))
                  .IfNone(TryAsync<A>(BottomException.Default));            

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
                var completed = await Task.WhenAny(tasks).ConfigureAwait(false);
                if (!completed.IsFaulted && !completed.Result.IsFaulted) return completed.Result;
                tasks = tasks.Remove(completed);
            }
            if (!resA.IsFaulted) return resA.Result;
            if (!resB.IsFaulted) return resB.Result;
            return new Result<A>(new AggregateException(resA.Exception, resB.Exception));
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
                var a = await f(unit).ConfigureAwait(false);
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
                var a = await x.ConfigureAwait(false);
                return new Result<A>(a);
            });

        [Pure]
        public TryAsync<A> Zero() => 
            none;

        [Pure]
        public async Task<B> Match<B>(TryAsync<A> opt, Func<A, B> Some, Func<B> None)
        {
            try
            {
                var res = await opt.Try().ConfigureAwait(false);
                if (res.IsFaulted)
                    return Check.NullReturn(None());
                else
                    return Check.NullReturn(Some(res.Value));
            }
            catch
            {
                return Check.NullReturn(None());
            }
        }

        [Pure]
        public async Task<B> MatchAsync<B>(TryAsync<A> opt, Func<A, Task<B>> SomeAsync, Func<B> None)
        {
            try
            {
                var res = await opt.Try().ConfigureAwait(false);
                if (res.IsFaulted)
                    return Check.NullReturn(None());
                else
                    return Check.NullReturn(await SomeAsync(res.Value).ConfigureAwait(false));
            }
            catch
            {
                return Check.NullReturn(None());
            }
        }

        [Pure]
        public async Task<B> MatchAsync<B>(TryAsync<A> opt, Func<A, B> Some, Func<Task<B>> NoneAsync)
        {
            try
            {
                var res = await opt.Try().ConfigureAwait(false);
                if (res.IsFaulted)
                    return Check.NullReturn(await NoneAsync().ConfigureAwait(false));
                else
                    return Check.NullReturn(Some(res.Value));
            }
            catch
            {
                return Check.NullReturn(await NoneAsync().ConfigureAwait(false));
            }
        }

        [Pure]
        public async Task<B> MatchAsync<B>(TryAsync<A> opt, Func<A, Task<B>> SomeAsync, Func<Task<B>> NoneAsync)
        {
            try
            {
                var res = await opt.Try().ConfigureAwait(false);
                if (res.IsFaulted)
                    return Check.NullReturn(await NoneAsync().ConfigureAwait(false));
                else
                    return Check.NullReturn(await SomeAsync(res.Value).ConfigureAwait(false));
            }
            catch
            {
                return Check.NullReturn(await NoneAsync().ConfigureAwait(false));
            }
        }

        public Task<Unit> Match(TryAsync<A> opt, Action<A> Some, Action None) =>
            Match(opt,
                x  => { Some(x); return unit; },
                () => { None(); return unit; });

        public Task<Unit> MatchAsync(TryAsync<A> opt, Func<A, Task> SomeAsync, Action None) =>
            MatchAsync(opt,
                async x => { await SomeAsync(x).ConfigureAwait(false); return unit; },
                () => { None(); return unit; });

        public Task<Unit> MatchAsync(TryAsync<A> opt, Action<A> Some, Func<Task> NoneAsync) =>
            MatchAsync(opt,
                x => { Some(x); return unit; },
                async () => { await NoneAsync().ConfigureAwait(false); return unit; });

        public Task<Unit> MatchAsync(TryAsync<A> opt, Func<A, Task> SomeAsync, Func<Task> NoneAsync) =>
            MatchAsync(opt,
                async x => { await SomeAsync(x).ConfigureAwait(false); return unit; },
                async () => { await NoneAsync().ConfigureAwait(false); return unit; });

        [Pure]
        public async Task<B> MatchUnsafe<B>(TryAsync<A> opt, Func<A, B> Some, Func<B> None)
        {
            try
            {
                var res = await opt.Try().ConfigureAwait(false);
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
        public async Task<B> MatchUnsafeAsync<B>(TryAsync<A> opt, Func<A, Task<B>> SomeAsync, Func<B> None)
        {
            try
            {
                var res = await opt.Try().ConfigureAwait(false);
                if (res.IsFaulted)
                    return None();
                else
                    return await SomeAsync(res.Value).ConfigureAwait(false);
            }
            catch
            {
                return None();
            }
        }

        [Pure]
        public async Task<B> MatchUnsafeAsync<B>(TryAsync<A> opt, Func<A, B> Some, Func<Task<B>> NoneAsync)
        {
            try
            {
                var res = await opt.Try().ConfigureAwait(false);
                if (res.IsFaulted)
                    return await NoneAsync().ConfigureAwait(false);
                else
                    return Some(res.Value);
            }
            catch
            {
                return await NoneAsync().ConfigureAwait(false);
            }
        }

        [Pure]
        public async Task<B> MatchUnsafeAsync<B>(TryAsync<A> opt, Func<A, Task<B>> SomeAsync, Func<Task<B>> NoneAsync)
        {
            try
            {
                var res = await opt.Try().ConfigureAwait(false);
                if (res.IsFaulted)
                    return await NoneAsync().ConfigureAwait(false);
                else
                    return await SomeAsync(res.Value).ConfigureAwait(false);
            }
            catch
            {
                return await NoneAsync().ConfigureAwait(false);
            }
        }

        [Pure]
        public Func<Unit, Task<S>> Fold<S>(TryAsync<A> ma, S state, Func<S, A, S> f) => _ =>
            default(MTryFirstAsync<A>).Match(ma, x => f(state, x), () => state);

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(TryAsync<A> ma, S state, Func<S, A, Task<S>> f) => _ =>
            default(MTryFirstAsync<A>).MatchAsync(ma, x => f(state, x), () => state);

        [Pure]
        public Func<Unit, Task<S>> FoldBack<S>(TryAsync<A> ma, S state, Func<S, A, S> f) => _ =>
            default(MTryFirstAsync<A>).Match(ma, x => f(state, x), () => state);

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(TryAsync<A> ma, S state, Func<S, A, Task<S>> f) => _ =>
            default(MTryFirstAsync<A>).MatchAsync(ma, x => f(state, x), () => state);

        [Pure]
        public Func<Unit, Task<int>> Count(TryAsync<A> ma) => _ =>
            default(MTryFirstAsync<A>).Match(ma, x => 1, () => 0);

        [Pure]
        public Task<bool> IsNone(TryAsync<A> opt) =>
            Match(opt, Some: _ => false, None: () => true);

        [Pure]
        public Task<bool> IsSome(TryAsync<A> opt) =>
            Match(opt, Some: _ => true, None: () => false);

        [Pure]
        public TryAsync<A> Some(A value) =>
            new TryAsync<A>(() => new Result<A>(value).AsTask());

        [Pure]
        public TryAsync<A> SomeAsync(Task<A> value) =>
            new TryAsync<A>(async () => new Result<A>(await value.ConfigureAwait(false)));

        [Pure]
        public TryAsync<A> Optional(A value) =>
            new TryAsync<A>(() => new Result<A>(value).AsTask());

        [Pure]
        public TryAsync<A> OptionalAsync(Task<A> value) =>
            new TryAsync<A>(async () => new Result<A>(await value.ConfigureAwait(false)));

        [Pure]
        public TryAsync<A> Empty() =>
            none;

        [Pure]
        public TryAsync<A> Append(TryAsync<A> x, TryAsync<A> y) =>
            Plus(x, y);

        [Pure]
        public Task<S> BiFold<S>(TryAsync<A> ma, S state, Func<S, A, S> Some, Func<S, Unit, S> None) =>
            default(MTryFirstAsync<A>).Match(
                ma, 
                Some: x  => Some(state, x), 
                None: () => None(state, unit));

        [Pure]
        public Task<S> BiFoldAsync<S>(TryAsync<A> ma, S state, Func<S, A, Task<S>> SomeAsync, Func<S, Unit, S> None) =>
            default(MTryFirstAsync<A>).MatchAsync(
                ma,
                SomeAsync: x => SomeAsync(state, x),
                None: () => None(state, unit));

        [Pure]
        public Task<S> BiFoldAsync<S>(TryAsync<A> ma, S state, Func<S, A, S> Some, Func<S, Unit, Task<S>> NoneAsync) =>
            default(MTryFirstAsync<A>).MatchAsync(
                ma,
                Some: x => Some(state, x),
                NoneAsync: () => NoneAsync(state, unit));

        [Pure]
        public Task<S> BiFoldAsync<S>(TryAsync<A> ma, S state, Func<S, A, Task<S>> SomeAsync, Func<S, Unit, Task<S>> NoneAsync) =>
            default(MTryFirstAsync<A>).MatchAsync(
                ma,
                SomeAsync: x => SomeAsync(state, x),
                NoneAsync: () => NoneAsync(state, unit));

        [Pure]
        public Task<S> BiFoldBack<S>(TryAsync<A> ma, S state, Func<S, A, S> Some, Func<S, Unit, S> None) =>
            default(MTryFirstAsync<A>).Match(
                ma,
                Some: x => Some(state, x),
                None: () => None(state, unit));

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryAsync<A> ma, S state, Func<S, A, Task<S>> SomeAsync, Func<S, Unit, S> None) =>
            default(MTryFirstAsync<A>).MatchAsync(
                ma,
                SomeAsync: x => SomeAsync(state, x),
                None: () => None(state, unit));

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryAsync<A> ma, S state, Func<S, A, S> Some, Func<S, Unit, Task<S>> NoneAsync) =>
            default(MTryFirstAsync<A>).MatchAsync(
                ma,
                Some: x => Some(state, x),
                NoneAsync: () => NoneAsync(state, unit));

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryAsync<A> ma, S state, Func<S, A, Task<S>> SomeAsync, Func<S, Unit, Task<S>> NoneAsync) =>
            default(MTryFirstAsync<A>).MatchAsync(
                ma,
                SomeAsync: x => SomeAsync(state, x),
                NoneAsync: () => NoneAsync(state, unit));

        [Pure]
        public TryAsync<A> Apply(Func<A, A, A> f, TryAsync<A> fa, TryAsync<A> fb) => async () =>
        {
            // Run in parallel
            var resA = fa.Try();
            var resB = fb.Try();
            var completed = await Task.WhenAll(resA, resB).ConfigureAwait(false);

            return !completed[0].IsFaulted && !completed[1].IsFaulted
                ? f(completed[0].Value, completed[1].Value)
                : throw new AggregateException(Seq(completed[0].Exception, completed[1].Exception).Where(e => e != null));
        };
    }
}
