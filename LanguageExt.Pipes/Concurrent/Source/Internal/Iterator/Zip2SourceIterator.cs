using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Pipes.Concurrent;

record Zip2SourceIterator<A, B>(SourceIterator<A> SourceA, SourceIterator<B> SourceB)
    : SourceIterator<(A First, B Second)>
{
    internal override ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        var ta = SourceA.ReadyToRead(token);
        var tb = SourceB.ReadyToRead(token);

        if (ta.IsCompleted && tb.IsCompleted)
            return new(ta.Result && tb.Result);

        return ReadyToReadAsync(ta.AsTask(), tb.AsTask());
    }

    async ValueTask<bool> ReadyToReadAsync(Task<bool> ta, Task<bool> tb)
    {
        await Task.WhenAll(ta, tb);
        return ta.Result && tb.Result;
    }

    internal override ValueTask<(A First, B Second)> ReadValue(CancellationToken token)
    {
        if(token.IsCancellationRequested) return ValueTask.FromException<(A First, B Second)>(Errors.Cancelled);
        var ta = SourceA.ReadValue(token);
        var tb = SourceB.ReadValue(token);

        if (ta.IsCompleted && tb.IsCompleted)
            return new ValueTask<(A First, B Second)>((ta.Result, tb.Result));

        return ReadValueAsync(ta.AsTask(), tb.AsTask());
    }

    async ValueTask<(A First, B Second)> ReadValueAsync(Task<A> ta, Task<B> tb)
    {
        await Task.WhenAll(ta, tb);
        return (ta.Result, tb.Result);
    }
}
