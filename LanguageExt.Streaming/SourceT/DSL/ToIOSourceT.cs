using LanguageExt.Traits;

namespace LanguageExt;

record ToIOSourceT<M, A>(SourceT<M, A> Source) : SourceT<M, IO<A>>
    where M : MonadIO<M>
{
    internal override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, IO<A>>, S> reducer) => 
        Source.ReduceInternalM(state, (s, mx) => reducer(s, M.ToIOMaybe(mx)));
}
