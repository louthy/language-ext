using LanguageExt.Traits;

namespace LanguageExt;

record FoldableSourceT<F, M, A>(K<F, K<M, A>> Items) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
    where F : Foldable<F>
{
    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, A>, S> reducer) =>
        Items.Fold(M.Pure(state), (ms, mx) => ms.Bind(s => reducer(s, mx)));
}
