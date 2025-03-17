using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Pipes.Concurrent;

record IteratorAsyncSourceIterator<A> : SourceIterator<A>
{
    internal required IteratorAsync<A> Src;

    internal override async ValueTask<A> ReadValue(CancellationToken token)
    {
        if (token.IsCancellationRequested) throw Errors.Cancelled;
        var state = Src.Clone();
        if (await state.IsEmpty) throw Errors.SourceClosed;
        Src = (await state.Tail).Split();
        return await state.Head;
    }

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token) =>
        !await Src.IsEmpty;
}
