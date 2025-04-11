using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record Zip3SourceTIterator<M, A, B, C>(
    SourceTIterator<M, A> SourceTA, 
    SourceTIterator<M, B> SourceTB, 
    SourceTIterator<M, C> SourceTC) :
    SourceTIterator<M, (A First, B Second, C Third)>
    where M : Monad<M>, Alternative<M>
{
    internal override async ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        if(token.IsCancellationRequested) return false;
        var a = await SourceTA.ReadyToRead(token);
        var b = await SourceTB.ReadyToRead(token);
        var c = await SourceTC.ReadyToRead(token);
        return a && b && c;
    }

    public override ReadResult<M, (A First, B Second, C Third)> Read() => 
        throw new NotImplementedException("TODO");
        //SourceTA.Read().Zip(SourceTB.Read(), SourceTC.Read());
}
