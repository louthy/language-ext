using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record BindSourceTIterator<M, A, B>(SourceTIterator<M, A> SourceT, Func<A, SourceTIterator<M, B>> F) : SourceTIterator<M, B>
    where M : MonadIO<M>, Alternative<M>
{
    public override ReadResult<M, B> Read() =>
        SourceT.Read() switch
        {
            ReadM<M, A> (var ma)      => ReadResult<M>.Iter(ma.Map(F)),
            ReadIter<M, A> (var iter) => ReadResult<M>.Iter(iter.Map(i => i.Bind(F)))
        };
    
    internal override async ValueTask<bool> ReadyToRead(CancellationToken token) =>
        !token.IsCancellationRequested && await SourceT.ReadyToRead(token);
}
