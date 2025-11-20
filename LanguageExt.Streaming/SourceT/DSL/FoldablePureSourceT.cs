using LanguageExt.Traits;

namespace LanguageExt;

record FoldablePureSourceT<F, M, A>(K<F, A> Items) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
    where F : Foldable<F>
{
    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, A>, S> reducer) =>
        Items.Fold(M.Pure(state), (ms, x) => ms.Bind(s => reducer(s, M.Pure(x))));
}
