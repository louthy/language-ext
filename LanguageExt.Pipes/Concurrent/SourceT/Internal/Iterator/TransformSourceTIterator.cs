using System;
using System.Threading;
using LanguageExt.Traits;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record TransformSourceTIterator<M, A, B>(SourceTIterator<M, A> Source, Transducer<A, B> Transducer) 
    : SourceTIterator<M, B>
    where M : MonadIO<M>, Alternative<M>
{
    public override ReadResult<M, B> Read()
    {
        return Source.Read() switch
               {
                   ReadM<M, A> (var ma) =>
                       ReadResult<M>.Value(
                           ma.Bind(
                               x => Transducer.ReduceM<M, Option<B>>(reduce)(None, x)
                                              .Bind(mb => mb.IsSome ? M.Pure((B)mb) : M.Empty<B>()))),

                   ReadIter<M, A> (var miter) =>
                       ReadResult<M>.Iter(
                           miter.Map(
                               iter => (SourceTIterator<M, B>)new TransformSourceTIterator<M, A, B>(iter, Transducer)))
               };

        static K<M, Option<B>> reduce(Option<B> _, B x) =>
            M.Pure(Optional(x));
    }
    
    internal override async ValueTask<bool> ReadyToRead(CancellationToken token) =>
        !token.IsCancellationRequested && await Source.ReadyToRead(token);
}
