using System;

namespace LanguageExt.Transducers;

record FoldTransducer1<S, E, A>(Transducer<E, A> Transducer, S InitialState, Func<S, A, S> Folder) : Transducer<E, S>
{
    public Transducer<E, S> Morphism =>
        this;

    public Reducer<S1, E> Transform<S1>(Reducer<S1, S> reduce) =>
        new Reduce1<S1>(Transducer, InitialState, Folder, reduce);

    record Reduce1<InS>(
        Transducer<E, A> Transducer, 
        S InitialState, 
        Func<S, A, S> Folder,
        Reducer<InS, S> Reduce) : Reducer<InS, E>
    {
        public override TResult<InS> Run(TState state, InS stateValue, E env) =>
            Transducer
                .Transform(Reducer.from<S, A>((_, s, x) => TResult.Continue(Folder(s, x))))
                .Run(state, InitialState, env)
                .Bind(sx => Reduce.Run(state, stateValue, sx));
    }
}

record FoldTransducer2<S, E, X, A>(Transducer<E, Sum<X, A>> Transducer, S InitialState, Func<S, A, S> Folder) : Transducer<E, Sum<X, S>>
{
    public Transducer<E, Sum<X, S>> Morphism =>
        this;

    public Reducer<S1, E> Transform<S1>(Reducer<S1, Sum<X, S>> reduce) =>
        new Reduce1<S1>(Transducer, InitialState, Folder, reduce);

    record Reduce1<InS>(
        Transducer<E, Sum<X, A>> Transducer, 
        S InitialState, 
        Func<S, A, S> Folder,
        Reducer<InS, Sum<X, S>> Reduce) : Reducer<InS, E>
    {
        public override TResult<InS> Run(TState state, InS stateValue, E env) =>
            Transducer
                .Transform(
                    Reducer.from<Sum<X, S>, Sum<X, A>>((st, s, xa) => (s, xa) switch
                    {
                        (SumRight<X, S> xs, SumRight<X, A> r) => TResult.Continue(Sum<X, S>.Right(Folder(xs.Value, r.Value))),
                        (SumRight<X, S>, SumLeft<X, A> l) => TResult.Complete(Sum<X, S>.Left(l.Value)),
                        _ => TResult.Complete(s),
                    }))
                .Run(state, Sum<X, S>.Right(InitialState), env)
                .Bind(sx => Reduce.Run(state, stateValue, sx));
    }
}
