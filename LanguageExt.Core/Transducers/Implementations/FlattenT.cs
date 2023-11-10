#nullable enable
using LanguageExt.HKT;

namespace LanguageExt.Transducers;

public static partial class Transducer
{
    public static Transducer<Env, A> FlattenT<Env, A>(
        this Transducer<Env, Transducer<Env, Transducer<Env, Transducer<Env, A>>>> m) =>
        m.Flatten().Flatten().Flatten();
    
    public static Transducer<Env, A> FlattenT<Env, A>(
        this Transducer<Env, Transducer<Unit, Transducer<Env, Transducer<Unit, A>>>> m) =>
        m.Flatten().Flatten().Flatten();

    public static Transducer<Env, Sum<X, A>> FlattenT<Env, X, A>(
        this Transducer<Env, Transducer<Unit, Sum<X, Transducer<Env, Transducer<Unit, Sum<X, A>>>>>> m) =>
        new FlattenTTransducer1<Env, X, A>(m);

    public static Transducer<Env, Sum<X, A>> FlattenT<Env, X, A>(
        this Transducer<Env, Transducer<Env, Sum<X, Transducer<Env, Transducer<Env, Sum<X, A>>>>>> m) =>
        new FlattenTTransducer2<Env, X, A>(m);
}

record FlattenTTransducer1<Env, X, A>(
    Transducer<Env, Transducer<Unit, Sum<X, Transducer<Env, Transducer<Unit, Sum<X, A>>>>>> FF) : 
    Transducer<Env, Sum<X, A>>
{
    public Reducer<S, Env> Transform<S>(Reducer<S, Sum<X, A>> reduce) =>
        new Reduce<S>(FF, reduce);

    record Reduce<S>(
        Transducer<Env, Transducer<Unit, Sum<X, Transducer<Env, Transducer<Unit, Sum<X, A>>>>>> FF,
        Reducer<S, Sum<X, A>> Reducer) : 
        Reducer<S, Env>
    {
        public override TResult<S> Run(TState st, S s, Env value) =>
            FF.Transform(new Reduce1<S>(value, Reducer)).Run(st, s, value);
    }
    
    record Reduce1<S>(Env Env, Reducer<S, Sum<X, A>> Reducer) : 
        Reducer<S, Transducer<Unit, Sum<X, Transducer<Env, Transducer<Unit, Sum<X, A>>>>>>
    {
        public override TResult<S> Run(
            TState st,
            S s,
            Transducer<Unit, Sum<X, Transducer<Env, Transducer<Unit, Sum<X, A>>>>> t) =>
            t.Transform(new Reduce2<S>(Env, Reducer)).Run(st, s, default);
    }
    
    record Reduce2<S>(Env Env, Reducer<S, Sum<X, A>> Reducer) : 
        Reducer<S, Sum<X, Transducer<Env, Transducer<Unit, Sum<X, A>>>>>
    {
        public override TResult<S> Run(
            TState st,
            S s,
            Sum<X, Transducer<Env, Transducer<Unit, Sum<X, A>>>> t) =>
            t switch
            {
                SumRight<X, Transducer<Env, Transducer<Unit, Sum<X, A>>>> r =>
                    r.Value.Transform(new Reduce3<S>(Reducer)).Run(st, s, Env),

                SumLeft<X, Transducer<Env, Transducer<Unit, Sum<X, A>>>> l =>
                    Reducer.Run(st, s,  Sum<X, A>.Left(l.Value)),

                _ => TResult.Complete(s),
            };
    }
    
    record Reduce3<S>(Reducer<S, Sum<X, A>> Reducer) : Reducer<S, Transducer<Unit, Sum<X, A>>>
    {
        public override TResult<S> Run(TState st, S s, Transducer<Unit, Sum<X, A>> t) =>
            t.Transform(Reducer).Run(st, s, default);
    }

    public Transducer<Env, Sum<X, A>> Morphism =>
        this;
}

record FlattenTTransducer2<Env, X, A>(
    Transducer<Env, Transducer<Env, Sum<X, Transducer<Env, Transducer<Env, Sum<X, A>>>>>> FF) : 
    Transducer<Env, Sum<X, A>>
{
    public Reducer<S, Env> Transform<S>(Reducer<S, Sum<X, A>> reduce) =>
        new Reduce<S>(FF, reduce);

    record Reduce<S>(
        Transducer<Env, Transducer<Env, Sum<X, Transducer<Env, Transducer<Env, Sum<X, A>>>>>> FF,
        Reducer<S, Sum<X, A>> Reducer) : 
        Reducer<S, Env>
    {
        public override TResult<S> Run(TState st, S s, Env value) =>
            FF.Transform(new Reduce1<S>(value, Reducer)).Run(st, s, value);
    }
    
    record Reduce1<S>(Env Env, Reducer<S, Sum<X, A>> Reducer) : 
        Reducer<S, Transducer<Env, Sum<X, Transducer<Env, Transducer<Env, Sum<X, A>>>>>>
    {
        public override TResult<S> Run(
            TState st,
            S s,
            Transducer<Env, Sum<X, Transducer<Env, Transducer<Env, Sum<X, A>>>>> t) =>
            t.Transform(new Reduce2<S>(Env, Reducer)).Run(st, s, Env);
    }
    
    record Reduce2<S>(Env Env, Reducer<S, Sum<X, A>> Reducer) : 
        Reducer<S, Sum<X, Transducer<Env, Transducer<Env, Sum<X, A>>>>>
    {
        public override TResult<S> Run(
            TState st,
            S s,
            Sum<X, Transducer<Env, Transducer<Env, Sum<X, A>>>> t) =>
            t switch
            {
                SumRight<X, Transducer<Env, Transducer<Env, Sum<X, A>>>> r =>
                    r.Value.Transform(new Reduce3<S>(Env, Reducer)).Run(st, s, Env),

                SumLeft<X, Transducer<Env, Transducer<Env, Sum<X, A>>>> l =>
                    Reducer.Run(st, s, Sum<X, A>.Left(l.Value)),

                _ => TResult.Complete(s)
            };
    }
    
    record Reduce3<S>(Env Env, Reducer<S, Sum<X, A>> Reducer) 
        : Reducer<S, Transducer<Env, Sum<X, A>>>
    {
        public override TResult<S> Run(
            TState st,
            S s,
            Transducer<Env, Sum<X, A>> t) =>
            t.Transform(Reducer).Run(st, s, Env);
    }

    public Transducer<Env, Sum<X, A>> Morphism =>
        this;
}
