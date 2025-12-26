using LanguageExt.Traits;

namespace LanguageExt;

record ForeverSourceT<M, A>(K<M, A> Value) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer) =>
        reducer(state, Value) >>
        (rs => rs switch
               {
                   { Continue: true, Value: var s } => ReduceInternalM(s, reducer),
                   _                                => M.Pure(rs)
               });
}
