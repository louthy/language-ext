using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Pipes.Concurrent;

record Zip4SourceIterator<A, B, C, D>(SourceIterator<A> SourceA, SourceIterator<B> SourceB, SourceIterator<C> SourceC, SourceIterator<D> SourceD)
    : SourceIterator<(A First, B Second, C Third, D Fourth)>
{
    internal override ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        var ta = SourceA.ReadyToRead(token);
        var tb = SourceB.ReadyToRead(token);
        var tc = SourceC.ReadyToRead(token);
        var td = SourceD.ReadyToRead(token);

        if (ta.IsCompleted && tb.IsCompleted && tc.IsCompleted && td.IsCompleted)
            return new(ta.Result && tb.Result && tc.Result && td.Result);

        return ReadyToReadAsync(ta.AsTask(), tb.AsTask(), tc.AsTask(), td.AsTask());
    }

    async ValueTask<bool> ReadyToReadAsync(Task<bool> ta, Task<bool> tb, Task<bool> tc, Task<bool> td)
    {
        await Task.WhenAll(ta, tb, tc, td);
        return ta.Result && tb.Result && tc.Result && td.Result;
    }

    internal override ValueTask<(A First, B Second, C Third, D Fourth)> ReadValue(CancellationToken token)
    {
        if(token.IsCancellationRequested) return ValueTask.FromException<(A First, B Second, C Third, D Fourth)>(Errors.Cancelled);
        var ta = SourceA.ReadValue(token);
        var tb = SourceB.ReadValue(token);
        var tc = SourceC.ReadValue(token);
        var td = SourceD.ReadValue(token);

        if (ta.IsCompleted && tb.IsCompleted && tc.IsCompleted && td.IsCompleted)
            return new ValueTask<(A First, B Second, C Third, D Fourth)>((ta.Result, tb.Result, tc.Result, td.Result));

        return ReadValueAsync(ta.AsTask(), tb.AsTask(), tc.AsTask(), td.AsTask());
    }

    async ValueTask<(A First, B Second, C Third, D Fourth)> ReadValueAsync(Task<A> ta, Task<B> tb, Task<C> tc, Task<D> td)
    {
        await Task.WhenAll(ta, tb, tc, td);
        return (ta.Result, tb.Result, tc.Result, td.Result);
    }
}
