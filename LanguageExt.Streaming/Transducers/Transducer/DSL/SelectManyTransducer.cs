using System;
using LanguageExt.Traits;

namespace LanguageExt;

record SelectManyTransducer1<Env, A, B, C>(
    Transducer<Env, A> First,
    Func<A, K<TransduceFrom<Env>, B>> F,
    Func<A, B, C> G) :
    Transducer<Env, C>
{
    public override ReducerIO<Env, S> Reduce<S>(ReducerIO<C, S> reducer) =>
        (s, env) => First.Reduce<S>((s1, x) =>
                                        F(x).As().Reduce<S>((s2, y) => reducer(s2, G(x, y)))(s1, env))(s, env);
}

record SelectManyTransducer2<Env, A, B, C>(Transducer<Env, A> First, Func<A, Transducer<Env, B>> F, Func<A, B, C> G) : 
    Transducer<Env, C> 
{
    public override ReducerIO<Env, S> Reduce<S>(ReducerIO<C, S> reducer) =>
        (s, env) => First.Reduce<S>(
            (s1, x) =>
                F(x).Reduce<S>((s2, y) => reducer(s2, G(x, y)))(s1, env))(s, env);
}
