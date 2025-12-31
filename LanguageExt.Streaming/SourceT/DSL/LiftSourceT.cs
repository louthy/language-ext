using LanguageExt.Traits;

namespace LanguageExt;

record LiftSourceT<M, A>(K<M, A> Value) : SourceT<M, A>
    where M : MonadIO<M>
{
    internal override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer) => 
        reducer(state, Value);
}
