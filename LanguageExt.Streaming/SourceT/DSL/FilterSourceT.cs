using System;
using LanguageExt.Traits;

namespace LanguageExt;

record FilterSourceT<M, A>(SourceT<M, A> Source, Func<A, bool> Predicate) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer) => 
        Source.ReduceInternal(state, 
                              (s, x) => Predicate(x) 
                                            ? reducer(s, M.Pure(x)) 
                                            : M.Pure(Reduced.Continue(s)));
}
