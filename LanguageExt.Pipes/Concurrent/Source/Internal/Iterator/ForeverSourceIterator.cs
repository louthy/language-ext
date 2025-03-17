using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt.Pipes.Concurrent;

record ForeverSourceIterator<A>(A Value) : SourceIterator<A>
{
    internal override ValueTask<A> ReadValue(CancellationToken token) =>
        new (Value);

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new(true);
}
