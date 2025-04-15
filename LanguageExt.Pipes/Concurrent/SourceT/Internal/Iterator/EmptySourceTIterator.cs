using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record EmptySourceTIterator<M, A> : SourceTIterator<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public static readonly SourceTIterator<M, A> Default = new EmptySourceTIterator<M, A>();

    public override ReadResult<M, A> Read() => 
        ReadResult<M>.empty<A>();

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new(false);
}
