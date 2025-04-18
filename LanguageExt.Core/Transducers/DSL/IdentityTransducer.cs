namespace LanguageExt;

record IdentityTransducer<A> : Transducer<A, A>
{
    public static readonly Transducer<A, A> Default = new IdentityTransducer<A>();
    
    public override ReducerAsync<A, S> Reduce<S>(ReducerAsync<A, S> reducer) =>
        reducer;

    public override ReducerM<M, A, S> ReduceM<M, S>(ReducerM<M, A, S> reducer) =>
        reducer;
}
