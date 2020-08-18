using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LanguageExt
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct OptionAsyncAwaiter<A> : INotifyCompletion
    {
        readonly OptionAsync<A> ma;
        readonly TaskAwaiter<(bool IsSome, A Value)> awaiter;

        internal OptionAsyncAwaiter(OptionAsync<A> ma) =>
            (this.ma, this.awaiter) = (ma, ma.Data.GetAwaiter());

        public bool IsCompleted =>
            awaiter.IsCompleted;

        public A GetResult()
        {
            var (isSome, value) = awaiter.GetResult();
            return isSome
                ? value
                : throw new ValueIsNoneException();
        }

        public void OnCompleted(Action completion) =>
            awaiter.OnCompleted(completion);
    }
}
