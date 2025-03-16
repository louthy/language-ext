using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record LiftSourceTIterator<M, A>(K<M, A> Value) : SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    volatile int read;

    public override K<M, A> Read() =>
        Interlocked.CompareExchange(ref read, 1, 0) == 0
            ? Value
            : M.Empty<A>();

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new(read == 0);
}
