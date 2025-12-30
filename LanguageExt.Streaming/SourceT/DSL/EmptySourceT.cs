using LanguageExt.Traits;

namespace LanguageExt;

record EmptySourceT<M, A> : SourceT<M, A>
    where M : MonadIO<M>
{
    public static readonly SourceT<M, A> Default = new EmptySourceT<M, A>();

    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer) => 
        M.Pure(Reduced.Done(state));
}
