namespace LanguageExt;

record ConstTransducer<A, B>(B Value) : Transducer<A, B> 
{
    public override ReducerAsync<A, S> Reduce<S>(ReducerAsync<B, S> reducer) =>
        (s, _) => reducer(s, Value);
}
