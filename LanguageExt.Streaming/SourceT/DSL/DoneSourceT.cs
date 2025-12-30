using LanguageExt.Traits;

namespace LanguageExt;

record DoneSourceT<M> : SourceT<M, Unit>
    where M : MonadIO<M>
{
    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, Unit>, S> reducer) => 
        M.Pure(Reduced.Done(state));
}
