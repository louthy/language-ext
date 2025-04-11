using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record LiftSourceTIterator<M, A>(K<M, A> Value) : SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    volatile int read;

    public override ReadResult<M, A> Read() =>
        Interlocked.CompareExchange(ref read, 1, 0) == 0
            ? ReadResult<M>.Value(Value)
            : ReadResult<M>.empty<A>();

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new(!token.IsCancellationRequested && read == 0);
}
