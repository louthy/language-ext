namespace LanguageExt;

record IdentityTransducer<A> : Transducer<A, A>
{
    public static readonly Transducer<A, A> Default = new IdentityTransducer<A>();
    
    public override ReducerIO<A, S> Reduce<S>(ReducerIO<A, S> reducer) =>
        reducer;
    
    public override TransducerM<M, A, A> Lift<M>() => 
        new IdentityTransducerM<M, A>();
}
