using LanguageExt.Traits;

namespace LanguageExt;

record SourcePureSourceT<M, A>(Source<A> Source) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer) =>
        M.LiftIO(Source.Reduce(
                     M.Pure(Reduced.Continue(state)),
                     (ms, a) => ms >> (ns => ns.Continue
                                                 ? reducer(ns.Value, M.Pure(a))
                                                 : M.Pure(ns))))
         .Flatten();
}
