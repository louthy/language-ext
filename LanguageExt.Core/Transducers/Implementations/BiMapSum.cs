#nullable enable
using System;

namespace LanguageExt.Transducers;

record BiMapSum<X, Y, A, B>(Transducer<X, Y> Left, Transducer<A, B> Right) : Transducer<Sum<X, A>, Sum<Y, B>>
{
    readonly Transducer<A, Sum<Y, B>> rightMap = Transducer.compose(Right, Transducer.mkRight<Y, B>());
    readonly Transducer<X, Sum<Y, B>> leftMap = Transducer.compose(Left, Transducer.mkLeft<Y, B>());

    public override Reducer<Sum<X, A>, S> Transform<S>(Reducer<Sum<Y, B>, S> reduce) =>
        new Reduce<S>(rightMap, leftMap, reduce);
    
    public override string ToString() => 
        "bimap";

    record Reduce<S>(Transducer<A, Sum<Y, B>> RightMap, Transducer<X, Sum<Y, B>> LeftMap, Reducer<Sum<Y, B>, S> Reducer) : Reducer<Sum<X, A>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<X, A> value) =>
            value switch
            {
                SumRight<X, A> r => RightMap.Transform(Reducer).Run(state, stateValue, r.Value),
                SumLeft<X, A> l  => LeftMap.Transform(Reducer).Run(state, stateValue, l.Value),
                _                => TResult.Complete(stateValue)
            };
    }
}

record BiMapProduct<X, Y, A, B>(Transducer<X, Y> Left, Transducer<A, B> Right) : Transducer<(X, A), (Y, B)>
{
    public override Reducer<(X, A), S> Transform<S>(Reducer<(Y, B), S> reduce) =>
        new Reduce<S>(Left, Right, reduce);
    
    public override string ToString() => 
        "bimap";

    record Reduce<S>(Transducer<X, Y> Left, Transducer<A, B> Right, Reducer<(Y, B), S> Reducer) : Reducer<(X, A), S>
    {
        public override TResult<S> Run(TState state, S stateValue, (X, A) value) =>
            Left.Transform(new LeftReduce<S>(Right, value.Item2, Reducer))
                .Run(state, stateValue, value.Item1);
    }
    
    record LeftReduce<S>(Transducer<A, B> Right, A Value, Reducer<(Y, B), S> Reducer) : Reducer<Y, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Y left) =>
            Right.Transform(new RightReduce<S>(left, Reducer))
                 .Run(state, stateValue, Value);
    }
    
    record RightReduce<S>(Y Left, Reducer<(Y, B), S> reducer) : Reducer<B, S>
    {
        public override TResult<S> Run(TState state, S stateValue, B right) =>
            reducer.Run(state, stateValue, (Left, right));
    }
}

