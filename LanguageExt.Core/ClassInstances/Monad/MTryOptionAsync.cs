using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct MTryOptionAsync<A> :
        Alternative<TryOptionAsync<A>, Unit, A>,
        OptionalAsync<TryOptionAsync<A>, A>,
        MonadAsync<TryOptionAsync<A>, A>,
        BiFoldableAsync<TryOptionAsync<A>, A, Unit>
    {
        public static readonly MTryOptionAsync<A> Inst = default(MTryOptionAsync<A>);

        static TryOptionAsync<A> none = new TryOptionAsync<A>(() => Task.FromResult(OptionalResult<A>.None));

        [Pure]
        public TryOptionAsync<A> NoneAsync => none;

        [Pure]
        public MB BindAsync<MONADB, MB, B>(TryOptionAsync<A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MONADB).RunAsync(async _ =>
            {
                try
                {
                    var ra = await ma.Try();
                    if (ra.IsBottom) return default(MONADB).FailAsync(new TaskCanceledException());
                    if (ra.IsFaulted) return default(MONADB).FailAsync(ra.Exception);
                    if (ra.IsFaultedOrNone) return default(MONADB).ZeroAsync();
                    return f(ra.Value.Value);
                }
                catch (Exception e)
                {
                    return default(MONADB).FailAsync(e);
                }
            });

        [Pure]
        public MB BindAsync<MONADB, MB, B>(TryOptionAsync<A> ma, Func<A, Task<MB>> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MONADB).RunAsync(async _ =>
            {
                try
                {
                    var ra = await ma.Try();
                    if (ra.IsBottom) return default(MONADB).FailAsync(new TaskCanceledException());
                    if (ra.IsFaulted) return default(MONADB).FailAsync(ra.Exception);
                    if (ra.IsFaultedOrNone) return default(MONADB).ZeroAsync();
                    return await f(ra.Value.Value);
                }
                catch (Exception e)
                {
                    return default(MONADB).FailAsync(e);
                }
            });

        [Pure]
        public TryOptionAsync<A> FailAsync(object err = null) =>
            err != null && err is Exception
                ? TryOptionAsync<A>((Exception)err)
                : TryOptionAsync(Option<A>.None);

        [Pure]
        public TryOptionAsync<A> PlusAsync(TryOptionAsync<A> ma, TryOptionAsync<A> mb) => async () =>
        {
            // Run in parallel
            var resA = ma.Try();
            var resB = mb.Try();

            var tasks = Set<OrdTask<OptionalResult<A>>, Task<OptionalResult<A>>>(resA, resB);

            while (tasks.Count > 0)
            {
                // Return first one that completes
                var completed = await Task.WhenAny(tasks);
                if (!completed.IsFaulted && !completed.Result.IsFaulted) return completed.Result;
                tasks = tasks.Remove(completed);
            }
            if (!resA.IsFaulted) return resA.Result;
            if (!resB.IsFaulted) return resB.Result;
            return OptionalResult<A>.None;
        };

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="x">The bound monad value</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public TryOptionAsync<A> ReturnAsync(Task<A> x) =>
            new TryOptionAsync<A>(async () => new OptionalResult<A>(await x));

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <returns>Monad of A</returns>
        [Pure]
        public TryOptionAsync<A> ReturnAsync(Func<Unit, Task<A>> f) =>
            new TryOptionAsync<A>(async () => new OptionalResult<A>(await f(unit)));

        [Pure]
        public TryOptionAsync<A> ZeroAsync() =>
            NoneAsync;

        [Pure]
        public TryOptionAsync<A> RunAsync(Func<Unit, Task<TryOptionAsync<A>>> ma) =>
            new TryOptionAsync<A>(async () => await (await ma(unit)).Try());

        [Pure]
        public TryOptionAsync<A> BindReturnAsync(Unit _, TryOptionAsync<A> mb) =>
            mb;

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> f) => _ =>
            ma.Map(a => f(state, a)).IfNoneOrFail(state);

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> f) => _ =>
            ma.Map(a => f(state, a)).IfNoneOrFail(state);

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> f) => _ =>
            ma.Map(a => f(state, a)).IfNoneOrFail(state);

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> f) => _ =>
            ma.Map(a => f(state, a)).IfNoneOrFail(state);

        [Pure]
        public Func<Unit, Task<int>> CountAsync(TryOptionAsync<A> ma) => _ =>
            ma.Map(a => 1).IfNoneOrFail(0);

        [Pure]
        public TryOptionAsync<A> Empty() =>
            none;

        [Pure]
        public TryOptionAsync<A> Append(TryOptionAsync<A> x, TryOptionAsync<A> y) =>
            PlusAsync(x, y);

        [Pure]
        public Task<S> BiFoldAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
            default(MTryOptionAsync<A>).MatchAsync(ma, x => fa(state, x), () => fb(state, unit));

        [Pure]
        public Task<S> BiFoldAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> fa, Func<S, Unit, S> fb) =>
            default(MTryOptionAsync<A>).MatchAsync(ma, x => fa(state, x), () => fb(state, unit));

        [Pure]
        public Task<S> BiFoldAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, Task<S>> fb) =>
            default(MTryOptionAsync<A>).MatchAsync(ma, x => fa(state, x), () => fb(state, unit));

        [Pure]
        public Task<S> BiFoldAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> fa, Func<S, Unit, Task<S>> fb) =>
            default(MTryOptionAsync<A>).MatchAsync(ma, x => fa(state, x), () => fb(state, unit));

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
            default(MTryOptionAsync<A>).MatchAsync(ma, x => fa(state, x), () => fb(state, unit));

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> fa, Func<S, Unit, S> fb) =>
            default(MTryOptionAsync<A>).MatchAsync(ma, x => fa(state, x), () => fb(state, unit));

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, Task<S>> fb) =>
            default(MTryOptionAsync<A>).MatchAsync(ma, x => fa(state, x), () => fb(state, unit));

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> fa, Func<S, Unit, Task<S>> fb) =>
            default(MTryOptionAsync<A>).MatchAsync(ma, x => fa(state, x), () => fb(state, unit));

        public Task<bool> IsUnsafeAsync(TryOptionAsync<A> opt) =>
            Task.FromResult(false);

        public Task<bool> IsSomeAsync(TryOptionAsync<A> opt) =>
            opt.Map(x => true).IfNoneOrFail(false);

        public Task<bool> IsNoneAsync(TryOptionAsync<A> opt) =>
            opt.Map(x => false).IfNoneOrFail(true);

        public Task<B> MatchAsync<B>(TryOptionAsync<A> opt, Func<A, B> Some, Func<B> None) =>
            opt.Match(
                Some,
                None: () => None(),
                Fail: _  => None());

        public Task<B> MatchAsync<B>(TryOptionAsync<A> opt, Func<A, Task<B>> Some, Func<B> None) =>
            opt.Match(
                Some,
                None: () => None(),
                Fail: _ => None());

        public Task<B> MatchAsync<B>(TryOptionAsync<A> opt, Func<A, B> Some, Func<Task<B>> None) =>
            opt.Match(
                Some,
                None: () => None(),
                Fail: _ => None());

        public Task<B> MatchAsync<B>(TryOptionAsync<A> opt, Func<A, Task<B>> Some, Func<Task<B>> None) =>
            opt.Match(
                Some,
                None: () => None(),
                Fail: _ => None());

        public Task<B> MatchUnsafeAsync<B>(TryOptionAsync<A> opt, Func<A, B> Some, Func<B> None) =>
            opt.Match(
                Some,
                None: () => None(),
                Fail: _ => None());

        public Task<B> MatchUnsafeAsync<B>(TryOptionAsync<A> opt, Func<A, Task<B>> Some, Func<B> None) =>
            opt.Match(
                Some,
                None: () => None(),
                Fail: _ => None());

        public Task<B> MatchUnsafeAsync<B>(TryOptionAsync<A> opt, Func<A, B> Some, Func<Task<B>> None) =>
            opt.Match(
                Some,
                None: () => None(),
                Fail: _ => None());

        public Task<B> MatchUnsafeAsync<B>(TryOptionAsync<A> opt, Func<A, Task<B>> Some, Func<Task<B>> None) =>
            opt.Match(
                Some,
                None: () => None(),
                Fail: _ => None());

        public Task<Unit> MatchAsync(TryOptionAsync<A> opt, Action<A> Some, Action None) =>
            opt.Match(
                Some,
                None: () => None(),
                Fail: _  => None());

        public TryOptionAsync<A> SomeAsync(A value) =>
            TryOptionAsync(value);

        public TryOptionAsync<A> OptionalAsync(A value) =>
            TryOptionAsync(value);

        [Pure]
        public TryOptionAsync<A> ApplyAsync(Func<A, A, A> f, TryOptionAsync<A> fa, TryOptionAsync<A> fb) => async () =>
        {
            // Run in parallel
            var resA = fa.Try();
            var resB = fb.Try();
            var completed = await Task.WhenAll(resA, resB);

            return !completed[0].IsFaulted && !completed[1].IsFaulted && completed[0].Value.IsSome && completed[1].Value.IsSome
                ? Option<A>.Some(f(completed[0].Value.Value, completed[1].Value.Value))
                : Option<A>.None; // TODO: Propagate exceptions
        };
    }
}
