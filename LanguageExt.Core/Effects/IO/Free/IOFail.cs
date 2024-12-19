using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

record IOFail<A>(Error Value) : InvokeSync<A>
{
    public override IO<B> Map<B>(Func<A, B> f) =>
        new IOFail<B>(Value);

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) => 
        new IOFail<B>(Value);

    public override A Invoke(EnvIO envIO) => 
        Value.Throw<A>();
}
