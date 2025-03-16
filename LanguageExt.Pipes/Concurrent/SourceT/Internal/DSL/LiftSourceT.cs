using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record LiftSourceT<M, A>(K<M, A> Value) : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new LiftSourceTIterator<M, A>(Value);

    internal override K<M, S> ReduceInternal<S>(S state, ReducerM<M, A, S> reducer) =>
        Value.Bind(x => reducer(state, x));
}
