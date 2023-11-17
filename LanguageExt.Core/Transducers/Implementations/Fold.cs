#nullable enable
using System;

namespace LanguageExt.Transducers;

record FoldTransducer1<TState, E, A>(
        Transducer<E, A> Transducer, 
        TState InitialState, 
        Func<TState, A, TState> Folder) 
    : Transducer<E, TState>
{
    public Transducer<E, TState> Morphism =>
        this;

    public Reducer<E, S> Transform<S>(Reducer<TState, S> reduce) =>
        new Reduce1<S>(Transducer, InitialState, Folder, reduce);

    record Reduce1<S>(
        Transducer<E, A> Transducer, 
        TState InitialState, 
        Func<TState, A, TState> Folder,
        Reducer<TState, S> Reduce) : Reducer<E, S>
    {
        public override TResult<S> Run(LanguageExt.TState state, S stateValue, E env) =>
            Transducer
                .Transform(Reducer.from<A, TState>((_, s, x) => TResult.Continue(Folder(s, x))))
                .Run(state, InitialState, env)
                .Bind(sx => Reduce.Run(state, stateValue, sx));
    }
            
    public override string ToString() =>  
        "fold";
}

record FoldTransducer2<TState, E, X, A>(
    Transducer<E, Sum<X, A>> Transducer, 
    TState InitialState, 
    Func<TState, A, TState> Folder) : Transducer<E, Sum<X, TState>>
{
    public Transducer<E, Sum<X, TState>> Morphism =>
        this;

    public Reducer<E, S> Transform<S>(Reducer<Sum<X, TState>, S> reduce) =>
        new Reduce1<S>(Transducer, InitialState, Folder, reduce);

    record Reduce1<S>(
        Transducer<E, Sum<X, A>> Transducer, 
        TState InitialState, 
        Func<TState, A, TState> Folder,
        Reducer<Sum<X, TState>, S> Reduce) : Reducer<E, S>
    {
        public override TResult<S> Run(LanguageExt.TState state, S stateValue, E env) =>
            Transducer
                .Transform(
                    Reducer.from<Sum<X, A>, Sum<X, TState>>((st, s, xa) => (s, xa) switch
                    {
                        (SumRight<X, TState> xs, SumRight<X, A> r) => TResult.Continue(Sum<X, TState>.Right(Folder(xs.Value, r.Value))),
                        (SumRight<X, TState>, SumLeft<X, A> l) => TResult.Complete(Sum<X, TState>.Left(l.Value)),
                        _ => TResult.Complete(s),
                    }))
                .Run(state, Sum<X, TState>.Right(InitialState), env)
                .Bind(sx => Reduce.Run(state, stateValue, sx));
    }
            
    public override string ToString() =>  
        "fold";
}
