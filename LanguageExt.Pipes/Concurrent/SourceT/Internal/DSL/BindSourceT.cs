using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record BindSourceT<M, A, B>(SourceT<M, A> SourceT, Func<A, SourceT<M, B>> F) : SourceT<M, B>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, B> GetIterator() =>
        new BindSourceTIterator<M, A, B>(SourceT.GetIterator(), x => F(x).GetIterator());

    internal override K<M, S> ReduceInternal<S>(S state, ReducerM<M, B, S> reducer) =>
        SourceT.ReduceInternal(state, (s, x) => F(x).ReduceInternal(s, reducer));
}
