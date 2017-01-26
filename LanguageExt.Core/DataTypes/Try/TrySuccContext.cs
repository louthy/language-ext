using System;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public struct TrySuccContext<T, R>
    {
        readonly Try<T> value;
        readonly Func<T, R> succHandler;

        internal TrySuccContext(Try<T> value, Func<T, R> succHandler)
        {
            this.value = value;
            this.succHandler = succHandler;
        }

        [Pure]
        public R Fail(Func<Exception, R> failHandler) =>
            value.Match(succHandler, failHandler);

        [Pure]
        public R Fail(R failValue) =>
            value.Match(succHandler, _ => failValue);
    }

    public struct TrySuccUnitContext<T>
    {
        readonly Try<T> value;
        readonly Action<T> succHandler;

        internal TrySuccUnitContext(Try<T> value, Action<T> succHandler)
        {
            this.value = value;
            this.succHandler = succHandler;
        }

        public Unit Fail(Action<Exception> failHandler) =>
            value.Match(succHandler, failHandler);
    }
}
