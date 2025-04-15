using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record ToIOSourceTIterator<M, A>(SourceTIterator<M, A> Source) : SourceTIterator<M, IO<A>>
    where M : MonadIO<M>, Alternative<M>
{
    public override ReadResult<M, IO<A>> Read() =>
        Source.Read() switch
        {
            ReadM<M, A> (var ma)    => ReadResult<M>.Value(ma.ToIO()),
            ReadIter<M, A> (var ma) => ReadResult<M>.Iter(ma.Map(i => (SourceTIterator<M, IO<A>>)new ToIOSourceTIterator<M, A>(i)))
        };

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token) =>
        !token.IsCancellationRequested && await Source.ReadyToRead(token);
}
