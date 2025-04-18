using System;

namespace LanguageExt;

record MapTransducer<A, B>(Func<A, B> F) : Transducer<A, B> 
{
    public override ReducerAsync<A, S> Reduce<S>(ReducerAsync<B, S> reducer) =>
        (s, x) => reducer(s, F(x));

    public override ReducerM<M, A, S> ReduceM<M, S>(ReducerM<M, B, S> reducer) => 
        (s, x) => reducer(s, F(x));
}
