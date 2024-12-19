using System;
using LanguageExt.Traits;

namespace LanguageExt;

record IOBind<A, B>(A Value, Func<A, K<IO, B>> F) : InvokeSyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) =>
        new IOBindMap<A, B, C>(Value, F, f);

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) =>
        new IOBindBind<A, B, C>(Value, F, f);

    public override IO<B> Invoke(EnvIO envIO) =>
        F(Value).As();
}
