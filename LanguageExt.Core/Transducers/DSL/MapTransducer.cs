using System;

namespace LanguageExt;

record MapTransducer<A, B>(Func<A, B> F) : Transducer<A, B> 
{
    public override Reducer<A, S> Reduce<S>(Reducer<B, S> reducer) =>
        (s, x) => reducer(s, F(x));
}
