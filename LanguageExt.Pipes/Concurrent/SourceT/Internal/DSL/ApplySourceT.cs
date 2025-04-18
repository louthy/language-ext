using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record ApplySourceT<M, A, B>(SourceT<M, Func<A, B>> FF, SourceT<M, A> FA) : SourceT<M, B>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, B>, S> reducer) => 
        FF.ReduceM(state, (s1, mf) => FA.ReduceM(s1, (s2, ma) => reducer(s2, mf.Apply(ma))));
}
