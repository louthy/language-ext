using System;
using LanguageExt.Traits;

namespace LanguageExt;

record MapTransducerM<M, A, B>(Func<A, B> F) : TransducerM<M, A, B> 
{
    public override ReducerM<M, A, S> Reduce<S>(ReducerM<M, B, S> reducer) => 
        (s, x) => reducer(s, F(x));
}
