namespace LanguageExt;

record IdentityTransducerM<M, A> : TransducerM<M, A, A>
{
    public static readonly TransducerM<M, A, A> Default = new IdentityTransducerM<M, A>();
    
    public override ReducerM<M, A, S> Reduce<S>(ReducerM<M, A, S> reducer) =>
        reducer;
}
