#nullable enable

namespace LanguageExt;

record FlattenTransducer1<A, B>(Transducer<A, Transducer<A, B>> FF) : Transducer<A, B>
{
    public override Reducer<A, S> Transform<S>(Reducer<B, S> reduce) =>
        new Reduce<S>(FF, reduce);
    
    record Reduce<S>(Transducer<A, Transducer<A, B>> FF, Reducer<B, S> Reducer) : Reducer<A, S>
    {
        public override TResult<S> Run(TState st, S s, A x) =>
            TResult.Recursive(st, s, x, FF.Transform(new Reduce1<S>(x, Reducer)));
    }    
    
    record Reduce1<S>(A Value, Reducer<B, S> Reducer) : Reducer<Transducer<A, B>, S>
    {
        public override TResult<S> Run(TState st, S s, Transducer<A, B> f) =>
            f.Transform(Reducer).Run(st, s, Value);
    }

    public override string ToString() =>  
        "flatten";
}

record FlattenTransducer2<A, B>(Transducer<A, Transducer<Unit, B>> FF) : Transducer<A, B>
{
    public override Reducer<A, S> Transform<S>(Reducer<B, S> reduce) =>
        new Reduce<S>(FF, reduce);
    
    record Reduce<S>(Transducer<A, Transducer<Unit, B>> FF, Reducer<B, S> Reducer) : Reducer<A, S>
    {
        public override TResult<S> Run(TState st, S s, A x) =>
            TResult.Recursive(st, s, x, FF.Transform(new Reduce1<S>(Reducer)));
    }    
    
    record Reduce1<S>(Reducer<B, S> Reducer) : Reducer<Transducer<Unit, B>, S>
    {
        public override TResult<S> Run(TState st, S s, Transducer<Unit, B> f) =>
            f.Transform(Reducer).Run(st, s, default);
    }    
}

record FlattenSumTransducer1<Env, X, A>(Transducer<Env, Sum<X, Transducer<Env, Sum<X, A>>>> FF) 
    : Transducer<Env, Sum<X, A>>
{
    public override Reducer<Env, S> Transform<S>(Reducer<Sum<X, A>, S> reduce) =>
        new Reduce0<S>(FF, reduce);

    record Reduce0<S>(Transducer<Env, Sum<X, Transducer<Env, Sum<X, A>>>> FF, Reducer<Sum<X, A>, S> Reducer) 
        : Reducer<Env, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Env value) =>
            TResult.Recursive(state, stateValue, value, FF.Transform(new Reduce<S>(value, Reducer)));
    }

    record Reduce<S>(Env Env, Reducer<Sum<X, A>, S> Reducer) : Reducer<Sum<X, Transducer<Env, Sum<X, A>>>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<X, Transducer<Env, Sum<X, A>>> value) =>
            value switch
            {
                SumRight<X, Transducer<Env, Sum<X, A>>> r =>
                    TResult.Recursive(state, stateValue, Env, r.Value.Transform(Reducer)),

                SumLeft<X, Transducer<Env, Sum<X, A>>> l =>
                    Reducer.Run(state, stateValue, Sum<X, A>.Left(l.Value)),
                
                _ => TResult.Complete(stateValue)
            };
    }    
}

record FlattenSumTransducer2<Env, X, A>(Transducer<Env, Sum<Transducer<Env, Sum<X, A>>, Transducer<Env, Sum<X, A>>>> FF) 
    : Transducer<Env, Sum<X, A>>
{
    public override Reducer<Env, S> Transform<S>(Reducer<Sum<X, A>, S> reduce) =>
        new Reduce0<S>(FF, reduce);

    record Reduce0<S>(Transducer<Env, Sum<Transducer<Env, Sum<X, A>>, Transducer<Env, Sum<X, A>>>> FF, Reducer<Sum<X, A>, S> Reducer) 
        : Reducer<Env, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Env value) =>
            TResult.Recursive(state, stateValue, value, FF.Transform(new Reduce1<S>(value, Reducer)));
    }

    record Reduce1<S>(Env Env, Reducer<Sum<X, A>, S> Reducer) : Reducer<Sum<Transducer<Env, Sum<X, A>>, Transducer<Env, Sum<X, A>>>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<Transducer<Env, Sum<X, A>>, Transducer<Env, Sum<X, A>>> value) =>
            value switch
            {
                SumRight<Transducer<Env, Sum<X, A>>, Transducer<Env, Sum<X, A>>> r =>
                    r.Value.Transform(Reducer).Run(state, stateValue, Env),

                SumLeft<Transducer<Env, Sum<X, A>>, Transducer<Env, Sum<X, A>>> l =>
                    l.Value.Transform(Reducer).Run(state, stateValue, Env),
                
                _ => TResult.Complete(stateValue)
            };
    }    
            
    public override string ToString() =>  
        "flatten";
}

record FlattenSumTransducer3<Env, X, A>(Transducer<Env, Sum<X, Transducer<Unit, Sum<X, A>>>> FF) 
    : Transducer<Env, Sum<X, A>>
{
    public override Reducer<Env, S> Transform<S>(Reducer<Sum<X, A>, S> reduce) =>
        new Reduce0<S>(FF, reduce);

    record Reduce0<S>(Transducer<Env, Sum<X, Transducer<Unit, Sum<X, A>>>> FF, Reducer<Sum<X, A>, S> Reducer) 
        : Reducer<Env, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Env value) =>
            TResult.Recursive(state, stateValue, value, FF.Transform(new Reduce<S>(Reducer)));
    }

    record Reduce<S>(Reducer<Sum<X, A>, S> Reducer) : Reducer<Sum<X, Transducer<Unit, Sum<X, A>>>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<X, Transducer<Unit, Sum<X, A>>> value) =>
            value switch
            {
                SumRight<X, Transducer<Unit, Sum<X, A>>> r =>
                    TResult.Recursive(state, stateValue, default, r.Value.Transform(Reducer)),

                SumLeft<X, Transducer<Unit, Sum<X, A>>> l =>
                    Reducer.Run(state, stateValue, Sum<X, A>.Left(l.Value)),
                
                _ => TResult.Complete(stateValue)
            };
    }    
}
