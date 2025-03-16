using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record PureSourceT<M, A>(A Value) : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new SingletonSourceTIterator<M, A>(Value);

    internal override K<M, S> ReduceInternal<S>(S state, ReducerM<M, A, S> reducer) =>
        reducer(state, Value);
}
