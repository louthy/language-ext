using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Pipes.Concurrent;

record CombineSourceIterator<A>(Seq<SourceIterator<A>> Sources) : SourceIterator<A>
{
    volatile int SourceIndex = -1;
    
    internal override async ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        if(SourceIndex >= Sources.Count) return false;
        if(token.IsCancellationRequested) return false;
        if (Sources.Count == 0) return false;
        if (Sources.Count == 1) return await Sources[0].ReadyToRead(token);
        var source = Sources[SourceIndex];
        var ready  = await source.ReadyToRead(token);
        if (ready) return true;
        Interlocked.Increment(ref SourceIndex);
        return await ReadyToRead(token);
    }

    internal override async ValueTask<A> ReadValue(CancellationToken token)
    {
        if(token.IsCancellationRequested) throw Errors.Cancelled;
        if(SourceIndex    >= Sources.Count) throw Errors.SourceClosed;
        if (Sources.Count == 0) throw Errors.SourceClosed;
        return await Sources[SourceIndex].ReadValue(token);
    }
}
