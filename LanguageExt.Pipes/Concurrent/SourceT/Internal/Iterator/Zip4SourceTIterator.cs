using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record Zip4SourceTIterator<M, A, B, C, D>(
    SourceTIterator<M, A> SourceTA, 
    SourceTIterator<M, B> SourceTB, 
    SourceTIterator<M, C> SourceTC, 
    SourceTIterator<M, D> SourceTD)
    : SourceTIterator<M, (A First, B Second, C Third, D Fourth)>
    where M : Monad<M>, Alternative<M>
{
    internal override async ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        if(token.IsCancellationRequested) return false;
        var a = await SourceTA.ReadyToRead(token);
        var b = await SourceTB.ReadyToRead(token);
        var c = await SourceTC.ReadyToRead(token);
        var d = await SourceTD.ReadyToRead(token);

        return a && b && c && d;
    }

    public override K<M, (A First, B Second, C Third, D Fourth)> Read() =>
        SourceTA.Read().Zip(SourceTB.Read(), SourceTC.Read(), SourceTD.Read());
}
