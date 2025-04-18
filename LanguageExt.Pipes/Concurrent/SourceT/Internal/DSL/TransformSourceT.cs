using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record TransformSourceT<M, A, B>(SourceT<M, A> Source, Transducer<K<M, A>, K<M, B>> Transducer) : SourceT<M, B>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, B>, S> reducer) => 
        Source.ReduceM(state, (s, mx) => Transducer.ReduceM(reducer)(s, mx));
}
