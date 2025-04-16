using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record Zip4SourceTIterator<M, A, B, C, D>(
    SourceTIterator<M, A> SourceA, 
    SourceTIterator<M, B> SourceB, 
    SourceTIterator<M, C> SourceC, 
    SourceTIterator<M, D> SourceD)
    : SourceTIterator<M, (A First, B Second, C Third, D Fourth)>
    where M : MonadIO<M>, Alternative<M>
{
    internal override async ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        if(token.IsCancellationRequested) return false;
        var a = await SourceA.ReadyToRead(token);
        var b = await SourceB.ReadyToRead(token);
        var c = await SourceC.ReadyToRead(token);
        var d = await SourceD.ReadyToRead(token);

        return a && b && c && d;
    }

    public override ReadResult<M, (A First, B Second, C Third, D Fourth)> Read()
    {
        var ra = SourceA.Read();
        var rb = SourceB.Read();
        var rc = SourceC.Read();
        var rd = SourceD.Read();
        return ra.Zip(rb, rc, rd);
    }
}
