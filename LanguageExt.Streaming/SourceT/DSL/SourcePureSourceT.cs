using LanguageExt.Traits;

namespace LanguageExt;

record SourcePureSourceT<M, A>(Source<A> Source) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, A>, S> reducer) =>
        M.LiftIO(Source.Reduce(M.Pure(state), (ms, a) => ms.Bind(s => reducer(s, M.Pure(a))))).Flatten();
}
