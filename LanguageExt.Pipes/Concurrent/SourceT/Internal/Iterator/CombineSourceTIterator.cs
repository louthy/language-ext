using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record CombineSourceTIterator<M, A>(Seq<SourceTIterator<M, A>> Sources) : SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        if(token.IsCancellationRequested) return new(false);
        if (Sources.Count == 0) return new(false);
        if (Sources.Count == 1) return Sources[0].ReadyToRead(token);
        return SourceTInternal.ReadyToRead(Sources, token);
    }

    public override ReadResult<M, A> Read()
    {
        if (Sources.Count == 0) return ReadResult<M>.empty<A>();
        if (Sources.Count == 1) return Sources[0].Read();
        return SourceTInternal.Read(Sources);
    }
}
