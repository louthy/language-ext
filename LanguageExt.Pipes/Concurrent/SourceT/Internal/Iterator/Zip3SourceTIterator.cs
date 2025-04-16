using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record Zip3SourceTIterator<M, A, B, C>(
    SourceTIterator<M, A> SourceA, 
    SourceTIterator<M, B> SourceB, 
    SourceTIterator<M, C> SourceC) :
    SourceTIterator<M, (A First, B Second, C Third)>
    where M : MonadIO<M>, Alternative<M>
{
    internal override async ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        if(token.IsCancellationRequested) return false;
        var a = await SourceA.ReadyToRead(token);
        var b = await SourceB.ReadyToRead(token);
        var c = await SourceC.ReadyToRead(token);
        return a && b && c;
    }

    public override ReadResult<M, (A First, B Second, C Third)> Read()  
    {
        var ra = SourceA.Read();
        var rb = SourceB.Read();
        var rc = SourceC.Read();
        return ra.Zip(rb, rc);
    }
}
