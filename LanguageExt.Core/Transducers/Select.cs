#nullable enable
using System;

namespace LanguageExt.Transducers;

record SelectTransducer<A, B, C>(Transducer<A, B> F, Func<B, C> G) : 
    Transducer<A, C>
{
    public Reducer<S, A> Transform<S>(Reducer<S, C> reduce) =>
        F.Transform(new Mapper<S>(G, reduce));
    
    public Transducer<A, C> Morphism =>
        this;

    record Mapper<S>(Func<B, C> G, Reducer<S, C> Reducer) :
        Reducer<S, B>
    {
        public override TResult<S> Run(TState st, S s, B b) =>
            Reducer.Run(st, s, G(b));
    }
}
