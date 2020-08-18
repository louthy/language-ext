using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using LanguageExt.Common;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct MTryAsync<A> :
        Alternative<TryAsync<A>, Unit, A>,
        OptionalAsync<TryAsync<A>, A>,
        OptionalUnsafeAsync<TryAsync<A>, A>,
        MonadAsync<TryAsync<A>, A>,
        FoldableAsync<TryAsync<A>, A>,
        BiFoldableAsync<TryAsync<A>, A, Unit>
    {
        public static readonly MTryAsync<A> Inst = default(MTryAsync<A>);

        static TryAsync<A> none = () => throw new BottomException();

        [Pure]
        public TryAsync<A> None => none;

        [Pure]
        public MB Bind<MONADB, MB, B>(TryAsync<A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MTryFirstAsync<A>).Bind<MONADB, MB, B>(ma, f);

        [Pure]
        public MB BindAsync<MONADB, MB, B>(TryAsync<A> ma, Func<A, Task<MB>> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MTryFirstAsync<A>).BindAsync<MONADB, MB, B>(ma, f);

        [Pure]
        public TryAsync<A> RunAsync(Func<Unit, Task<TryAsync<A>>> ma) =>
            default(MTryFirstAsync<A>).RunAsync(ma);

        [Pure]
        public TryAsync<A> BindReturn(Unit _, TryAsync<A> mb) =>
            mb;

        [Pure]
        public TryAsync<A> Fail(object err = null) =>
            default(MTryFirstAsync<A>).Fail(err);

        [Pure]
        public TryAsync<A> Plus(TryAsync<A> ma, TryAsync<A> mb) => async () =>
        {
            var a = await ma.Try().ConfigureAwait(false);
            if (a.IsSuccess) return a;
            var b = await mb.Try().ConfigureAwait(false);
            return b.IsSuccess
                ? b
                : new Result<A>(new AggregateException(a.Exception, b.Exception));
        };

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <returns>Monad of A</returns>
        [Pure]
        public TryAsync<A> ReturnAsync(Func<Unit, Task<A>> f) =>
            default(MTryFirstAsync<A>).ReturnAsync(f);

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="x">The bound monad value</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public TryAsync<A> ReturnAsync(Task<A> x) =>
            default(MTryFirstAsync<A>).ReturnAsync(x);

        [Pure]
        public TryAsync<A> Zero() =>
            default(MTryFirstAsync<A>).Zero();

        [Pure]
        public Task<B> Match<B>(TryAsync<A> opt, Func<A, B> Some, Func<B> None) =>
            default(MTryFirstAsync<A>).Match(opt, Some, None);

        [Pure]
        public Task<B> MatchAsync<B>(TryAsync<A> opt, Func<A, Task<B>> SomeAsync, Func<B> None) =>
            default(MTryFirstAsync<A>).MatchAsync(opt, SomeAsync, None);

        [Pure]
        public Task<B> MatchAsync<B>(TryAsync<A> opt, Func<A, B> Some, Func<Task<B>> NoneAsync) =>
            default(MTryFirstAsync<A>).MatchAsync(opt, Some, NoneAsync);

        [Pure]
        public Task<B> MatchAsync<B>(TryAsync<A> opt, Func<A, Task<B>> SomeAsync, Func<Task<B>> NoneAsync) =>
            default(MTryFirstAsync<A>).MatchAsync(opt, SomeAsync, NoneAsync);

        public Task<Unit> Match(TryAsync<A> opt, Action<A> Some, Action None) =>
            default(MTryFirstAsync<A>).Match(opt, Some, None);

        public Task<Unit> MatchAsync(TryAsync<A> opt, Func<A, Task> SomeAsync, Action None) =>
            default(MTryFirstAsync<A>).MatchAsync(opt, SomeAsync, None);

        public Task<Unit> MatchAsync(TryAsync<A> opt, Action<A> Some, Func<Task> NoneAsync) =>
            default(MTryFirstAsync<A>).MatchAsync(opt, Some, NoneAsync);

        public Task<Unit> MatchAsync(TryAsync<A> opt, Func<A, Task> SomeAsync, Func<Task> NoneAsync) =>
            default(MTryFirstAsync<A>).MatchAsync(opt, SomeAsync, NoneAsync);

        [Pure]
        public Task<B> MatchUnsafe<B>(TryAsync<A> opt, Func<A, B> Some, Func<B> None) =>
            default(MTryFirstAsync<A>).MatchUnsafe(opt, Some, None);

        [Pure]
        public Task<B> MatchUnsafeAsync<B>(TryAsync<A> opt, Func<A, Task<B>> SomeAsync, Func<B> None) =>
            default(MTryFirstAsync<A>).MatchUnsafeAsync(opt, SomeAsync, None);

        [Pure]
        public Task<B> MatchUnsafeAsync<B>(TryAsync<A> opt, Func<A, B> Some, Func<Task<B>> NoneAsync) =>
            default(MTryFirstAsync<A>).MatchUnsafeAsync(opt, Some, NoneAsync);

        [Pure]
        public Task<B> MatchUnsafeAsync<B>(TryAsync<A> opt, Func<A, Task<B>> SomeAsync, Func<Task<B>> NoneAsync) =>
            default(MTryFirstAsync<A>).MatchUnsafeAsync(opt, SomeAsync, NoneAsync);

        [Pure]
        public Func<Unit, Task<S>> Fold<S>(TryAsync<A> ma, S state, Func<S, A, S> f) =>
            default(MTryFirstAsync<A>).Fold(ma, state, f);

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(TryAsync<A> ma, S state, Func<S, A, Task<S>> f) =>
            default(MTryFirstAsync<A>).FoldAsync(ma, state, f);

        [Pure]
        public Func<Unit, Task<S>> FoldBack<S>(TryAsync<A> ma, S state, Func<S, A, S> f) =>
            default(MTryFirstAsync<A>).FoldBack(ma, state, f);

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(TryAsync<A> ma, S state, Func<S, A, Task<S>> f) =>
            default(MTryFirstAsync<A>).FoldBackAsync(ma, state, f);

        [Pure]
        public Func<Unit, Task<int>> Count(TryAsync<A> ma) =>
            default(MTryFirstAsync<A>).Count(ma);

        [Pure]
        public Task<bool> IsNone(TryAsync<A> opt) =>
            default(MTryFirstAsync<A>).IsNone(opt);

        [Pure]
        public Task<bool> IsSome(TryAsync<A> opt) =>
            default(MTryFirstAsync<A>).IsSome(opt);

        [Pure]
        public TryAsync<A> Some(A value) =>
            default(MTryFirstAsync<A>).Some(value);

        [Pure]
        public TryAsync<A> SomeAsync(Task<A> value) =>
            default(MTryFirstAsync<A>).SomeAsync(value);

        [Pure]
        public TryAsync<A> Optional(A value) =>
            default(MTryFirstAsync<A>).Optional(value);

        [Pure]
        public TryAsync<A> OptionalAsync(Task<A> value) =>
            default(MTryFirstAsync<A>).OptionalAsync(value);

        [Pure]
        public TryAsync<A> Empty() =>
            default(MTryFirstAsync<A>).Empty();

        [Pure]
        public TryAsync<A> Append(TryAsync<A> x, TryAsync<A> y) =>
            Plus(x, y);

        [Pure]
        public Task<S> BiFold<S>(TryAsync<A> ma, S state, Func<S, A, S> Some, Func<S, Unit, S> None) =>
            default(MTryFirstAsync<A>).BiFold(ma, state, Some, None);

        [Pure]
        public Task<S> BiFoldAsync<S>(TryAsync<A> ma, S state, Func<S, A, Task<S>> SomeAsync, Func<S, Unit, S> None) =>
            default(MTryFirstAsync<A>).BiFoldAsync(ma, state, SomeAsync, None);

        [Pure]
        public Task<S> BiFoldAsync<S>(TryAsync<A> ma, S state, Func<S, A, S> Some, Func<S, Unit, Task<S>> NoneAsync) =>
            default(MTryFirstAsync<A>).BiFoldAsync(ma, state, Some, NoneAsync);

        [Pure]
        public Task<S> BiFoldAsync<S>(TryAsync<A> ma, S state, Func<S, A, Task<S>> SomeAsync, Func<S, Unit, Task<S>> NoneAsync) =>
            default(MTryFirstAsync<A>).BiFoldAsync(ma, state, SomeAsync, NoneAsync);

        [Pure]
        public Task<S> BiFoldBack<S>(TryAsync<A> ma, S state, Func<S, A, S> Some, Func<S, Unit, S> None) =>
            default(MTryFirstAsync<A>).BiFoldBack(ma, state, Some, None);

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryAsync<A> ma, S state, Func<S, A, Task<S>> SomeAsync, Func<S, Unit, S> None) =>
            default(MTryFirstAsync<A>).BiFoldBackAsync(ma, state, SomeAsync, None);

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryAsync<A> ma, S state, Func<S, A, S> Some, Func<S, Unit, Task<S>> NoneAsync) =>
            default(MTryFirstAsync<A>).BiFoldBackAsync(ma, state, Some, NoneAsync);

        [Pure]
        public Task<S> BiFoldBackAsync<S>(TryAsync<A> ma, S state, Func<S, A, Task<S>> SomeAsync, Func<S, Unit, Task<S>> NoneAsync) =>
            default(MTryFirstAsync<A>).BiFoldBackAsync(ma, state, SomeAsync, NoneAsync);

        [Pure]
        public TryAsync<A> Apply(Func<A, A, A> f, TryAsync<A> fa, TryAsync<A> fb) =>
            default(MTryFirstAsync<A>).Apply(f, fa, fb);
    }
}
