using System;
using LanguageExt.Traits;

namespace LanguageExt;

record BindSourceT<M, A, B>(SourceT<M, A> Source, Func<A, SourceT<M, B>> F) : SourceT<M, B>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, B>, S> reducer) => 
        Source.ReduceInternalM(state, (s, mx) => mx.Bind(x => F(x).ReduceInternalM(s, reducer)));
}
