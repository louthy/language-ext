#nullable enable
using System;

namespace LanguageExt.Transducers;

record LiftTransducer1<A, B>(Func<A, TResult<B>> F) : Transducer<A, B>
{
    public Reducer<S, A> Transform<S>(Reducer<S, B> reduce) =>
        new Reduce<S>(F, reduce);

    record Reduce<S>(Func<A, TResult<B>> F, Reducer<S, B> Reducer) : Reducer<S, A>
    {
        public override TResult<S> Run(TState st, S s, A x) =>
            F(x).Reduce(st, s, Reducer);
    }
    
    public Transducer<A, B> Morphism =>
        this;

    public override string ToString() =>
        "lift";
}

record LiftTransducer2<A>(Func<TResult<A>> F) : Transducer<Unit, A>
{
    public Reducer<S, Unit> Transform<S>(Reducer<S, A> reduce) =>
        new Reduce<S>(F, reduce);

    record Reduce<S>(Func<TResult<A>> F, Reducer<S, A> Reducer) : Reducer<S, Unit>
    {
        public override TResult<S> Run(TState st, S s, Unit x) =>
            F().Reduce(st, s, Reducer);
    }
    
    public Transducer<Unit, A> Morphism =>
        this;

    public override string ToString() =>
        "lift";
}

record LiftTransducer3<A, B>(Func<A, B> F) : Transducer<A, B>
{
    public Reducer<S, A> Transform<S>(Reducer<S, B> reduce) =>
        new Reduce<S>(F, reduce);

    record Reduce<S>(Func<A, B> F, Reducer<S, B> Reducer) : Reducer<S, A>
    {
        public override TResult<S> Run(TState st, S s, A x) =>
            Reducer.Run(st, s, F(x));
    }
    
    public Transducer<A, B> Morphism =>
        this;
    
    public override string ToString() =>
        "lift";

}

record LiftTransducer4<A>(Func<A> F) : Transducer<Unit, A>
{
    public Reducer<S, Unit> Transform<S>(Reducer<S, A> reduce) =>
        new Reduce<S>(F, reduce);

    record Reduce<S>(Func<A> F, Reducer<S, A> Reducer) : Reducer<S, Unit>
    {
        public override TResult<S> Run(TState st, S s, Unit x) =>
            Reducer.Run(st, s, F());
    }
    
    public Transducer<Unit, A> Morphism =>
        this;
    
    public override string ToString() =>
        "lift";
}
