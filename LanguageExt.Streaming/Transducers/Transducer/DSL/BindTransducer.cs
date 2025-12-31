using System;
using LanguageExt.Traits;

namespace LanguageExt;

record BindTransducer1<Env, A, B>(Transducer<Env, A> First, Func<A, K<TransduceFrom<Env>, B>> F) : 
    Transducer<Env, B> 
{
    public override ReducerIO<Env, S> Reduce<S>(ReducerIO<B, S> reducer) =>
        (s, env) => First.Reduce<S>(
            (s1, x) =>
                F(x).As().Reduce(reducer)(s1, env))(s, env);

    public override TransducerM<M, Env, B> Lift<M>() =>
        new BindTransducerM1<M, Env, A, B>(First.Lift<M>(), x => F(x).As().Lift<M>());
}

record BindTransducer2<Env, A, B>(Transducer<Env, A> First, Func<A, Transducer<Env, B>> F) : 
    Transducer<Env, B> 
{
    public override ReducerIO<Env, S> Reduce<S>(ReducerIO<B, S> reducer) =>
        (s, env) => First.Reduce<S>(
            (s1, x) =>
                F(x).Reduce(reducer)(s1, env))(s, env);

    public override TransducerM<M, Env, B> Lift<M>() =>
        new BindTransducerM2<M, Env, A, B>(First.Lift<M>(), x => F(x).Lift<M>());
}
