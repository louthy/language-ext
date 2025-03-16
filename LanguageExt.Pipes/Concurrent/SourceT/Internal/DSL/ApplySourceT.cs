using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record ApplySourceT<M, A, B>(SourceT<M, Func<A, B>> FF, SourceT<M, A> FA) : SourceT<M, B>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, B> GetIterator() =>
        new ApplySourceTIterator<M, A, B>(FF.GetIterator(), FA.GetIterator());

    internal override K<M, S> ReduceInternal<S>(S state, ReducerM<M, B, S> reducer) =>
        FF.ReduceInternal(state, (s, f) => FA.ReduceInternal(s, (s1, x) => reducer(s1, f(x))));
}
