using System;
using LanguageExt.Traits;

namespace LanguageExt;

record MapIOSourceT<M, A, B>(SourceT<M, A> Source, Func<IO<A>, IO<B>> F) : SourceT<M, B>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, B>, S> reducer) =>
        Source.ReduceM(state, (s, ma) => reducer(s, M.MapIO(ma, F)));
}
