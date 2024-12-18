using System;
using LanguageExt.Common;
using LanguageExt.DSL;
using LanguageExt.Traits;

namespace LanguageExt;

record IOFail<A>(Error Value) : IO<A>
{
    public override IO<B> Map<B>(Func<A, B> f) =>
        new IOFail<B>(Value);

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) =>
        IO.fail<B>(Value);

    public override IO<B> ApplyBack<B>(K<IO, Func<A, B>> mf) =>
        mf switch
        {
            IOPure<Func<A, B>>         => new IOFail<B>(Value),
            IOPureAsync<Func<A, B>>    => new IOFail<B>(Value),
            IOFail<Func<A, B>> (var v) => new IOFail<B>(v + Value),
            IOBind<Func<A, B>> (var f) => new IOBind<B>(f.Map(x => x.Apply(this).As())),
            IOCatch<Func<A, B>>        => new IOFail<B>(Value),
            _                          => throw new InvalidOperationException()
        };
}
