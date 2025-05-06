using LanguageExt.Traits;

namespace LanguageExt;

record ToIOSourceT<M, A>(SourceT<M, A> Source) : SourceT<M, IO<A>>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, IO<A>>, S> reducer) => 
        Source.ReduceM(state, (s, mx) => reducer(s, M.ToIO(mx)));
}
