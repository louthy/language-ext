/*
using System;
using LanguageExt.DSL;
using LanguageExt.Traits;

namespace LanguageExt;

record IOMap<A, B>(IO<A> Value, Func<A, B> F) : IO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) =>
        new IOMap<A, C>(Value, x => f(F(x)));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) =>
        new IOMap<A, K<IO, C>>(Value, x => f(F(x)).As()).Bind(x => x);

    public override IO<C> ApplyBack<C>(K<IO, Func<B, C>> mf) =>
        mf switch
        {
            IOPure<Func<B, C>> (var f)       => new IOMap<A, C>(Value, x => f(F(x))),
            IOPureAsync<Func<B, C>> (var tf) => new IOMap<A, IO<C>>(Value, x => IO.pureAsync(tf.Map(f => f(F(x))))).Bind(x => x),  
            IOFail<Func<B, C>> (var v)       => new IOFail<C>(v),
            IOBind<Func<B, C>> (var f)       => new IOBind<C>(f.Map(x => x.Apply(this).As())),
            IOCatch<Func<B, C>> mc           => Value.Bind(a => IO.pure(F(a)).ApplyBack(mc)),
            _                                => throw new InvalidOperationException()
        };
}
*/
