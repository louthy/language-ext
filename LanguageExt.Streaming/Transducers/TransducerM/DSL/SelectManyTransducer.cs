using System;
using LanguageExt.Traits;

namespace LanguageExt;

record SelectManyTransducerM1<M, Env, A, B, C>(TransducerM<M, Env, A> First, Func<A, K<TransduceFromM<M, Env>, B>> F, Func<A, B, C> G) : 
    TransducerM<M, Env, C> 
{
    public override ReducerM<M, Env, S> Reduce<S>(ReducerM<M, C, S> reducer) => 
        (s, env) => First.Reduce<S>(
            (s1, x) =>
                F(x).As().Reduce<S>((s2, y) => reducer(s2, G(x, y)))(s1, env))(s, env);

}

record SelectManyTransducerM2<M, Env, A, B, C>(TransducerM<M, Env, A> First, Func<A, TransducerM<M, Env, B>> F, Func<A, B, C> G) : 
    TransducerM<M, Env, C> 
{
    public override ReducerM<M, Env, S> Reduce<S>(ReducerM<M, C, S> reducer) => 
        (s, env) => First.Reduce<S>(
            (s1, x) =>
                F(x).Reduce<S>((s2, y) => reducer(s2, G(x, y)))(s1, env))(s, env);

}
