using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record EmptySourceTIterator<M, A> : SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    public static readonly SourceTIterator<M, A> Default = new EmptySourceTIterator<M, A>();

    public override K<M, A> Read() => 
        M.Empty<A>();

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new(false);
}
