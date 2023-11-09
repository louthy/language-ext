#nullable enable
using System;

namespace LanguageExt.Transducers;

record SelectTransducer<A, B, C>(Transducer<A, B> F, Func<B, C> G) : 
    Transducer<A, C>
{
    public Reducer<S, A> Transform<S>(Reducer<S, C> reduce) =>
        new Reduce<S>(F, G, reduce);
    
    public Transducer<A, C> Morphism =>
        this;

    record Reduce<S>(Transducer<A, B> F, Func<B, C> G, Reducer<S, C> Reducer) : 
        Reducer<S, A>
    {
        public override TResult<S> Run(TState st, S s, A x) =>
            F.Transform(new Mapper<S>(G, Reducer)).Run(st, s, x);
    }

    record Mapper<S>(Func<B, C> G, Reducer<S, C> Reducer) :
        Reducer<S, B>
    {
        public override TResult<S> Run(TState st, S s, B b) =>
            Reducer.Run(st, s, G(b));
    }
}
