#nullable enable
using System.Diagnostics;
using LanguageExt.Common;
using LanguageExt.HKT;

namespace LanguageExt.Transducers;

record FlattenTransducer1<A, B>(Transducer<A, Transducer<A, B>> FF) : Transducer<A, B>
{
    public Reducer<S, A> Transform<S>(Reducer<S, B> reduce) =>
        new Reduce<S>(FF, reduce);
    
    record Reduce<S>(Transducer<A, Transducer<A, B>> FF, Reducer<S, B> Reducer) : Reducer<S, A>
    {
        public override TResult<S> Run(TState st, S s, A x) =>
            FF.Transform(new Reduce1<S>(x, Reducer)).Run(st, s, x);
    }    
    
    record Reduce1<S>(A Value, Reducer<S, B> Reducer) : Reducer<S, Transducer<A, B>>
    {
        public override TResult<S> Run(TState st, S s, Transducer<A, B> f) =>
            f.Transform(Reducer).Run(st, s, Value);
    }

    public Transducer<A, B> Morphism =>
        this;
}

record FlattenTransducer2<A, B>(Transducer<A, Transducer<Unit, B>> FF) : Transducer<A, B>
{
    public Reducer<S, A> Transform<S>(Reducer<S, B> reduce) =>
        new Reduce<S>(FF, reduce);
    
    record Reduce<S>(Transducer<A, Transducer<Unit, B>> FF, Reducer<S, B> Reducer) : Reducer<S, A>
    {
        public override TResult<S> Run(TState st, S s, A x) =>
            FF.Transform(new Reduce1<S>(Reducer)).Run(st, s, x);
    }    
    
    record Reduce1<S>(Reducer<S, B> Reducer) : Reducer<S, Transducer<Unit, B>>
    {
        public override TResult<S> Run(TState st, S s, Transducer<Unit, B> f) =>
            f.Transform(Reducer).Run(st, s, default);
    }    

    public Transducer<A, B> Morphism =>
        this;
}

record FlattenSumTransducer1<Env, X, A>(Transducer<Env, Sum<X, Transducer<Env, Sum<X, A>>>> FF) 
    : Transducer<Env, Sum<X, A>>
{
    public Transducer<Env, Sum<X, A>> Morphism => 
        this;

    public Reducer<S, Env> Transform<S>(Reducer<S, Sum<X, A>> reduce) =>
        new Reduce0<S>(FF, reduce);

    record Reduce0<S>(Transducer<Env, Sum<X, Transducer<Env, Sum<X, A>>>> FF, Reducer<S, Sum<X, A>> Reducer) 
        : Reducer<S, Env>
    {
        public override TResult<S> Run(TState state, S stateValue, Env value) =>
            FF.Transform(new Reduce<S>(value, Reducer)).Run(state, stateValue, value);
    }

    record Reduce<S>(Env Env, Reducer<S, Sum<X, A>> Reducer) : Reducer<S, Sum<X, Transducer<Env, Sum<X, A>>>>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<X, Transducer<Env, Sum<X, A>>> value) =>
            value switch
            {
                SumRight<X, Transducer<Env, Sum<X, A>>> r =>
                    r.Value.Transform(Reducer).Run(state, stateValue, Env),

                SumLeft<X, Transducer<Env, Sum<X, A>>> l =>
                    Reducer.Run(state, stateValue, Sum<X, A>.Left(l.Value)),
                
                _ => TResult.Complete(stateValue)
            };
    }    
}

record FlattenSumTransducer2<Env, X, A>(Transducer<Env, Sum<Transducer<Env, Sum<X, A>>, Transducer<Env, Sum<X, A>>>> FF) 
    : Transducer<Env, Sum<X, A>>
{
    public Transducer<Env, Sum<X, A>> Morphism => 
        this;

    public Reducer<S, Env> Transform<S>(Reducer<S, Sum<X, A>> reduce) =>
        new Reduce0<S>(FF, reduce);

    record Reduce0<S>(Transducer<Env, Sum<Transducer<Env, Sum<X, A>>, Transducer<Env, Sum<X, A>>>> FF, Reducer<S, Sum<X, A>> Reducer) 
        : Reducer<S, Env>
    {
        public override TResult<S> Run(TState state, S stateValue, Env value) =>
            FF.Transform(new Reduce1<S>(value, Reducer)).Run(state, stateValue, value);
    }

    record Reduce1<S>(Env Env, Reducer<S, Sum<X, A>> Reducer) : Reducer<S, Sum<Transducer<Env, Sum<X, A>>, Transducer<Env, Sum<X, A>>>>
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
}
