using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record Zip2SourceTIterator<M, A, B>(SourceTIterator<M, A> SourceTA, SourceTIterator<M, B> SourceTB)
    : SourceTIterator<M, (A First, B Second)>
    where M : Monad<M>, Alternative<M>
{
    internal override async ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        if(token.IsCancellationRequested) return false;
        var a = await SourceTA.ReadyToRead(token);
        var b = await SourceTB.ReadyToRead(token);
        return a && b;
    }

    public override K<M, (A First, B Second)> Read() =>
        SourceTA.Read().Zip(SourceTB.Read());
}
