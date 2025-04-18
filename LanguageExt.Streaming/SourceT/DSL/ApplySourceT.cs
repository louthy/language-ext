using System;
using LanguageExt.Traits;

namespace LanguageExt;

record ApplySourceT<M, A, B>(SourceT<M, Func<A, B>> FF, SourceT<M, A> FA) : SourceT<M, B>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, B>, S> reducer) =>
        FF.Zip(FA).Map(p => p.First(p.Second)).ReduceM(state, reducer);
}
