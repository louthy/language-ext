using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record Zip2SourceTIterator<M, A, B>(
    SourceTIterator<M, A> SourceA, 
    SourceTIterator<M, B> SourceB)
    : SourceTIterator<M, (A First, B Second)>
    where M : MonadIO<M>, Alternative<M>
{
    internal override async ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        if(token.IsCancellationRequested) return false;
        var a = await SourceA.ReadyToRead(token);
        var b = await SourceB.ReadyToRead(token);
        return a && b;
    }

    public override ReadResult<M, (A First, B Second)> Read()
    {
        var ra = SourceA.Read();
        var rb = SourceB.Read();
        return ra.Zip(rb);
    }
}
