using LanguageExt.Traits;

namespace LanguageExt;

record FoldableSourceT<F, M, A>(K<F, K<M, A>> Items) : SourceT<M, A>
    where M : MonadIO<M>
    where F : Foldable<F>
{
    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer) =>
        Items.Fold(M.Pure(Reduced.Continue(state)), 
                   (ms, mx) => ms >> (ns => ns.Continue
                                                ? reducer(ns.Value, mx)
                                                : M.Pure(ns)));
}
