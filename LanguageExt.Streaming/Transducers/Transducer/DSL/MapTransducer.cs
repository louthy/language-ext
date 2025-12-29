using System;

namespace LanguageExt;

record MapTransducer<A, B>(Func<A, B> F) : Transducer<A, B> 
{
    public override ReducerIO<A, S> Reduce<S>(ReducerIO<B, S> reducer) =>
        (s, x) => reducer(s, F(x));
}
