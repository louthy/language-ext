using System;

namespace LanguageExt.Transducers;

record ReleaseTransducer<A> : Transducer<A, Unit>
{
    public static Transducer<A, Unit> Default = new ReleaseTransducer<A>();

    public Transducer<A, Unit> Morphism =>
        this;
    
    public Reducer<A, S> Transform<S>(Reducer<Unit, S> reduce) =>
        new Reduce1<S>(reduce);

    record Reduce1<S>(Reducer<Unit, S> Reduce) : Reducer<A, S>
    {
        public override TResult<S> Run(TState state, S stateValue, A value)
        {
            state.Release(value);
            return Reduce.Run(state, stateValue, default);
        }
    }
}
