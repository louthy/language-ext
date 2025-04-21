using System;
using LanguageExt.Traits;

namespace LanguageExt;

record BindTransducer1<Env, A, B>(Transducer<Env, A> First, Func<A, K<TransduceFrom<Env>, B>> F) : 
    Transducer<Env, B> 
{
    public override ReducerAsync<Env, S> Reduce<S>(ReducerAsync<B, S> reducer) =>
        (s, env) => First.Reduce<S>(
            (s1, x) =>
                F(x).As().Reduce(reducer)(s1, env))(s, env);

    public override ReducerM<M, Env, S> ReduceM<M, S>(ReducerM<M, B, S> reducer) => 
        (s, env) => First.ReduceM<M, S>(
            (s1, x) =>
                F(x).As().ReduceM(reducer)(s1, env))(s, env);
}

record BindTransducer2<Env, A, B>(Transducer<Env, A> First, Func<A, Transducer<Env, B>> F) : 
    Transducer<Env, B> 
{
    public override ReducerAsync<Env, S> Reduce<S>(ReducerAsync<B, S> reducer) =>
        (s, env) => First.Reduce<S>(
            (s1, x) =>
                F(x).Reduce(reducer)(s1, env))(s, env);

    public override ReducerM<M, Env, S> ReduceM<M, S>(ReducerM<M, B, S> reducer) => 
        (s, env) => First.ReduceM<M, S>(
            (s1, x) =>
                F(x).ReduceM(reducer)(s1, env))(s, env);
}
