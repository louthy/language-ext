using System;
using LanguageExt.DSL;
using LanguageExt.Traits;

namespace LanguageExt;

record IOPure<A>(A Value) : IO<A>
{
    public override IO<B> Map<B>(Func<A, B> f) =>
        new IOPure<B>(f(Value));

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) =>
        f(Value).As();

    public override IO<B> ApplyBack<B>(K<IO, Func<A, B>> mf) =>
        mf switch
        {
            IOPure<Func<A, B>> (var f)       => new IOPure<B>(f(Value)),
            IOPureAsync<Func<A, B>> (var tf) => new IOPureAsync<B>(tf.Map(f => f(Value))),
            IOFail<Func<A, B>> (var v)       => new IOFail<B>(v),
            IOBind<Func<A, B>> (var f)       => new IOBind<B>(f.Map(x => x.Apply(this).As())),
            IOCatch<Func<A, B>> mc           => mc.Map(f => f(Value)),
            _                                => throw new InvalidOperationException()
        };
}
