using System;
using LanguageExt.Traits;

namespace LanguageExt;

record BindTransducer1<Env, A, B>(Transducer<Env, A> First, Func<A, K<Transducer<Env>, B>> F) : 
    Transducer<Env, B> 
{
    public override Reducer<Env, S> Reduce<S>(Reducer<B, S> reducer) =>
        (s, env) => First.Reduce<S>(
            (s1, x) =>
                F(x).As().Reduce(reducer)(s1, env))(s, env);
}

record BindTransducer2<Env, A, B>(Transducer<Env, A> First, Func<A, Transducer<Env, B>> F) : 
    Transducer<Env, B> 
{
    public override Reducer<Env, S> Reduce<S>(Reducer<B, S> reducer) =>
        (s, env) => First.Reduce<S>(
            (s1, x) =>
                F(x).Reduce(reducer)(s1, env))(s, env);
}
