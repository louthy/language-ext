using System;
using System.Diagnostics.Contracts;
using LanguageExt;

namespace LanguageExt
{
    public struct TryOptionSomeContext<T, R>
    {
        readonly TryOption<T> option;
        readonly Func<T, R> someHandler;

        internal TryOptionSomeContext(TryOption<T> option, Func<T, R> someHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
        }

        [Pure]
        public TryOptionNoneContext<T, R> None(Func<R> noneHandler) =>
            new TryOptionNoneContext<T, R>(option, someHandler, noneHandler);

        [Pure]
        public TryOptionNoneContext<T, R> None(R noneValue) =>
            new TryOptionNoneContext<T, R>(option, someHandler, () => noneValue);
    }

    public struct TryOptionSomeUnitContext<T>
    {
        readonly TryOption<T> option;
        readonly Action<T> someHandler;

        internal TryOptionSomeUnitContext(TryOption<T> option, Action<T> someHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
        }

        [Pure]
        public TryOptionNoneUnitContext<T> None(Action noneHandler) =>
            new TryOptionNoneUnitContext<T>(option, someHandler, noneHandler);
    }

    public struct TryOptionNoneContext<T, R>
    {
        readonly TryOption<T> option;
        readonly Func<T, R> someHandler;
        readonly Func<R> noneHandler;

        internal TryOptionNoneContext(TryOption<T> option, Func<T, R> someHandler, Func<R> noneHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
            this.noneHandler = noneHandler;
        }

        [Pure]
        public R Fail(Func<Exception, R> failHandler) =>
            option.Match(someHandler, noneHandler, failHandler);

        [Pure]
        public R Fail(R failValue) =>
            option.Match(someHandler, noneHandler, _ => failValue);
    }

    public struct TryOptionNoneUnitContext<T>
    {
        readonly TryOption<T> option;
        readonly Action<T> someHandler;
        readonly Action noneHandler;

        internal TryOptionNoneUnitContext(TryOption<T> option, Action<T> someHandler, Action noneHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
            this.noneHandler = noneHandler;
        }

        public Unit Fail(Action<Exception> failHandler) =>
            option.Match(someHandler, noneHandler, failHandler);
    }
}
