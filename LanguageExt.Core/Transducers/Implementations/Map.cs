#nullable enable
using System;

namespace LanguageExt.Transducers;

record MapTransducer<A, B, C>(Transducer<A, B> F, Func<B, C> G) : 
    Transducer<A, C>
{
    public override Reducer<A, S> Transform<S>(Reducer<C, S> reduce) =>
        F.Transform(new Mapper<S>(G, reduce));
    
    record Mapper<S>(Func<B, C> G, Reducer<C, S> Reducer) : Reducer<B, S>
    {
        public override TResult<S> Run(TState st, S s, B b) =>
            Reducer.Run(st, s, G(b));
    }
            
    public override string ToString() =>  
        "map";
}
