using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record CombineSourceT<M, A>(Seq<SourceT<M, A>> Sources) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, A>, S> reducer)  
    {
        return go(state, Sources);
        K<M, S> go(S state, Seq<SourceT<M, A>> sources) =>
            sources switch
            {
                []         => M.Empty<S>(),
                [var s]    => s.ReduceM(state, reducer),
                var (h, t) => h.ReduceM(state, reducer).Bind(ns => go(ns, t))
            };
    }
}
