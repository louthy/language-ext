using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt
{
    public struct TryAsyncSuccContext<A, B>
    {
        readonly TryAsync<A> value;
        readonly Func<A, B> succHandler;

        internal TryAsyncSuccContext(TryAsync<A> value, Func<A, B> succHandler)
        {
            this.value = value;
            this.succHandler = succHandler;
        }

        [Pure]
        public Task<B> Fail(Func<Exception, B> f) =>
            value.Match(succHandler, f);

        [Pure]
        public Task<B> Fail(Func<Exception, Task<B>> f) =>
            value.Match(succHandler, f);

        [Pure]
        public Task<B> Fail(B failValue) =>
            value.Match(succHandler, _ => failValue);

        [Pure]
        public Task<B> Fail(Task<B> failValue) =>
            value.Match(succHandler, _ => failValue);
    }

    public struct TryAsyncSuccUnitContext<A>
    {
        readonly TryAsync<A> value;
        readonly Action<A> succHandler;

        internal TryAsyncSuccUnitContext(TryAsync<A> value, Action<A> succHandler)
        {
            this.value = value;
            this.succHandler = succHandler;
        }

        public Task<Unit> Fail(Action<Exception> f) =>
            value.Match(succHandler, f);
    }
}
