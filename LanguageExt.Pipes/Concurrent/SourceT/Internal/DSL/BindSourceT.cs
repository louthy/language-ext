using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record BindSourceT<M, A, B>(SourceT<M, A> Source, Func<A, SourceT<M, B>> F) : SourceT<M, B>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, B>, S> reducer) => 
        Source.ReduceM(state, (s, mx) => mx.Bind(x => F(x).ReduceM(s, reducer)));
}
