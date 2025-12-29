namespace LanguageExt;

record PureSource<A>(A Value) : Source<A>
{
    internal override IO<Reduced<S>> ReduceInternal<S>(S state, ReducerIO<A, S> reducer) => 
        reducer(state, Value);
}
