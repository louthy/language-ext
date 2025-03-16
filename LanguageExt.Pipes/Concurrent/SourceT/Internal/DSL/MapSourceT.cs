using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record MapSourceT<M, A, B>(SourceT<M, A> SourceT, Func<A, B> F) : SourceT<M, B>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, B> GetIterator() =>
        new MapSourceTIterator<M, A, B>(SourceT.GetIterator(), F);

    internal override K<M, S> ReduceInternal<S>(S state, ReducerM<M, B, S> reducer) =>
        SourceT.ReduceInternal(state, (s, x) => reducer(s, F(x)));
}
