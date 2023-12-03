#nullable enable
using System;

namespace LanguageExt.Transducers;

record PartialTransducer<A, B, C>(A Value, Transducer<A, Transducer<B, C>> F) : Transducer<B, C>
{
    public override Reducer<B, S> Transform<S>(Reducer<C, S> reduce) =>
        new Reduce1<S>(Value, F, reduce);

    record Reduce1<S>(A Value, Transducer<A, Transducer<B, C>> F, Reducer<C, S> Reducer) : Reducer<B, S>
    {
        public override TResult<S> Run(TState state, S stateValue, B value) =>
            F.Transform(new Reduce2<S>(value, Reducer)).Run(state, stateValue, Value);
    }

    record Reduce2<S>(B Value, Reducer<C, S> Reducer) : Reducer<Transducer<B, C>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Transducer<B, C> f) =>
            f.Transform(Reducer).Run(state, stateValue, Value);
    }
}

record PartialFTransducer<A, B, C>(A Value, Transducer<A, Func<B, C>> F) : Transducer<B, C>
{
    public override Reducer<B, S> Transform<S>(Reducer<C, S> reduce) =>
        new Reduce1<S>(Value, F, reduce);

    record Reduce1<S>(A Value, Transducer<A, Func<B, C>> F, Reducer<C, S> Reducer) : Reducer<B, S>
    {
        public override TResult<S> Run(TState state, S stateValue, B value) =>
            F.Transform(new Reduce2<S>(value, Reducer)).Run(state, stateValue, Value);
    }

    record Reduce2<S>(B Value, Reducer<C, S> Reducer) : Reducer<Func<B, C>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Func<B, C> f) =>
            Reducer.Run(state, stateValue, f(Value));
    }
}
