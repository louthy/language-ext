using LanguageExt.Traits;

namespace LanguageExt;

record PureSourceT<M, A>(A Value) : SourceT<M, A>
    where M : MonadIO<M>
{
    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer) => 
        reducer(state, M.Pure(Value));
}
