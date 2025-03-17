using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Pipes.Concurrent;

record IteratorSyncSourceIterator<A> : SourceIterator<A>
{
    internal required Iterator<A> Src;

    internal override ValueTask<A> ReadValue(CancellationToken token)
    {
        if (token.IsCancellationRequested) return ValueTask.FromException<A>(Errors.Cancelled);
        var state = Src.Clone();
        if (state.IsEmpty) return ValueTask.FromException<A>(Errors.SourceClosed);
        Src = state.Tail.Split();
        return new(state.Head);
    }

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new (!Src.IsEmpty);
}
