using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record ForeverSourceTIterator<M, A>(K<M, A> Value) : SourceTIterator<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public override ReadResult<M, A> Read() =>
        ReadResult<M>.Value(Value);

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new(!token.IsCancellationRequested);
}
