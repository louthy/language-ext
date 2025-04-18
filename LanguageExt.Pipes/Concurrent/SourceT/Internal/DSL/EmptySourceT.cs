using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record EmptySourceT<M, A> : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public static readonly SourceT<M, A> Default = new EmptySourceT<M, A>();

    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, A>, S> reducer) => 
        M.Pure(state);
}
