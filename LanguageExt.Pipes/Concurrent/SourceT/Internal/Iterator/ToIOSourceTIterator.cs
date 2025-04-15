using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record MapIOSourceTIterator<M, A, B>(SourceTIterator<M, A> Source, Func<IO<A>, IO<B>> F) : SourceTIterator<M, B>
    where M : MonadIO<M>, Alternative<M>
{
    public override ReadResult<M, B> Read() =>
        Source.Read() switch
        {
            ReadM<M, A> (var ma)    => ReadResult<M>.Value(ma.MapIO(F)),
            ReadIter<M, A> (var ma) => ReadResult<M>.Iter(ma.Map(i => (SourceTIterator<M, B>)new MapIOSourceTIterator<M, A, B>(i, F)))
        };

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token) =>
        !token.IsCancellationRequested && await Source.ReadyToRead(token);
}
