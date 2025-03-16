using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

public abstract record SourceTIterator<M, A>
    where M : Monad<M>, Alternative<M>
{
    public abstract K<M, A> Read();
    internal abstract ValueTask<bool> ReadyToRead(CancellationToken token);
}


