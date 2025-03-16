using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record MapSourceTIterator<M, A, B>(SourceTIterator<M, A> Source, Func<A, B> F) : SourceTIterator<M, B>
    where M : Monad<M>, Alternative<M>
{
    public override K<M, B> Read() => 
        Source.Read().Map(F);

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token) => 
        !token.IsCancellationRequested && await Source.ReadyToRead(token);
}
