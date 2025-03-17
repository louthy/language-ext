using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Pipes.Concurrent;

record SingletonSourceIterator<A>(A Value) : SourceIterator<A>
{
    volatile int read;

    internal override ValueTask<A> ReadValue(CancellationToken token) =>
        Interlocked.CompareExchange(ref read, 1, 0) == 0
            ? new (Value)
            : ValueTask.FromException<A>(Errors.SourceClosed);

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new(read == 0);
}
