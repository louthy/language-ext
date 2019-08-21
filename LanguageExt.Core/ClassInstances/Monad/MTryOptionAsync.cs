using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    public struct MTryOptionAsync<A> :
        Alternative<TryOptionAsync<A>, Unit, A>,
        OptionalAsync<TryOptionAsync<A>, A>,
        OptionalUnsafeAsync<TryOptionAsync<A>, A>,
        MonadAsync<TryOptionAsync<A>, A>,
        FoldableAsync<TryOptionAsync<A>, A>,
        BiFoldableAsync<TryOptionAsync<A>, A, Unit>
    {
        public static readonly MTryOptionAsync<A> Inst = default(MTryOptionAsync<A>);

        static TryOptionAsync<A> none = new TryOptionAsync<A>(() => OptionalResult<A>.None.AsTask());

        [Pure]
        public TryOptionAsync<A> None => none;

        [Pure]
        public MB Bind<MONADB, MB, B>(TryOptionAsync<A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MTryOptionFirstAsync<A>).Bind<MONADB, MB, B>(ma, f);

        [Pure]
        public MB BindAsync<MONADB, MB, B>(TryOptionAsync<A> ma, Func<A, Task<MB>> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MTryOptionFirstAsync<A>).BindAsync<MONADB, MB, B>(ma, f);

        [Pure]
        public TryOptionAsync<A> Fail(object err = null) =>
            default(MTryOptionFirstAsync<A>).Fail(err);

        [Pure]
        public TryOptionAsync<A> Plus(TryOptionAsync<A> ma, TryOptionAsync<A> mb) => async () =>
        {
            var a = await ma.Try();
            if (!a.IsFaultedOrNone) return a;
            var b = await mb.Try();
            return b.IsFaulted && a.IsFaulted
                ? new OptionalResult<A>(new AggregateException(a.Exception, b.Exception))
                : b;
        };

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="x">The bound monad value</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public TryOptionAsync<A> ReturnAsync(Task<A> x) =>
            default(MTryOptionFirstAsync<A>).ReturnAsync(x);

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <returns>Monad of A</returns>
        [Pure]
        public TryOptionAsync<A> ReturnAsync(Func<Unit, Task<A>> f) =>
            default(MTryOptionFirstAsync<A>).ReturnAsync(f);

        [Pure]
        public TryOptionAsync<A> Zero() =>
            default(MTryOptionFirstAsync<A>).Zero();

        [Pure]
        public TryOptionAsync<A> RunAsync(Func<Unit, Task<TryOptionAsync<A>>> ma) =>
            default(MTryOptionFirstAsync<A>).RunAsync(ma);

        [Pure]
        public TryOptionAsync<A> BindReturn(Unit _, TryOptionAsync<A> mb) =>
            default(MTryOptionFirstAsync<A>).BindReturn(_, mb);

        [Pure]
        public Func<Unit, Task<S>> Fold<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> f) =>
            default(MTryOptionFirstAsync<A>).Fold(ma, state, f);

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> f) =>
            default(MTryOptionFirstAsync<A>).FoldAsync(ma, state, f);

        [Pure]
        public Func<Unit, Task<S>> FoldBack<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> f) =>
            default(MTryOptionFirstAsync<A>).FoldBack(ma, state, f);

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> f) =>
            default(MTryOptionFirstAsync<A>).FoldBackAsync(ma, state, f);

        [Pure]
        public Func<Unit, Task<int>> Count(TryOptionAsync<A> ma) =>
            default(MTryOptionFirstAsync<A>).Count(ma);

        [Pure]
        public TryOptionAsync<A> Empty() =>
            default(MTryOptionFirstAsync<A>).Empty();

        [Pure]
        public TryOptionAsync<A> Append(TryOptionAsync<A> x, TryOptionAsync<A> y) =>
            Plus(x, y);

        [Pure]
        public Task<S> BiFold<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> Succ, Func<S, Unit, S> Fail) =>
            default(MTryOptionFirstAsync<A>).BiFold(ma, state, Succ, Fail);

        [Pure]
        public Task<S> BiFoldAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> SuccAsync, Func<S, Unit, S> Fail) =>
            default(MTryOptionFirstAsync<A>).BiFoldAsync(ma, state, SuccAsync, Fail);

        [Pure]
        public Task<S> BiFoldAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> Succ, Func<S, Unit, Task<S>> FailAsync) =>
            default(MTryOptionFirstAsync<A>).BiFoldAsync(ma, state, Succ, FailAsync);

        [Pure]
        public Task<S> BiFoldAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> SuccAsync, Func<S, Unit, Task<S>> FailAsync) =>
            default(MTryOptionFirstAsync<A>).BiFoldAsync(ma, state, SuccAsync, FailAsync);

        [Pure]
        public Task<S> BiFoldBack<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> Succ, Func<S, Unit, S> Fail) =>
            default(MTryOptionFirstAsync<A>).BiFoldBack(ma, state, Succ, Fail);

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> SuccAsync, Func<S, Unit, S> Fail) =>
            default(MTryOptionFirstAsync<A>).BiFoldBackAsync(ma, state, SuccAsync, Fail);

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, S> Succ, Func<S, Unit, Task<S>> FailAsync) =>
            default(MTryOptionFirstAsync<A>).BiFoldBackAsync(ma, state, Succ, FailAsync);

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryOptionAsync<A> ma, S state, Func<S, A, Task<S>> SuccAsync, Func<S, Unit, Task<S>> FailAsync) =>
            default(MTryOptionFirstAsync<A>).BiFoldBackAsync(ma, state, SuccAsync, FailAsync);

        [Pure]
        public Task<bool> IsSome(TryOptionAsync<A> opt) =>
            default(MTryOptionFirstAsync<A>).IsSome(opt);

        [Pure]
        public Task<bool> IsNone(TryOptionAsync<A> opt) =>
            default(MTryOptionFirstAsync<A>).IsNone(opt);

        [Pure]
        public Task<B> Match<B>(TryOptionAsync<A> opt, Func<A, B> Succ, Func<B> Fail) =>
            default(MTryOptionFirstAsync<A>).Match(opt, Succ, Fail);

        [Pure]
        public Task<B> MatchAsync<B>(TryOptionAsync<A> opt, Func<A, Task<B>> SuccAsync, Func<B> Fail) =>
            default(MTryOptionFirstAsync<A>).MatchAsync(opt, SuccAsync, Fail);

        [Pure]
        public Task<B> MatchAsync<B>(TryOptionAsync<A> opt, Func<A, B> Succ, Func<Task<B>> FailAsync) =>
            default(MTryOptionFirstAsync<A>).MatchAsync(opt, Succ, FailAsync);

        [Pure]
        public Task<B> MatchAsync<B>(TryOptionAsync<A> opt, Func<A, Task<B>> SuccAsync, Func<Task<B>> FailAsync) =>
            default(MTryOptionFirstAsync<A>).MatchAsync(opt, SuccAsync, FailAsync);

        [Pure]
        public Task<B> MatchUnsafe<B>(TryOptionAsync<A> opt, Func<A, B> Succ, Func<B> Fail) =>
            default(MTryOptionFirstAsync<A>).MatchUnsafe(opt, Succ, Fail);

        [Pure]
        public Task<B> MatchUnsafeAsync<B>(TryOptionAsync<A> opt, Func<A, Task<B>> SuccAsync, Func<B> Fail) =>
            default(MTryOptionFirstAsync<A>).MatchUnsafeAsync(opt, SuccAsync, Fail);

        [Pure]
        public Task<B> MatchUnsafeAsync<B>(TryOptionAsync<A> opt, Func<A, B> Succ, Func<Task<B>> FailAsync) =>
            default(MTryOptionFirstAsync<A>).MatchUnsafeAsync(opt, Succ, FailAsync);

        [Pure]
        public Task<B> MatchUnsafeAsync<B>(TryOptionAsync<A> opt, Func<A, Task<B>> SuccAsync, Func<Task<B>> FailAsync) =>
            default(MTryOptionFirstAsync<A>).MatchUnsafeAsync(opt, SuccAsync, FailAsync);

        [Pure]
        public Task<Unit> Match(TryOptionAsync<A> opt, Action<A> Succ, Action Fail) =>
            default(MTryOptionFirstAsync<A>).Match(opt, Succ, Fail);

        [Pure]
        public Task<Unit> MatchAsync(TryOptionAsync<A> opt, Func<A, Task> SuccAsync, Action Fail) =>
            default(MTryOptionFirstAsync<A>).MatchAsync(opt, SuccAsync, Fail);

        [Pure]
        public Task<Unit> MatchAsync(TryOptionAsync<A> opt, Action<A> Succ, Func<Task> FailAsync) =>
            default(MTryOptionFirstAsync<A>).MatchAsync(opt, Succ, FailAsync);

        [Pure]
        public Task<Unit> MatchAsync(TryOptionAsync<A> opt, Func<A, Task> SuccAsync, Func<Task> FailAsync) =>
            default(MTryOptionFirstAsync<A>).MatchAsync(opt, SuccAsync, FailAsync);

        [Pure]
        public TryOptionAsync<A> SomeAsync(Task<A> value) =>
            default(MTryOptionFirstAsync<A>).SomeAsync(value);

        [Pure]
        public TryOptionAsync<A> OptionalAsync(Task<A> value) =>
            default(MTryOptionFirstAsync<A>).OptionalAsync(value);

        [Pure]
        public TryOptionAsync<A> Some(A value) =>
            default(MTryOptionFirstAsync<A>).Some(value);

        [Pure]
        public TryOptionAsync<A> Optional(A value) =>
            default(MTryOptionFirstAsync<A>).Optional(value);

        [Pure]
        public TryOptionAsync<A> Apply(Func<A, A, A> f, TryOptionAsync<A> fa, TryOptionAsync<A> fb) =>
            default(MTryOptionFirstAsync<A>).Apply(f, fa, fb);
    }
}
