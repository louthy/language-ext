using LanguageExt.Traits;

namespace LanguageExt;

record FoldablePureSourceT<F, M, A>(K<F, A> Items) : SourceT<M, A>
    where M : MonadIO<M>
    where F : Foldable<F>
{
    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer) =>
        Items.Fold(M.Pure(Reduced.Continue(state)),
                   (ms, x) => ms >> (ns => ns.Continue
                                               ? reducer(ns.Value, M.Pure(x))
                                               : M.Pure(ns)));
}
