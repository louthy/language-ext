#nullable enable
using System;

namespace LanguageExt.Transducers;


record SelectManyTransducer1<E, A, B, C>(
    Transducer<E, A> F, 
    Func<A, Transducer<E, B>> BindF, 
    Func<A, B, C> Project) : 
    Transducer<E, C>
{
    public Reducer<E, S> Transform<S>(Reducer<C, S> reduce) =>
        new Reduce<S>(F, BindF, Project, reduce);
    
    public Transducer<E, C> Morphism =>
        this;

    record Reduce<S>(Transducer<E, A> F, Func<A, Transducer<E, B>> Bind, Func<A, B, C> Project, Reducer<C, S> Reducer) : 
        Reducer<E, S>
    {
        public override TResult<S> Run(TState st, S s, E x) =>
            F.Transform(new Binder<S>(x, Bind, Project, Reducer)).Run(st, s, x);
    }

    record Binder<S>(E Value, Func<A, Transducer<E, B>> Bind, Func<A, B, C> Project, Reducer<C, S> Reducer) :
        Reducer<A, S>
    {
        public override TResult<S> Run(TState st, S s, A a) =>
            Bind(a) switch
            {
                TailTransducer<E, B> tail when typeof(B) == typeof(C) =>
                    TResult.Recursive(st, s, Value, tail.Recursive.Transform((Reducer<B, S>)(object)Reducer)),

                var b =>
                    TResult.Recursive(st, s, Value, b.Transform(new Projector<S>(a, Project, Reducer)))
            };
    }
    
    record Projector<S>(A Value, Func<A, B, C> Project, Reducer<C, S> Reducer) :
        Reducer<B, S>
    {
        public override TResult<S> Run(TState st, S s, B c) =>
          Reducer.Run(st, s, Project(Value, c));
    }
}

record SelectManyTransducer2<E, A, B, C>(
    Transducer<E, A> F, 
    Transducer<A, Transducer<E, B>> BindF, 
    Func<A, B, C> Project) : 
    Transducer<E, C>
{
    public Reducer<E, S> Transform<S>(Reducer<C, S> reduce) =>
        new Reduce<S>(F, BindF, Project, reduce);

    public Transducer<E, C> Morphism =>
        this;

    record Reduce<S>(Transducer<E, A> F, Transducer<A, Transducer<E, B>> Bind, Func<A, B, C> Project, Reducer<C, S> Reducer) : 
        Reducer<E, S>
    {
        public override TResult<S> Run(TState st, S s, E x) =>
            F.Transform(new Binder<S>(x, Bind, Project, Reducer)).Run(st, s, x);
    }

    record Binder<S>(E Value, Transducer<A, Transducer<E, B>> Bind, Func<A, B, C> Project, Reducer<C, S> Reducer) :
        Reducer<A, S>
    {
        public override TResult<S> Run(TState st, S s, A b) =>
            Bind.Transform(new BindApply<S>(Value, b, Project, Reducer)).Run(st, s, b);
    }

    record BindApply<S>(E ValueX, A ValueY, Func<A, B, C> Project, Reducer<C, S> Reducer) : Reducer<Transducer<E, B>, S>
    {
        public override TResult<S> Run(TState st, S s, Transducer<E, B> t) =>
            t switch
            {
                TailTransducer<E, B> tail when typeof(B) == typeof(C) =>
                    TResult.Recursive(st, s, ValueX, tail.Recursive.Transform((Reducer<B, S>)(object)Reducer)),

                var b =>
                    TResult.Recursive(st, s, ValueX, b.Transform(new Projector<S>(ValueY, Project, Reducer)))
            };
    }

    record Projector<S>(A Value, Func<A, B, C> Project, Reducer<C, S> Reducer) :
        Reducer<B, S>
    {
        public override TResult<S> Run(TState st, S s, B c) =>
            Reducer.Run(st, s, Project(Value, c));
    }
}

record SelectManySumTransducer1<E, X, A, B, C>(
    Transducer<E, Sum<X, A>> F, 
    Func<A, Transducer<E, Sum<X, B>>> BindF, 
    Func<A, B, C> Project) : 
    Transducer<E, Sum<X, C>>
{
    public Reducer<E, S> Transform<S>(Reducer<Sum<X, C>, S> reduce) =>
        new Reduce<S>(F, BindF, Project, reduce);
    
    public Transducer<E, Sum<X, C>> Morphism =>
        this;

    record Reduce<S>(Transducer<E, Sum<X, A>> F, Func<A, Transducer<E, Sum<X, B>>> Bind, Func<A, B, C> Project, Reducer<Sum<X, C>, S> Reducer) : 
        Reducer<E, S>
    {
        public override TResult<S> Run(TState st, S s, E x) =>
            F.Transform(new Binder<S>(x, Bind, Project, Reducer)).Run(st, s, x);
    }

    record Binder<S>(E Value, Func<A, Transducer<E, Sum<X, B>>> Bind, Func<A, B, C> Project, Reducer<Sum<X, C>, S> Reducer) :
        Reducer<Sum<X, A>, S>
    {
        public override TResult<S> Run(TState st, S s, Sum<X, A> ma) =>
            ma switch
            {
                SumRight<X, A> r =>
                    Bind(r.Value) switch
                    {
                        TailTransducer<E, Sum<X, B>> tail when typeof(B) == typeof(C) => 
                            TResult.Recursive(st, s, Value, tail.Recursive.Transform((Reducer<Sum<X, B>, S>)(object)Reducer)),
        
                        var b => 
                            TResult.Recursive(st, s, Value, b.Transform(new Projector<S>(r.Value, Project, Reducer)))
                    },
                
                SumLeft<X, A> l =>
                    Reducer.Run(st, s, Sum<X, C>.Left(l.Value)),
                
                _ => TResult.Complete(s)
            };
    }
    
    record Projector<S>(A Value, Func<A, B, C> Project, Reducer<Sum<X, C>, S> Reducer) :
        Reducer<Sum<X, B>, S>
    {
        public override TResult<S> Run(TState st, S s, Sum<X, B> mb) =>
            mb switch
            {
                SumRight<X, B> r =>
                    Reducer.Run(st, s, Sum<X, C>.Right(Project(Value, r.Value))),

                SumLeft<X, B> l =>
                    Reducer.Run(st, s, Sum<X, C>.Left(l.Value)),

                _ => TResult.Complete(s)
            };
    }
}
