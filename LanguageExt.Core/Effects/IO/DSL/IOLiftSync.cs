using System;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

record IOLiftSync<A, B>(Func<EnvIO, A> F, Func<A, K<IO, B>> Next) : InvokeSyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOLiftSync<A, C>(F, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOLiftSync<A, C>(F, x => Next(x).Bind(f));

    public override IO<B> Invoke(EnvIO envIO) =>
        Next(F(envIO)).As();
}
