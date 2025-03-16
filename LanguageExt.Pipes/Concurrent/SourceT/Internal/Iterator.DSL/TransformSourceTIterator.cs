using System.Threading;
using LanguageExt.Traits;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record TransformSourceTIterator<M, A, B>(SourceTIterator<M, A> SourceT, Transducer<A, B> Transducer) 
    : SourceTIterator<M, B>
    where M : Monad<M>, Alternative<M>
{
    public override K<M, B> Read()
    {
        var mvalue = SourceT.Read();
        var result = mvalue.Bind(v => Transducer.ReduceM<M, Option<B>>(reduce)(None, v))
                           .Bind(mb => mb.IsSome ? M.Pure((B)mb) : M.Empty<B>()); 
        return result;
        
        static K<M, Option<B>> reduce(Option<B> _, B x) => 
            M.Pure(Optional(x));
    }
    
    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        SourceT.ReadyToRead(token);
}
