using System;
using LanguageExt.Traits;

namespace LanguageExt;

record MapSourceT<M, A, B>(SourceT<M, A> Source, Func<A, B> F) : SourceT<M, B>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, B>, S> reducer) => 
        Source.ReduceInternalM(state, (s, mx) => reducer(s, mx.Map(F)));
}
