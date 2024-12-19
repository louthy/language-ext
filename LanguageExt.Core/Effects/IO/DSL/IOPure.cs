using System;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

record IOPure<A>(A Value) : InvokeSync<A>
{
    public override IO<B> Map<B>(Func<A, B> f) =>
        new IOPureMap<A, B, B>(f, Value, IO.pure);

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) =>
        new IOBind<A, B>(Value, f);

    public override A Invoke(EnvIO envIO) => 
        Value;
}
