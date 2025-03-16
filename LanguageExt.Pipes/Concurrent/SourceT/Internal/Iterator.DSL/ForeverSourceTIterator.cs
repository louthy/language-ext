using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record ForeverSourceTIterator<M, A>(K<M, A> Value) : SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    public override K<M, A> Read() =>
        Value;

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new(!token.IsCancellationRequested);
}
