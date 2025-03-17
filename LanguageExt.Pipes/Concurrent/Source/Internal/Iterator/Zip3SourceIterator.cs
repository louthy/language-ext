using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Pipes.Concurrent;

record Zip3SourceIterator<A, B, C>(SourceIterator<A> SourceA, SourceIterator<B> SourceB, SourceIterator<C> SourceC) :
    SourceIterator<(A First, B Second, C Third)>
{
    internal override ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        var ta = SourceA.ReadyToRead(token);
        var tb = SourceB.ReadyToRead(token);
        var tc = SourceC.ReadyToRead(token);

        if (ta.IsCompleted && tb.IsCompleted && tc.IsCompleted)
            return new(ta.Result && tb.Result && tc.Result);

        return ReadyToReadAsync(ta.AsTask(), tb.AsTask(), tc.AsTask());
    }

    async ValueTask<bool> ReadyToReadAsync(Task<bool> ta, Task<bool> tb, Task<bool> tc)
    {
        await Task.WhenAll(ta, tb, tc);
        return ta.Result && tb.Result && tc.Result;
    }

    internal override ValueTask<(A First, B Second, C Third)> ReadValue(CancellationToken token)
    {
        if(token.IsCancellationRequested) return ValueTask.FromException<(A First, B Second, C Third)>(Errors.Cancelled);
        var ta = SourceA.ReadValue(token);
        var tb = SourceB.ReadValue(token);
        var tc = SourceC.ReadValue(token);

        if (ta.IsCompleted && tb.IsCompleted && tc.IsCompleted)
            return new ValueTask<(A First, B Second, C Third)>((ta.Result, tb.Result, tc.Result));

        return ReadValueAsync(ta.AsTask(), tb.AsTask(), tc.AsTask());
    }

    async ValueTask<(A First, B Second, C Third)> ReadValueAsync(Task<A> ta, Task<B> tb, Task<C> tc)
    {
        await Task.WhenAll(ta, tb, tc);
        return (ta.Result, tb.Result, tc.Result);
    }
}
