using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Pipes.Concurrent;

record ApplySourceIterator<A, B>(SourceIterator<Func<A, B>> FF, SourceIterator<A> FA) : SourceIterator<B>
{
    internal override ValueTask<B> ReadValue(CancellationToken token)
    {
        if(token.IsCancellationRequested) return ValueTask.FromException<B>(Errors.Cancelled);
        var ta = FA.ReadValue(token);
        var tf = FF.ReadValue(token);
        if(ta.IsCompleted && tf.IsCompleted) return new (tf.Result(ta.Result));
        return ReadValue(tf.AsTask(), ta.AsTask());
    }

    static async ValueTask<B> ReadValue(Task<Func<A, B>> tf, Task<A> ta)
    {
        await Task.WhenAll(tf, ta);
        return tf.Result(ta.Result);
    }

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token) =>
        await FA.ReadyToRead(token) && await FF.ReadyToRead(token);
}
