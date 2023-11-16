using System;

namespace LanguageExt.Transducers;

record BiMap<E, X, Y, A, B>(
    Transducer<E, Sum<X, A>> First, 
    Transducer<X, Y> Left,
    Transducer<A, B> Right) : Transducer<E, Sum<Y, B>>
{
    public Transducer<E, Sum<Y, B>> Morphism =>
        this;

    public Reducer<E, S> Transform<S>(Reducer<Sum<Y, B>, S> reduce) =>
        First.Transform(new FirstReduce<S>(Left, Right, reduce));

    record FirstReduce<S>(
        Transducer<X, Y> Left, 
        Transducer<A, B> Right, 
        Reducer<Sum<Y, B>, S> Reduce) : Reducer<Sum<X, A>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<X, A> value) =>
            value switch
            {
                SumRight<X, A> r => TResult.Recursive(state, stateValue, r.Value, Right.Transform(new RightReduce<S>(Reduce))), 
                SumLeft<X, A> l => Left.Transform(new LeftReduce<S>(Reduce)).Run(state, stateValue, l.Value),
                _ => TResult.Complete(stateValue)
            };
    }

    record RightReduce<S>(Reducer<Sum<Y, B>, S> Reduce) : Reducer<B, S>
    {
        public override TResult<S> Run(TState state, S stateValue, B value) =>
            Reduce.Run(state, stateValue, Sum<Y, B>.Right(value));
    }
    
    record LeftReduce<S>(Reducer<Sum<Y, B>, S> Reduce) : Reducer<Y, S>
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

    public Reducer<Sum<X, A>, S> Transform<S>(Reducer<Sum<Z, C>, S> reduce) =>
        First.Transform(new FirstReduce<S>(Left, Right, reduce));

    record FirstReduce<S>(
        Transducer<Y, Z> Left, 
        Transducer<B, C> Right, 
        Reducer<Sum<Z, C>, S> Reduce) : Reducer<Sum<Y, B>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<Y, B> value) =>
            value switch
            {
                SumRight<Y, B> r => TResult.Recursive(state, stateValue, r.Value, Right.Transform(new RightReduce<S>(Reduce))), 
                SumLeft<Y, B> l => Left.Transform(new LeftReduce<S>(Reduce)).Run(state, stateValue, l.Value),
                _ => TResult.Complete(stateValue)
            };
    }

    record RightReduce<S>(Reducer<Sum<Z, C>, S> Reduce) : Reducer<C, S>
    {
        public override TResult<S> Run(TState state, S stateValue, C value) =>
            Reduce.Run(state, stateValue, Sum<Z, C>.Right(value));
    }
    
    record LeftReduce<S>(Reducer<Sum<Z, C>, S> Reduce) : Reducer<Z, S>
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

    public Reducer<E, S> Transform<S>(Reducer<Sum<X, B>, S> reduce) =>
        First.Transform(new FirstReduce<S>(Right, reduce));

    record FirstReduce<S>(Transducer<A, B> Right, Reducer<Sum<X, B>, S> Reduce) : Reducer<Sum<X, A>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<X, A> value) =>
            value switch
            {
                SumRight<X, A> r => TResult.Recursive(state, stateValue, r.Value, Right.Transform(new RightReduce<S>(Reduce))),
                SumLeft<X, A> l => Reduce.Run(state, stateValue, Sum<X, B>.Left(l.Value)),
                _ => TResult.Complete(stateValue)
            };
    }

    record RightReduce<S>(Reducer<Sum<X, B>, S> Reduce) : Reducer<B, S>
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

    public Reducer<Sum<X, A>, S> Transform<S>(Reducer<Sum<Y, C>, S> reduce) =>
        First.Transform(new FirstReduce<S>(Right, reduce));

    record FirstReduce<S>(Transducer<B, C> Right, Reducer<Sum<Y, C>, S> Reduce) : Reducer<Sum<Y, B>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<Y, B> value) =>
            value switch
            {
                SumRight<Y, B> r => TResult.Recursive(state, stateValue, r.Value, Right.Transform(new RightReduce<S>(Reduce))), 
                SumLeft<Y, B> l => Reduce.Run(state, stateValue, Sum<Y, C>.Left(l.Value)),
                _ => TResult.Complete(stateValue)
            };
    }

    record RightReduce<S>(Reducer<Sum<Y, C>, S> Reduce) : Reducer<C, S>
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

    public Reducer<E, S> Transform<S>(Reducer<Sum<Y, A>, S> reduce) =>
        First.Transform(new FirstReduce<S>(Left, reduce));

    record FirstReduce<S>(Transducer<X, Y> Left, Reducer<Sum<Y, A>, S> Reduce) : Reducer<Sum<X, A>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<X, A> value) =>
            value switch
            {
                SumRight<X, A> r => Reduce.Run(state, stateValue, Sum<Y, A>.Right(r.Value)),
                SumLeft<X, A> l => Left.Transform(new LeftReduce<S>(Reduce)).Run(state, stateValue, l.Value),
                _ => TResult.Complete(stateValue)
            };
    }
    
    record LeftReduce<S>(Reducer<Sum<Y, A>, S> Reduce) : Reducer<Y, S>
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

    public Reducer<Sum<X, A>, S> Transform<S>(Reducer<Sum<Z, B>, S> reduce) =>
        First.Transform(new FirstReduce<S>(Left, reduce));

    record FirstReduce<S>(Transducer<Y, Z> Left, Reducer<Sum<Z, B>, S> Reduce) : Reducer<Sum<Y, B>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<Y, B> value) =>
            value switch
            {
                SumRight<Y, B> r => Reduce.Run(state, stateValue, Sum<Z, B>.Right(r.Value)),
                SumLeft<Y, B> l => Left.Transform(new LeftReduce<S>(Reduce)).Run(state, stateValue, l.Value),
                _ => TResult.Complete(stateValue)
            };
    }
    
    record LeftReduce<S>(Reducer<Sum<Z, B>, S> Reduce) : Reducer<Z, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Z value) =>
            Reduce.Run(state, stateValue, Sum<Z, B>.Left(value));
    }

    public override string ToString() => 
        "mapLeft";
}

record BiMap3<E, X, Y, A, B>(
    Transducer<E, Sum<X, A>> First, 
    Func<X, Y> Left,
    Func<A, B> Right) : Transducer<E, Sum<Y, B>>
{
    public Transducer<E, Sum<Y, B>> Morphism =>
        this;

    public Reducer<E, S> Transform<S>(Reducer<Sum<Y, B>, S> reduce) =>
        First.Transform(new FirstReduce<S>(Left, Right, reduce));

    record FirstReduce<S>(
        Func<X, Y> Left, 
        Func<A, B> Right, 
        Reducer<Sum<Y, B>, S> Reduce) : Reducer<Sum<X, A>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<X, A> value) =>
            value switch
            {
                SumRight<X, A> r => Reduce.Run(state, stateValue, Sum<Y, B>.Right(Right(r.Value))), 
                SumLeft<X, A> l => Reduce.Run(state, stateValue, Sum<Y, B>.Left(Left(l.Value))),
                _ => TResult.Complete(stateValue)
            };
    }

    public override string ToString() => 
        "bimap";
}

record BiMap4<X, Y, Z, A, B, C>(
    Transducer<Sum<X, A>, Sum<Y, B>> First, 
    Func<Y, Z> Left,
    Func<B, C> Right) : Transducer<Sum<X, A>, Sum<Z, C>>
{
    public Transducer<Sum<X, A>, Sum<Z, C>> Morphism =>
        this;

    public Reducer<Sum<X, A>, S> Transform<S>(Reducer<Sum<Z, C>, S> reduce) =>
        First.Transform(new FirstReduce<S>(Left, Right, reduce));

    record FirstReduce<S>(
        Func<Y, Z> Left, 
        Func<B, C> Right, 
        Reducer<Sum<Z, C>, S> Reduce) : Reducer<Sum<Y, B>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<Y, B> value) =>
            value switch
            {
                SumRight<Y, B> r => Reduce.Run(state, stateValue, Sum<Z, C>.Right(Right(r.Value))), 
                SumLeft<Y, B> l => Reduce.Run(state, stateValue, Sum<Z, C>.Left(Left(l.Value))),
                _ => TResult.Complete(stateValue)
            };
    }

    public override string ToString() => 
        "bimap";
}

record MapRight3<E, X, A, B>(Transducer<E, Sum<X, A>> First, Func<A, B> Right) : Transducer<E, Sum<X, B>>
{
    public Transducer<E, Sum<X, B>> Morphism =>
        this;

    public Reducer<E, S> Transform<S>(Reducer<Sum<X, B>, S> reduce) =>
        First.Transform(new FirstReduce<S>(Right, reduce));

    record FirstReduce<S>(Func<A, B> Right, Reducer<Sum<X, B>, S> Reduce) : Reducer<Sum<X, A>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<X, A> value) =>
            value switch
            {
                SumRight<X, A> r => Reduce.Run(state, stateValue, Sum<X, B>.Right(Right(r.Value))),
                SumLeft<X, A> l => Reduce.Run(state, stateValue, Sum<X, B>.Left(l.Value)),
                _ => TResult.Complete(stateValue)
            };
    }

    public override string ToString() => 
        "mapRight";
}

record MapRight1<X, Y, A, B, C>(
    Transducer<Sum<X, A>, Sum<Y, B>> First, 
    Func<B, C> Right) : Transducer<Sum<X, A>, Sum<Y, C>>
{
    public Transducer<Sum<X, A>, Sum<Y, C>> Morphism =>
        this;

    public Reducer<Sum<X, A>, S> Transform<S>(Reducer<Sum<Y, C>, S> reduce) =>
        First.Transform(new FirstReduce<S>(Right, reduce));

    record FirstReduce<S>(Func<B, C> Right, Reducer<Sum<Y, C>, S> Reduce) : Reducer<Sum<Y, B>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<Y, B> value) =>
            value switch
            {
                SumRight<Y, B> r => Reduce.Run(state, stateValue, Sum<Y, C>.Right(Right(r.Value))), 
                SumLeft<Y, B> l => Reduce.Run(state, stateValue, Sum<Y, C>.Left(l.Value)),
                _ => TResult.Complete(stateValue)
            };
    }

    public override string ToString() => 
        "mapRight";
}

record MapLeft2<E, X, Y, A>(Transducer<E, Sum<X, A>> First, Func<X, Y> Left) : Transducer<E, Sum<Y, A>>
{
    public Transducer<E, Sum<Y, A>> Morphism =>
        this;

    public Reducer<E, S> Transform<S>(Reducer<Sum<Y, A>, S> reduce) =>
        First.Transform(new FirstReduce<S>(Left, reduce));

    record FirstReduce<S>(Func<X, Y> Left, Reducer<Sum<Y, A>, S> Reduce) : Reducer<Sum<X, A>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<X, A> value) =>
            value switch
            {
                SumRight<X, A> r => Reduce.Run(state, stateValue, Sum<Y, A>.Right(r.Value)),
                SumLeft<X, A> l => Reduce.Run(state, stateValue, Sum<Y, A>.Left(Left(l.Value))),
                _ => TResult.Complete(stateValue)
            };
    }

    public override string ToString() => 
        "mapLeft";
}

record MapLeft3<X, Y, Z, A, B>(
    Transducer<Sum<X, A>, Sum<Y, B>> First, 
    Func<Y, Z> Left) : Transducer<Sum<X, A>, Sum<Z, B>>
{
    public Transducer<Sum<X, A>, Sum<Z, B>> Morphism =>
        this;

    public Reducer<Sum<X, A>, S> Transform<S>(Reducer<Sum<Z, B>, S> reduce) =>
        First.Transform(new FirstReduce<S>(Left, reduce));

    record FirstReduce<S>(Func<Y, Z> Left, Reducer<Sum<Z, B>, S> Reduce) : Reducer<Sum<Y, B>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<Y, B> value) =>
            value switch
            {
                SumRight<Y, B> r => Reduce.Run(state, stateValue, Sum<Z, B>.Right(r.Value)),
                SumLeft<Y, B> l => Reduce.Run(state, stateValue, Sum<Z, B>.Left(Left(l.Value))),
                _ => TResult.Complete(stateValue)
            };
    }

    public override string ToString() => 
        "mapLeft";
}
