using System.Diagnostics;
using LanguageExt.Common;

namespace LanguageExt.Transducers;

record BiMap<E, X, Y, A, B>(
    Transducer<E, Sum<X, A>> First, 
    Transducer<X, Y> Left,
    Transducer<A, B> Right) : Transducer<E, Sum<Y, B>>
{
    public Transducer<E, Sum<Y, B>> Morphism =>
        this;

    public Reducer<S, E> Transform<S>(Reducer<S, Sum<Y, B>> reduce) =>
        First.Transform(new FirstReduce<S>(Left, Right, reduce));

    record FirstReduce<S>(
        Transducer<X, Y> Left, 
        Transducer<A, B> Right, 
        Reducer<S, Sum<Y, B>> Reduce) : Reducer<S, Sum<X, A>>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<X, A> value) =>
            value switch
            {
                SumRight<X, A> r => Right.Transform(new RightReduce<S>(Reduce)).Run(state, stateValue, r.Value), 
                SumLeft<X, A> l => Left.Transform(new LeftReduce<S>(Reduce)).Run(state, stateValue, l.Value),
                _ => throw new BottomException()
            };
    }

    record RightReduce<S>(Reducer<S, Sum<Y, B>> Reduce) : Reducer<S, B>
    {
        public override TResult<S> Run(TState state, S stateValue, B value) =>
            Reduce.Run(state, stateValue, Sum<Y, B>.Right(value));
    }
    
    record LeftReduce<S>(Reducer<S, Sum<Y, B>> Reduce) : Reducer<S, Y>
    {
        public override TResult<S> Run(TState state, S stateValue, Y value) =>
            Reduce.Run(state, stateValue, Sum<Y, B>.Left(value));
    }

    public override string ToString() => 
        "bimap";
}

record BiMap2<X, Y, Z, A, B, C>(
    Transducer<Sum<X, A>, Sum<Y, B>> First, 
    Transducer<Y, Z> Left,
    Transducer<B, C> Right) : Transducer<Sum<X, A>, Sum<Z, C>>
{
    public Transducer<Sum<X, A>, Sum<Z, C>> Morphism =>
        this;

    public Reducer<S, Sum<X, A>> Transform<S>(Reducer<S, Sum<Z, C>> reduce) =>
        First.Transform(new FirstReduce<S>(Left, Right, reduce));

    record FirstReduce<S>(
        Transducer<Y, Z> Left, 
        Transducer<B, C> Right, 
        Reducer<S, Sum<Z, C>> Reduce) : Reducer<S, Sum<Y, B>>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<Y, B> value) =>
            value switch
            {
                SumRight<Y, B> r => Right.Transform(new RightReduce<S>(Reduce)).Run(state, stateValue, r.Value), 
                SumLeft<Y, B> l => Left.Transform(new LeftReduce<S>(Reduce)).Run(state, stateValue, l.Value),
                _ => throw new BottomException()
            };
    }

    record RightReduce<S>(Reducer<S, Sum<Z, C>> Reduce) : Reducer<S, C>
    {
        public override TResult<S> Run(TState state, S stateValue, C value) =>
            Reduce.Run(state, stateValue, Sum<Z, C>.Right(value));
    }
    
    record LeftReduce<S>(Reducer<S, Sum<Z, C>> Reduce) : Reducer<S, Z>
    {
        public override TResult<S> Run(TState state, S stateValue, Z value) =>
            Reduce.Run(state, stateValue, Sum<Z, C>.Left(value));
    }

    public override string ToString() => 
        "bimap";
}

record MapRight<E, X, A, B>(Transducer<E, Sum<X, A>> First, Transducer<A, B> Right) : Transducer<E, Sum<X, B>>
{
    public Transducer<E, Sum<X, B>> Morphism =>
        this;

    public Reducer<S, E> Transform<S>(Reducer<S, Sum<X, B>> reduce) =>
        First.Transform(new FirstReduce<S>(Right, reduce));

    record FirstReduce<S>(Transducer<A, B> Right, Reducer<S, Sum<X, B>> Reduce) : Reducer<S, Sum<X, A>>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<X, A> value) =>
            value switch
            {
                SumRight<X, A> r => Right.Transform(new RightReduce<S>(Reduce)).Run(state, stateValue, r.Value), 
                SumLeft<X, A> l => Reduce.Run(state, stateValue, Sum<X, B>.Left(l.Value)),
                _ => throw new BottomException()
            };
    }

    record RightReduce<S>(Reducer<S, Sum<X, B>> Reduce) : Reducer<S, B>
    {
        public override TResult<S> Run(TState state, S stateValue, B value) =>
            Reduce.Run(state, stateValue, Sum<X, B>.Right(value));
    }

    public override string ToString() => 
        "mapRight";
}

record MapRight2<X, Y, A, B, C>(
    Transducer<Sum<X, A>, Sum<Y, B>> First, 
    Transducer<B, C> Right) : Transducer<Sum<X, A>, Sum<Y, C>>
{
    public Transducer<Sum<X, A>, Sum<Y, C>> Morphism =>
        this;

    public Reducer<S, Sum<X, A>> Transform<S>(Reducer<S, Sum<Y, C>> reduce) =>
        First.Transform(new FirstReduce<S>(Right, reduce));

    record FirstReduce<S>(Transducer<B, C> Right, Reducer<S, Sum<Y, C>> Reduce) : Reducer<S, Sum<Y, B>>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<Y, B> value) =>
            value switch
            {
                SumRight<Y, B> r => Right.Transform(new RightReduce<S>(Reduce)).Run(state, stateValue, r.Value), 
                SumLeft<Y, B> l => Reduce.Run(state, stateValue, Sum<Y, C>.Left(l.Value)),
                _ => throw new BottomException()
            };
    }

    record RightReduce<S>(Reducer<S, Sum<Y, C>> Reduce) : Reducer<S, C>
    {
        public override TResult<S> Run(TState state, S stateValue, C value) =>
            Reduce.Run(state, stateValue, Sum<Y, C>.Right(value));
    }

    public override string ToString() => 
        "mapRight";
}

record MapLeft<E, X, Y, A>(Transducer<E, Sum<X, A>> First, Transducer<X, Y> Left) : Transducer<E, Sum<Y, A>>
{
    public Transducer<E, Sum<Y, A>> Morphism =>
        this;

    public Reducer<S, E> Transform<S>(Reducer<S, Sum<Y, A>> reduce) =>
        First.Transform(new FirstReduce<S>(Left, reduce));

    record FirstReduce<S>(Transducer<X, Y> Left, Reducer<S, Sum<Y, A>> Reduce) : Reducer<S, Sum<X, A>>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<X, A> value) =>
            value switch
            {
                SumRight<X, A> r => Reduce.Run(state, stateValue, Sum<Y, A>.Right(r.Value)),
                SumLeft<X, A> l => Left.Transform(new LeftReduce<S>(Reduce)).Run(state, stateValue, l.Value),
                _ => throw new BottomException()
            };
    }
    
    record LeftReduce<S>(Reducer<S, Sum<Y, A>> Reduce) : Reducer<S, Y>
    {
        public override TResult<S> Run(TState state, S stateValue, Y value) =>
            Reduce.Run(state, stateValue, Sum<Y, A>.Left(value));
    }

    public override string ToString() => 
        "mapLeft";
}

record MapLeft2<X, Y, Z, A, B>(
    Transducer<Sum<X, A>, Sum<Y, B>> First, 
    Transducer<Y, Z> Left) : Transducer<Sum<X, A>, Sum<Z, B>>
{
    public Transducer<Sum<X, A>, Sum<Z, B>> Morphism =>
        this;

    public Reducer<S, Sum<X, A>> Transform<S>(Reducer<S, Sum<Z, B>> reduce) =>
        First.Transform(new FirstReduce<S>(Left, reduce));

    record FirstReduce<S>(Transducer<Y, Z> Left, Reducer<S, Sum<Z, B>> Reduce) : Reducer<S, Sum<Y, B>>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<Y, B> value) =>
            value switch
            {
                SumRight<Y, B> r => Reduce.Run(state, stateValue, Sum<Z, B>.Right(r.Value)),
                SumLeft<Y, B> l => Left.Transform(new LeftReduce<S>(Reduce)).Run(state, stateValue, l.Value),
                _ => TResult.Complete(stateValue)
            };
    }
    
    record LeftReduce<S>(Reducer<S, Sum<Z, B>> Reduce) : Reducer<S, Z>
    {
        public override TResult<S> Run(TState state, S stateValue, Z value) =>
            Reduce.Run(state, stateValue, Sum<Z, B>.Left(value));
    }

    public override string ToString() => 
        "mapLeft";
}
