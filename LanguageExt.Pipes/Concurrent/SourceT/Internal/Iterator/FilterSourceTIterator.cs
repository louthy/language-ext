using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record FilterSourceTIterator<M, A>(SourceTIterator<M, A> Source, Func<A, bool> Predicate) : SourceTIterator<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public override ReadResult<M, A> Read() =>
        Source.Read() switch
        {
            ReadM<M, A> (var ma) =>
                ReadResult<M>.Iter(ma.Map(x => Predicate(x)
                                                   ? new SingletonSourceTIterator<M, A>(x)
                                                   : EmptySourceTIterator<M, A>.Default)),
            
            ReadIter<M, A> (var miter) =>
                ReadResult<M>.Iter(miter.Map(iter => (SourceTIterator<M, A>)new FilterSourceTIterator<M, A>(iter, Predicate)))
        };

    internal override async ValueTask<bool> ReadyToRead(CancellationToken token)
    {
        if(token.IsCancellationRequested) return false;
        return await Source.ReadyToRead(token);
    }
}
