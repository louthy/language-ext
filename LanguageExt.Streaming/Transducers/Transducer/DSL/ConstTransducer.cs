namespace LanguageExt;

record ConstTransducer<A, B>(B Value) : Transducer<A, B> 
{
    public override ReducerIO<A, S> Reduce<S>(ReducerIO<B, S> reducer) =>
        (s, _) => reducer(s, Value);
}
