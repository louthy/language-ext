using LanguageExt.Traits;

namespace LanguageExt;

record CombineSourceT<M, A>(Seq<SourceT<M, A>> Sources) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer)  
    {
        return go(state, Sources);
        K<M, Reduced<S>> go(S state, Seq<SourceT<M, A>> sources) =>
            sources switch
            {
                []         => M.Empty<Reduced<S>>(),
                [var s]    => s.ReduceInternalM(state, reducer),
                var (h, t) => h.ReduceInternalM(state, reducer).Bind(ns => ns.Continue ? go(ns.Value, t) : M.Pure(ns))
            };
    }
}
