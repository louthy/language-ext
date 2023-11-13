using System;

namespace LanguageExt.Transducers;

record UseTransducer1<A, B>(Transducer<A, B> F, Func<B, Unit> CleanUp) : Transducer<A, B>
{
    public Transducer<A, B> Morphism =>
        this;
    
    public Reducer<A, S> Transform<S>(Reducer<B, S> reduce) =>
        F.Transform(new Reduce1<S>(CleanUp, reduce));

    record Reduce1<S>(Func<B, Unit> CleanUp, Reducer<B, S> Reduce) : Reducer<B, S>
    {
        public override TResult<S> Run(TState state, S stateValue, B value)
        {
            state.Using(value, CleanUp);
            return Reduce.Run(state, stateValue, value);
        }
    }
}

record UseTransducer2<A, B>(Transducer<A, B> F) : Transducer<A, B> where B : IDisposable
{
    public Transducer<A, B> Morphism =>
        this;
    
    public Reducer<A, S> Transform<S>(Reducer<B, S> reduce) =>
        F.Transform(new Reduce1<S>(reduce));

    record Reduce1<S>(Reducer<B, S> Reduce) : Reducer<B, S>
    {
        public override TResult<S> Run(TState state, S stateValue, B value)
        {
            state.Using(value);
            return Reduce.Run(state, stateValue, value);
        }
    }
}
