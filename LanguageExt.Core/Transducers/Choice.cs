#nullable enable
using System;

namespace LanguageExt.Transducers;

record ChoiceTransducer<X, A, B>(Seq<Transducer<A, Sum<X, B>>> Transducers) : Transducer<A, Sum<X, B>>
{
    public Transducer<A, Sum<X, B>> Morphism =>
        this;

    public Reducer<S, A> Transform<S>(Reducer<S, Sum<X, B>> reduce) =>
        Transducers.Head.Transform(new Reduce<S>(reduce, Transducers));

    record Reduce<S>(Reducer<S, Sum<X, B>> Reducer, Seq<Transducer<A, Sum<X, B>>> Transducers) : Reducer<S, Sum<X, B>>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<X, B> value) =>
            value switch
            {
                SumRight<X, B> r => Reducer.Run(state, stateValue, value),
                _ => Transducers.IsEmpty
                    ? TResult.Complete(stateValue)
                    : TResult.Recursive(state, stateValue, value, new Reduce<S>(Reducer, Transducers.Tail))
            };
    }
}
