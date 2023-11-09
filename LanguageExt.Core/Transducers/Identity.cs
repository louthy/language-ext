#nullable enable
namespace LanguageExt.Transducers;

/// <summary>
/// Identity transducer, simply passes the value through 
/// </summary>
record IdentityTransducer<A> : Transducer<A, A>
{
    public static readonly Transducer<A, A> Default = new IdentityTransducer<A>();

    public Reducer<S, A> Transform<S>(Reducer<S, A> reduce) =>
        new Reduce<S>(reduce);

    record Reduce<S>(Reducer<S, A> Reducer) : Reducer<S, A>
    {
        public override TResult<S> Run(TState state, S stateValue, A value) =>
            Reducer.Run(state, stateValue, value);
    }
    
    public Transducer<A, A> Morphism =>
        this;
}
