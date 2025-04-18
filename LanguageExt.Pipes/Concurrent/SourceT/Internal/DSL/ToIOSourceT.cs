using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record ToIOSourceT<M, A>(SourceT<M, A> Source) : SourceT<M, IO<A>>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, IO<A>>, S> reducer) => 
        Source.ReduceM(state, (s, mx) => reducer(s, mx.ToIO()));
}
