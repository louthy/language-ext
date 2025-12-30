using LanguageExt.Traits;

namespace LanguageExt;

record SourceSourceT<M, A>(Source<K<M, A>> Source) : SourceT<M, A>
    where M : MonadIO<M>
{
    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer) =>
        M.LiftIO(Source.Reduce(
                     M.Pure(Reduced.Continue(state)),
                     (ms, ma) => ms >> (ns => ns.Continue
                                            ? reducer(ns.Value, ma)
                                            : M.Pure(ns))))
         .Flatten();
}
