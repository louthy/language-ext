using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Pipes.Concurrent;

public abstract record SourceIterator<A>
{
    public IO<A> Read() => 
        IO.liftVAsync(
            async e =>
            {
                if (!await ReadyToRead(e.Token)) throw Errors.SourceClosed;
                return await ReadValue(e.Token);
            });

    internal abstract ValueTask<A> ReadValue(CancellationToken token);
    internal abstract ValueTask<bool> ReadyToRead(CancellationToken token);
}
