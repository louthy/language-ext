#nullable enable
using System;

namespace LanguageExt.Transducers;

/// <summary>
/// Applicative apply transducer
/// </summary>
record ApplyTransducer<A, B, C>(Transducer<A, Func<B, C>> FF, Transducer<A, B> FA) : Transducer<A, C>
{
    public Transducer<A, C> Morphism =>
        this;

    public Reducer<S, A> Transform<S>(Reducer<S, C> reduce) =>
        new Reduce<S>(FF, FA, reduce);

    record Reduce<S>(Transducer<A, Func<B, C>> FF, Transducer<A, B> FA, Reducer<S, C> Reducer) : Reducer<S, A>
    {
        public override TResult<S> Run(TState state, S stateValue, A value) =>
            FF.Transform(new Ap<S>(value, FA, Reducer)).Run(state, stateValue, value);
    }
    
    record Ap<S>(A Value, Transducer<A, B> FA, Reducer<S, C> Reducer) : Reducer<S, Func<B, C>>
    {
        public override TResult<S> Run(TState state, S stateValue, Func<B, C> f) =>
            FA.Transform(new Ap2<S>(f, Reducer)).Run(state, stateValue, Value);
    }
    
    record Ap2<S>(Func<B, C> F, Reducer<S, C> Reducer) : Reducer<S, B>
    {
        public override TResult<S> Run(TState state, S stateValue, B value) =>
            TResult.Recursive(state, stateValue, F(value), Reducer);
    }
}
