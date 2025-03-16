using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record ApplySourceTIterator<M, A, B>(SourceTIterator<M, Func<A, B>> FF, SourceTIterator<M, A> FA) : SourceTIterator<M, B>
    where M : Monad<M>, Alternative<M>
{
    public override K<M, B> Read() =>
        FF.Read().Apply(FA.Read());

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token) =>
        await FA.ReadyToRead(token) && await FF.ReadyToRead(token);
}
