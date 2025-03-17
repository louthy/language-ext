using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record MapIOSourceTIterator<M, A, B>(SourceTIterator<M, A> Source, Func<IO<A>, IO<B>> F) : SourceTIterator<M, B>
    where M : Monad<M>, Alternative<M>
{
    public override K<M, B> Read() =>
        Source.Read().MapIO(F);

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token) =>
        !token.IsCancellationRequested && await Source.ReadyToRead(token);
}
