#nullable enable
using System;

namespace LanguageExt.Transducers;

/// <summary>
/// Applicative apply transducer
/// </summary>
record ApplyTransducer<A, B, C>(Transducer<A, Func<B, C>> FF, Transducer<A, B> FA) : Transducer<A, C>
{
    public override Reducer<A, S> Transform<S>(Reducer<C, S> reduce) =>
        new Reduce<S>(FF, FA, reduce);

    record Reduce<S>(Transducer<A, Func<B, C>> FF, Transducer<A, B> FA, Reducer<C, S> Reducer) : Reducer<A, S>
    {
        public override TResult<S> Run(TState state, S stateValue, A value) =>
            FF.Transform(new Ap<S>(value, FA, Reducer)).Run(state, stateValue, value);
    }
    
    record Ap<S>(A Value, Transducer<A, B> FA, Reducer<C, S> Reducer) : Reducer<Func<B, C>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Func<B, C> f) =>
            FA.Transform(new Ap2<S>(f, Reducer)).Run(state, stateValue, Value);
    }
    
    record Ap2<S>(Func<B, C> F, Reducer<C, S> Reducer) : Reducer<B, S>
    {
        public override TResult<S> Run(TState state, S stateValue, B value) =>
            TResult.Recursive(state, stateValue, F(value), Reducer);
    }

    public override string ToString() =>  
        "apply";
}
/// <summary>
/// Applicative apply transducer
/// </summary>
record ApplyTransducer2<A, B, C>(Transducer<A, Transducer<B, C>> FF, Transducer<A, B> FA) : Transducer<A, C>
{
    public override Reducer<A, S> Transform<S>(Reducer<C, S> reduce) =>
        new Reduce<S>(FF, FA, reduce);

    record Reduce<S>(Transducer<A, Transducer<B, C>> FF, Transducer<A, B> FA, Reducer<C, S> Reducer) : Reducer<A, S>
    {
        public override TResult<S> Run(TState state, S stateValue, A value) =>
            FF.Transform(new Ap<S>(value, FA, Reducer)).Run(state, stateValue, value);
    }
    
    record Ap<S>(A Value, Transducer<A, B> FA, Reducer<C, S> Reducer) : Reducer<Transducer<B, C>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Transducer<B, C> f) =>
            FA.Transform(new Ap2<S>(f, Reducer)).Run(state, stateValue, Value);
    }
    
    record Ap2<S>(Transducer<B, C> F, Reducer<C, S> Reducer) : Reducer<B, S>
    {
        public override TResult<S> Run(TState state, S stateValue, B value) =>
            F.Transform(Reducer).Run(state, stateValue, value);
    }

    public override string ToString() =>  
        "apply";
}
