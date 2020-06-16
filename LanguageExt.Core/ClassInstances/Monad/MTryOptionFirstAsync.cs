using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    public struct MTryOptionFirstAsync<A> :
        Alternative<TryOptionAsync<A>, Unit, A>,
        OptionalAsync<TryOptionAsync<A>, A>,
        OptionalUnsafeAsync<TryOptionAsync<A>, A>,
        MonadAsync<TryOptionAsync<A>, A>,
        FoldableAsync<TryOptionAsync<A>, A>,
        BiFoldableAsync<TryOptionAsync<A>, A, Unit>
    {
        public static readonly MTryOptionFirstAsync<A> Inst = default(MTryOptionFirstAsync<A>);

        static TryOptionAsync<A> none = new TryOptionAsync<A>(() => OptionalResult<A>.None.AsTask());

        [Pure]
        public TryOptionAsync<A> None => none;

        [Pure]
        public MB Bind<MONADB, MB, B>(TryOptionAsync<A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MONADB).RunAsync(async _ =>
            {
                try
                {
                    var ra = await ma.Try();
                    if (ra.IsBottom) return default(MONADB).Fail(new TaskCanceledException());
                    if (ra.IsFaulted) return default(MONADB).Fail(ra.Exception);
                    if (ra.IsFaultedOrNone) return default(MONADB).Zero();
                    return f(ra.Value.Value);
                }
                catch (Exception e)
                {
                    return default(MONADB).Fail(e);
                }
            });

        [Pure]
        public MB BindAsync<MONADB, MB, B>(TryOptionAsync<A> ma, Func<A, Task<MB>> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MONADB).RunAsync(async _ =>
            {
                try
                {
                    var ra = await ma.Try();
                    if (ra.IsBottom) return default(MONADB).Fail(new TaskCanceledException());
                    if (ra.IsFaulted) return default(MONADB).Fail(ra.Exception);
                    if (ra.IsFaultedOrNone) return default(MONADB).Zero();
                    return await f(ra.Value.Value);
                }
                catch (Exception e)
                {
                    return default(MONADB).Fail(e);
                }
            });

        [Pure]
        public TryOptionAsync<A> Fail(object err = null) =>
            Common.Error
                  .Convert<Exception>(err)
                  .Map(f => TryOptionAsync<A>(f))
                  .IfNone(TryOptionAsync<A>(Option<A>.None));            
        
        [Pure]
        public TryOptionAsync<A> Plus(TryOptionAsync<A> ma, TryOptionAsync<A> mb) => async () =>
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
        public TryOptionAsync<A> Zero() =>
            None;

        [Pure]
        public TryOptionAsync<A> RunAsync(Func<Unit, Task<TryOptionAsync<A>>> ma) =>
            new TryOptionAsync<A>(async () => await (await ma(unit)).Try());

        [Pure]
        public TryOptionAsync<A> BindReturn(Unit _, TryOptionAsync<A> mb) =>
            mb;

        [Pure]
        public Func<Unit, Task<S>> Fold<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> f) => _ =>
            ma.Map(a => f(state, a)).IfNoneOrFail(state);

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> f) => _ =>
            ma.MapAsync(a => f(state, a)).IfNoneOrFail(state);

        [Pure]
        public Func<Unit, Task<S>> FoldBack<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> f) => _ =>
            ma.Map(a => f(state, a)).IfNoneOrFail(state);

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> f) => _ =>
            ma.MapAsync(a => f(state, a)).IfNoneOrFail(state);

        [Pure]
        public Func<Unit, Task<int>> Count(TryOptionAsync<A> ma) => _ =>
            ma.Map(a => 1).IfNoneOrFail(0);

        [Pure]
        public TryOptionAsync<A> Empty() =>
            none;

        [Pure]
        public TryOptionAsync<A> Append(TryOptionAsync<A> x, TryOptionAsync<A> y) =>
            Plus(x, y);

        [Pure]
        public Task<S> BiFold<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> Succ, Func<S, Unit, S> Fail) =>
            default(MTryOptionFirstAsync<A>).Match(ma, x => Succ(state, x), () => Fail(state, unit));

        [Pure]
        public Task<S> BiFoldAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> SuccAsync, Func<S, Unit, S> Fail) =>
            default(MTryOptionFirstAsync<A>).MatchAsync(ma, x => SuccAsync(state, x), () => Fail(state, unit));

        [Pure]
        public Task<S> BiFoldAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> Succ, Func<S, Unit, Task<S>> FailAsync) =>
            default(MTryOptionFirstAsync<A>).MatchAsync(ma, x => Succ(state, x), () => FailAsync(state, unit));

        [Pure]
        public Task<S> BiFoldAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> SuccAsync, Func<S, Unit, Task<S>> FailAsync) =>
            default(MTryOptionFirstAsync<A>).MatchAsync(ma, x => SuccAsync(state, x), () => FailAsync(state, unit));

        [Pure]
        public Task<S> BiFoldBack<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> Succ, Func<S, Unit, S> Fail) =>
            default(MTryOptionFirstAsync<A>).Match(ma, x => Succ(state, x), () => Fail(state, unit));

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> SuccAsync, Func<S, Unit, S> Fail) =>
            default(MTryOptionFirstAsync<A>).MatchAsync(ma, x => SuccAsync(state, x), () => Fail(state, unit));

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> Succ, Func<S, Unit, Task<S>> FailAsync) =>
            default(MTryOptionFirstAsync<A>).MatchAsync(ma, x => Succ(state, x), () => FailAsync(state, unit));

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> SuccAsync, Func<S, Unit, Task<S>> FailAsync) =>
            default(MTryOptionFirstAsync<A>).MatchAsync(ma, x => SuccAsync(state, x), () => FailAsync(state, unit));

        [Pure]
        public Task<bool> IsSome(TryOptionAsync<A> opt) =>
            opt.Map(x => true).IfNoneOrFail(false);

        [Pure]
        public Task<bool> IsNone(TryOptionAsync<A> opt) =>
            opt.Map(x => false).IfNoneOrFail(true);

        [Pure]
        public Task<B> Match<B>(TryOptionAsync<A> opt, Func<A, B> Succ, Func<B> Fail) =>
            opt.Match(
                Succ,
                None: Fail,
                Fail: _  => Fail());

        [Pure]
        public Task<B> MatchAsync<B>(TryOptionAsync<A> opt, Func<A, Task<B>> SuccAsync, Func<B> Fail) =>
            opt.MatchAsync(
                SuccAsync,
                None: Fail,
                Fail: _ => Fail());

        [Pure]
        public Task<B> MatchAsync<B>(TryOptionAsync<A> opt, Func<A, B> Succ, Func<Task<B>> FailAsync) =>
            opt.MatchAsync(
                Succ,
                NoneAsync: FailAsync,
                FailAsync: async _ => await FailAsync());

        [Pure]
        public Task<B> MatchAsync<B>(TryOptionAsync<A> opt, Func<A, Task<B>> SuccAsync, Func<Task<B>> FailAsync) =>
            opt.MatchAsync(
                SuccAsync,
                NoneAsync: FailAsync,
                FailAsync: _ => FailAsync());

        [Pure]
        public Task<B> MatchUnsafe<B>(TryOptionAsync<A> opt, Func<A, B> Succ, Func<B> Fail) =>
            opt.MatchUnsafe(
                Succ,
                None: () => Fail(),
                Fail: _ => Fail());

        [Pure]
        public Task<B> MatchUnsafeAsync<B>(TryOptionAsync<A> opt, Func<A, Task<B>> SuccAsync, Func<B> Fail) =>
            opt.MatchAsync(
                SuccAsync,
                None: Fail,
                Fail: _ => Fail());

        [Pure]
        public Task<B> MatchUnsafeAsync<B>(TryOptionAsync<A> opt, Func<A, B> Succ, Func<Task<B>> FailAsync) =>
            opt.MatchAsync(
                Some: Succ,
                NoneAsync: FailAsync,
                FailAsync: _ => FailAsync());

        [Pure]
        public Task<B> MatchUnsafeAsync<B>(TryOptionAsync<A> opt, Func<A, Task<B>> SuccAsync, Func<Task<B>> FailAsync) =>
            opt.MatchAsync(
                SomeAsync: SuccAsync,
                NoneAsync: FailAsync,
                FailAsync: _ => FailAsync());

        [Pure]
        public Task<Unit> Match(TryOptionAsync<A> opt, Action<A> Succ, Action Fail) =>
            opt.Match(
                Succ,
                None: Fail,
                Fail: _  => Fail());

        [Pure]
        public Task<Unit> MatchAsync(TryOptionAsync<A> opt, Func<A, Task> SuccAsync, Action Fail) =>
            opt.MatchAsync(
                SuccAsync: SuccAsync,
                Fail: Fail);

        [Pure]
        public Task<Unit> MatchAsync(TryOptionAsync<A> opt, Action<A> Succ, Func<Task> FailAsync) =>
            opt.MatchAsync(
                Succ: Succ,
                FailAsync: FailAsync);

        [Pure]
        public Task<Unit> MatchAsync(TryOptionAsync<A> opt, Func<A, Task> SuccAsync, Func<Task> FailAsync) =>
            opt.MatchAsync(
                SuccAsync: SuccAsync,
                FailAsync: FailAsync);

        [Pure]
        public TryOptionAsync<A> SomeAsync(Task<A> value) =>
            TryOptionAsync(value);

        [Pure]
        public TryOptionAsync<A> OptionalAsync(Task<A> value) =>
            TryOptionAsync(value);

        [Pure]
        public TryOptionAsync<A> Some(A value) =>
            TryOptionAsync(value);

        [Pure]
        public TryOptionAsync<A> Optional(A value) =>
            TryOptionAsync(value);

        [Pure]
        public TryOptionAsync<A> Apply(Func<A, A, A> f, TryOptionAsync<A> fa, TryOptionAsync<A> fb) => async () =>
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
