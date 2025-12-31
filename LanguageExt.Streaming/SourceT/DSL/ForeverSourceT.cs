using LanguageExt.Traits;

namespace LanguageExt;

record ForeverSourceT<M, A>(K<M, A> Value) : SourceT<M, A>
    where M : MonadIO<M>
{
    internal override K<M, Reduced<S>> ReduceInternalM<S>(S initialState, ReducerM<M, K<M, A>, S> reducer)
    {
        return Monad.recur(initialState, go);
        K<M, Next<S, Reduced<S>>> go(S state) =>
            reducer(state, Value) *
            (rs => rs switch
                   {
                       { Continue: true, Value: var s } => Next.Loop<S, Reduced<S>>(s),
                       _                                => Next.Done<S, Reduced<S>>(rs)
                   });
    }
}
