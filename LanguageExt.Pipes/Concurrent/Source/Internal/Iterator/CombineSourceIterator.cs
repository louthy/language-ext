using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Pipes.Concurrent;

record CombineSourceIterator<A>(Seq<SourceIterator<A>> Sources) : SourceIterator<A>
{
    internal override ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        if (Sources.Count == 0) return new ValueTask<bool>(false);
        if (Sources.Count == 1) return Sources[0].ReadyToRead(token);
        return SourceInternal.ReadyToRead(Sources, token);
    }

    internal override ValueTask<A> ReadValue(CancellationToken token)
    {
        if(token.IsCancellationRequested) return ValueTask.FromException<A>(Errors.Cancelled);
        if (Sources.Count == 0) return ValueTask.FromException<A>(Errors.SourceClosed);
        if (Sources.Count == 1) return Sources[0].ReadValue(token);
        return SourceInternal.Read(Sources, token);
    }
}
