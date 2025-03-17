using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Pipes.Concurrent;

record EmptySourceIterator<A> : SourceIterator<A>
{
    public static readonly SourceIterator<A> Default = new EmptySourceIterator<A>();

    internal override ValueTask<A> ReadValue(CancellationToken token) => 
        ValueTask.FromException<A>(Errors.SourceClosed);

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new(false);
}
