namespace LanguageExt;

/// <summary>
/// Identity transducer, simply passes the value through 
/// </summary>
record IdentityTransducer<A> : Transducer<A, A>
{
    public static readonly Transducer<A, A> Default = new IdentityTransducer<A>();

    public override Reducer<A, S> Transform<S>(Reducer<A, S> reduce) =>
        reduce;
                
    public override string ToString() =>  
        "identity";
}
