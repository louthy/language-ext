#nullable enable
using System;

namespace LanguageExt.Transducers;

record LiftTransducer1<A, B>(Func<A, TResult<B>> F) : Transducer<A, B>
{
    public override Reducer<A, S> Transform<S>(Reducer<B, S> reduce) =>
        new Reduce<S>(F, reduce);

    record Reduce<S>(Func<A, TResult<B>> F, Reducer<B, S> Reducer) : Reducer<A, S>
    {
        public override TResult<S> Run(TState st, S s, A x) =>
            F(x).Reduce(st, s, Reducer);
    }

    public override string ToString() =>
        "lift";
}

record LiftTransducer2<A>(Func<TResult<A>> F) : Transducer<Unit, A>
{
    public override Reducer<Unit, S> Transform<S>(Reducer<A, S> reduce) =>
        new Reduce<S>(F, reduce);

    record Reduce<S>(Func<TResult<A>> F, Reducer<A, S> Reducer) : Reducer<Unit, S>
    {
        public override TResult<S> Run(TState st, S s, Unit x) =>
            F().Reduce(st, s, Reducer);
    }

    public override string ToString() =>
        "lift";
}

record LiftTransducer3<A, B>(Func<A, B> F) : Transducer<A, B>
{
    public override Reducer<A, S> Transform<S>(Reducer<B, S> reduce) =>
        new Reduce<S>(F, reduce);

    record Reduce<S>(Func<A, B> F, Reducer<B, S> Reducer) : Reducer<A, S>
    {
        public override TResult<S> Run(TState st, S s, A x) =>
            Reducer.Run(st, s, F(x));
    }
    
    public override string ToString() =>
        "lift";

}

record LiftTransducer4<A>(Func<A> F) : Transducer<Unit, A>
{
    public override Reducer<Unit, S> Transform<S>(Reducer<A, S> reduce) =>
        new Reduce<S>(F, reduce);

    record Reduce<S>(Func<A> F, Reducer<A, S> Reducer) : Reducer<Unit, S>
    {
        public override TResult<S> Run(TState st, S s, Unit x) =>
            Reducer.Run(st, s, F());
    }
    
    public override string ToString() =>
        "lift";
}
