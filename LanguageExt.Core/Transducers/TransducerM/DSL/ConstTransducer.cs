namespace LanguageExt;

record ConstTransducerM<M, A, B>(B Value) : TransducerM<M, A, B> 
{
    public override ReducerM<M, A, S> Reduce<S>(ReducerM<M, B, S> reducer) => 
        (s, _) => reducer(s, Value);
}
