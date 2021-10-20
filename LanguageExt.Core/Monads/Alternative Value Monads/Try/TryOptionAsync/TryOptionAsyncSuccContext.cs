using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt
{
    public struct TryOptionAsyncSuccContext<A, B>
    {
        readonly TryOptionAsync<A> value;
        readonly Func<A, B> succHandler;
        readonly Func<B> noneHandler;

        internal TryOptionAsyncSuccContext(TryOptionAsync<A> value, Func<A, B> succHandler, Func<B> noneHandler)
        {
            this.value = value;
            this.succHandler = succHandler;
            this.noneHandler = noneHandler;
        }

        [Pure]
        public TryOptionAsyncSuccContext<A, B> None(Func<B> f) =>
            new TryOptionAsyncSuccContext<A, B>(value, succHandler, f);

        [Pure]
        public Task<B> Fail(Func<Exception, B> f) =>
            value.Match(succHandler, noneHandler, f);

        [Pure]
        public Task<B> Fail(Func<Exception, Task<B>> f) =>
            value.MatchAsync(succHandler, noneHandler, f);

        [Pure]
        public Task<B> Fail(B failValue) =>
            value.Match(succHandler, noneHandler, _ => failValue);

        [Pure]
        public Task<B> Fail(Task<B> failValue) =>
            value.MatchAsync(succHandler, noneHandler, _ => failValue);
    }

    public struct TryOptionAsyncSuccContext<A>
    {
        readonly TryOptionAsync<A> value;
        readonly Action<A> succHandler;
        readonly Action noneHandler;

        internal TryOptionAsyncSuccContext(TryOptionAsync<A> value, Action<A> succHandler, Action noneHandler)
        {
            this.value = value;
            this.succHandler = succHandler;
            this.noneHandler = noneHandler;
        }

        public TryOptionAsyncSuccContext<A> None(Action f) =>
            new TryOptionAsyncSuccContext<A>(value, succHandler, f);

        public Task<Unit> Fail(Action<Exception> f) =>
            value.Match(succHandler, noneHandler, f);
    }
}
