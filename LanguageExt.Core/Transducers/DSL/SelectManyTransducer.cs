using System;
using LanguageExt.Traits;

namespace LanguageExt;

record SelectManyTransducer1<Env, A, B, C>(Transducer<Env, A> First, Func<A, K<Transducer<Env>, B>> F, Func<A, B, C> G) : 
    Transducer<Env, C> 
{
    public override Reducer<Env, S> Reduce<S>(Reducer<C, S> reducer) =>
        (s, env) => First.Reduce<S>(
            (s1, x) =>
                F(x).As().Reduce<S>((s2, y) => reducer(s2, G(x, y)))(s1, env))(s, env);

    public override ReducerM<M, Env, S> ReduceM<M, S>(ReducerM<M, C, S> reducer) => 
        (s, env) => First.ReduceM<M, S>(
            (s1, x) =>
                F(x).As().ReduceM<M, S>((s2, y) => reducer(s2, G(x, y)))(s1, env))(s, env);

}

record SelectManyTransducer2<Env, A, B, C>(Transducer<Env, A> First, Func<A, Transducer<Env, B>> F, Func<A, B, C> G) : 
    Transducer<Env, C> 
{
    public override Reducer<Env, S> Reduce<S>(Reducer<C, S> reducer) =>
        (s, env) => First.Reduce<S>(
            (s1, x) =>
                F(x).Reduce<S>((s2, y) => reducer(s2, G(x, y)))(s1, env))(s, env);

    public override ReducerM<M, Env, S> ReduceM<M, S>(ReducerM<M, C, S> reducer) => 
        (s, env) => First.ReduceM<M, S>(
            (s1, x) =>
                F(x).ReduceM<M, S>((s2, y) => reducer(s2, G(x, y)))(s1, env))(s, env);

}
