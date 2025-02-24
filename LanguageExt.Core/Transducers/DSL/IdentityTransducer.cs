namespace LanguageExt;

record IdentityTransducer<A> : Transducer<A, A>
{
    public static readonly Transducer<A, A> Default = new IdentityTransducer<A>();
    
    public override Reducer<A, S> Reduce<S>(Reducer<A, S> reducer) =>
        reducer;
}
