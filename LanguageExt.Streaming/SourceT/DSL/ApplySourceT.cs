using System;
using LanguageExt.Traits;

namespace LanguageExt;

record ApplySourceT<M, A, B>(SourceT<M, Func<A, B>> FF, SourceT<M, A> FA) : SourceT<M, B>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, B>, S> reducer) =>
        new Zip2SourceT<M, Func<A, B>, A>(FF, FA).Map(p => p.First(p.Second)).ReduceInternalM(state, reducer);
}

record ApplySourceT2<M, A, B>(SourceT<M, Func<A, B>> FF, Memo<SourceT<M>, A> FA) : SourceT<M, B>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, B>, S> reducer) =>
        new Zip2SourceT<M, Func<A, B>, A>(FF, FA.Value.As()).Map(p => p.First(p.Second)).ReduceInternalM(state, reducer);
}
