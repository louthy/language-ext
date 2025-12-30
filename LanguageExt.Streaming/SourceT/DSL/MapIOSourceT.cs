using System;
using LanguageExt.Traits;

namespace LanguageExt;

record MapIOSourceT<M, A, B>(SourceT<M, A> Source, Func<IO<A>, IO<B>> F) : SourceT<M, B>
    where M : MonadIO<M>
{
    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, B>, S> reducer) =>
        Source.ReduceInternalM(state, (s, ma) => reducer(s, M.MapIOMaybe(ma, F)));
}
