using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record ApplySourceTIterator<M, A, B>(SourceTIterator<M, Func<A, B>> FF, SourceTIterator<M, A> FA) : SourceTIterator<M, B>
    where M : Monad<M>, Alternative<M>
{
    public override ReadResult<M, B> Read() =>
        FA.Read().ApplyBack(FF.Read());

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token) =>
        !token.IsCancellationRequested && 
        await FA.ReadyToRead(token) && 
        await FF.ReadyToRead(token);
}
