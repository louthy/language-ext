using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record LiftSourceT<M, A>(K<M, A> Value) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, A>, S> reducer) => 
        reducer(state, Value);
}
