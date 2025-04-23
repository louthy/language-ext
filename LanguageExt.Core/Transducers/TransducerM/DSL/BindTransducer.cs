using System;
using LanguageExt.Traits;

namespace LanguageExt;

record BindTransducerM1<M, Env, A, B>(TransducerM<M, Env, A> First, Func<A, K<TransduceFromM<M, Env>, B>> F) : 
    TransducerM<M, Env, B> 
{
    public override ReducerM<M, Env, S> Reduce<S>(ReducerM<M, B, S> reducer) => 
        (s, env) => First.Reduce<S>(
            (s1, x) =>
                F(x).As().Reduce(reducer)(s1, env))(s, env);
}

record BindTransducerM2<M, Env, A, B>(TransducerM<M, Env, A> First, Func<A, TransducerM<M, Env, B>> F) : 
    TransducerM<M, Env, B> 
{
    public override ReducerM<M, Env, S> Reduce<S>(ReducerM<M, B, S> reducer) => 
        (s, env) => First.Reduce<S>(
            (s1, x) =>
                F(x).Reduce(reducer)(s1, env))(s, env);
}
