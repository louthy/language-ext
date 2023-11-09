#nullable enable
using System;

namespace LanguageExt.Transducers;


record SelectManyTransducer2<A, B, C, D>(
    Transducer<A, B> F, 
    Func<B, Transducer<A, C>> BindF, 
    Func<B, C, D> Project) : 
    Transducer<A, D>
{
    public Reducer<S, A> Transform<S>(Reducer<S, D> reduce) =>
        new Reduce<S>(F, BindF, Project, reduce);
    
    public Transducer<A, D> Morphism =>
        this;

    record Reduce<S>(Transducer<A, B> F, Func<B, Transducer<A, C>> Bind, Func<B, C, D> Project, Reducer<S, D> Reducer) : 
        Reducer<S, A>
    {
        public override TResult<S> Run(TState st, S s, A x) =>
            F.Transform(new Binder<S>(x, Bind, Project, Reducer)).Run(st, s, x);
    }

    record Binder<S>(A Value, Func<B, Transducer<A, C>> Bind, Func<B, C, D> Project, Reducer<S, D> Reducer) :
        Reducer<S, B>
    {
        public override TResult<S> Run(TState st, S s, B b) =>
            TResult.Recursive(st, s, Value, Bind(b).Transform(new Projector<S>(b, Project, Reducer)));
    }
    
    record Projector<S>(B Value, Func<B, C, D> Project, Reducer<S, D> Reducer) :
        Reducer<S, C>
    {
        public override TResult<S> Run(TState st, S s, C c) =>
          Reducer.Run(st, s, Project(Value, c));
    }
}

record SelectManyTransducer3<A, B, C, D>(
    Transducer<A, B> F, 
    Transducer<B, Transducer<A, C>> BindF, 
    Func<B, C, D> Project) : 
    Transducer<A, D>
{
    public Reducer<S, A> Transform<S>(Reducer<S, D> reduce) =>
        new Reduce<S>(F, BindF, Project, reduce);

    public Transducer<A, D> Morphism =>
        this;

    record Reduce<S>(Transducer<A, B> F, Transducer<B, Transducer<A, C>> Bind, Func<B, C, D> Project, Reducer<S, D> Reducer) : 
        Reducer<S, A>
    {
        public override TResult<S> Run(TState st, S s, A x) =>
            F.Transform(new Binder<S>(x, Bind, Project, Reducer)).Run(st, s, x);
    }

    record Binder<S>(A Value, Transducer<B, Transducer<A, C>> Bind, Func<B, C, D> Project, Reducer<S, D> Reducer) :
        Reducer<S, B>
    {
        public override TResult<S> Run(TState st, S s, B b) =>
            Bind.Transform(new BindApply<S>(Value, b, Project, Reducer)).Run(st, s, b);
    }

    record BindApply<S>(A ValueX, B ValueY, Func<B, C, D> Project, Reducer<S, D> Reducer) : Reducer<S, Transducer<A, C>>
    {
        public override TResult<S> Run(TState st, S s, Transducer<A, C> t) =>
            TResult.Recursive(st, s, ValueX, t.Transform(new Projector<S>(ValueY, Project, Reducer)));
    }

    record Projector<S>(B Value, Func<B, C, D> Project, Reducer<S, D> Reducer) :
        Reducer<S, C>
    {
        public override TResult<S> Run(TState st, S s, C c) =>
            Reducer.Run(st, s, Project(Value, c));
    }
}
