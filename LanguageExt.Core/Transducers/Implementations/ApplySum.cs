#nullable enable
using System;

namespace LanguageExt;

/// <summary>
/// Applicative apply transducer
/// </summary>
record ApplySumTransducer<E, X, A, B>(Transducer<E, Sum<X, Func<A, B>>> FF, Transducer<E, Sum<X, A>> FA) : Transducer<E, Sum<X, B>>
{
    public override Reducer<E, S> Transform<S>(Reducer<Sum<X, B>, S> reduce) =>
        new Reduce<S>(FF, FA, reduce);

    record Reduce<S>(Transducer<E, Sum<X, Func<A, B>>> FF, Transducer<E, Sum<X, A>> FA, Reducer<Sum<X, B>, S> Reducer) : Reducer<E, S>
    {
        public override TResult<S> Run(TState state, S stateValue, E env) =>
            FF.Transform(new Ap<S>(env, FA, Reducer)).Run(state, stateValue, env);
    }
    
    record Ap<S>(E Env, Transducer<E, Sum<X, A>> FA, Reducer<Sum<X, B>, S> Reducer) : Reducer<Sum<X, Func<A, B>>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<X, Func<A, B>> xf) =>
            xf switch
            {
                SumRight<X, Func<A, B>> r => TResult.Recursive(state, stateValue, Env, FA.Transform(new Ap2<S>(r.Value, Reducer))),
                SumLeft<X, Func<A, B>> l  => Reducer.Run(state, stateValue, Sum<X, B>.Left(l.Value)),
                _                         => TResult.Continue(stateValue)
            };
    }

    record Ap2<S>(Func<A, B> F, Reducer<Sum<X, B>, S> Reducer) : Reducer<Sum<X, A>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<X, A> xa) =>
            xa switch
            {
                SumRight<X, A> r => Reducer.Run(state, stateValue, Sum<X, B>.Right(F(r.Value))),
                SumLeft<X, A> l  => Reducer.Run(state, stateValue, Sum<X, B>.Left(l.Value)),
                _                => TResult.Continue(stateValue)
            };
    }

    public override string ToString() =>  
        "applySum";
}

/// <summary>
/// Applicative apply transducer
/// </summary>
record ApplySumTransducer2<E, X, A, B>(Transducer<E, Sum<X, Transducer<A, B>>> FF, Transducer<E, Sum<X, A>> FA) : Transducer<E, Sum<X, B>>
{
    public override Reducer<E, S> Transform<S>(Reducer<Sum<X, B>, S> reduce) =>
        new Reduce<S>(FF, FA, reduce);

    record Reduce<S>(Transducer<E, Sum<X, Transducer<A, B>>> FF, Transducer<E, Sum<X, A>> FA, Reducer<Sum<X, B>, S> Reducer) : Reducer<E, S>
    {
        public override TResult<S> Run(TState state, S stateValue, E env) =>
            FF.Transform(new Ap<S>(env, FA, Reducer)).Run(state, stateValue, env);
    }
    
    record Ap<S>(E Env, Transducer<E, Sum<X, A>> FA, Reducer<Sum<X, B>, S> Reducer) : Reducer<Sum<X, Transducer<A, B>>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<X, Transducer<A, B>> xf) =>
            xf switch
            {
                SumRight<X, Transducer<A, B>> r => FA.Transform(new Ap2<S>(r.Value, Reducer)).Run(state, stateValue, Env),
                SumLeft<X, Transducer<A, B>> l  => Reducer.Run(state, stateValue, Sum<X, B>.Left(l.Value)),
                _                               => TResult.Continue(stateValue)
            };

    }
    
    record Ap2<S>(Transducer<A, B> F, Reducer<Sum<X, B>, S> Reducer) : Reducer<Sum<X, A>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<X, A> xa) =>
            xa switch
            {
                SumRight<X, A> r => TResult.Recursive(state,
                                                      stateValue,
                                                      r.Value,
                                                      Transducer.compose(F, Transducer.mkRight<X, B>()).Transform(Reducer)),
                SumLeft<X, A> l => Reducer.Run(state, stateValue, Sum<X, B>.Left(l.Value)),
                _               => TResult.Continue(stateValue)
            };
    }

    public override string ToString() =>  
        "applySum";
}
