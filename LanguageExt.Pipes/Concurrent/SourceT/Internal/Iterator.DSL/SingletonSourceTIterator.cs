using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record SingletonSourceTIterator<M, A>(A Value) : SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    volatile int read;

    public override K<M, A> Read() =>
        Interlocked.CompareExchange(ref read, 1, 0) == 0
            ? M.Pure(Value)
            : M.Empty<A>();

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new(read == 0);
}
