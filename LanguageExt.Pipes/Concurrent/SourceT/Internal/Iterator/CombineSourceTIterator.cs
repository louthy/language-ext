using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record CombineSourceTIterator<M, A>(Seq<SourceTIterator<M, A>> Sources) : SourceTIterator<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    volatile int SourceIndex;
    
    internal override async ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        if(SourceIndex >= Sources.Count) return false;
        if(token.IsCancellationRequested) return false;
        if (Sources.Count == 0) return false;
        if (Sources.Count == 1) return await Sources[0].ReadyToRead(token);
        var source = Sources[SourceIndex];
        var ready = await source.ReadyToRead(token);
        if (ready) return true;
        Interlocked.Increment(ref SourceIndex);
        return await ReadyToRead(token);
    }

    public override ReadResult<M, A> Read()
    {
        if (SourceIndex   >= Sources.Count) return ReadResult<M>.empty<A>();
        if (Sources.Count == 0) return ReadResult<M>.empty<A>();
        return Sources[SourceIndex].Read();
    }
}
