using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record FilterSourceT<M, A>(SourceT<M, A> Source, Func<A, bool> Predicate) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, A>, S> reducer) => 
        Source.Reduce(state, (s, x) => Predicate(x) ? reducer(s, M.Pure(x)) : M.Pure(s));
}
