using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record ToIOSourceTIterator<M, A>(SourceTIterator<M, A> Source) : SourceTIterator<M, IO<A>>
    where M : Monad<M>, Alternative<M>
{
    public override K<M, IO<A>> Read() =>
        Source.Read().ToIO();

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token) =>
        !token.IsCancellationRequested && await Source.ReadyToRead(token);
}
