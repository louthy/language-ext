using System;
using LanguageExt.DSL;
using LanguageExt.Traits;

namespace LanguageExt;

record IOLift<A>(IODsl<IO<A>> Value) : IO<A>
{
    public override IO<B> Map<B>(Func<A, B> f) =>
        new IOLift<B>(Value.Map(fa => fa.Map(f)));

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) =>
        new IOLift<B>(Value.Map(mx => mx.Bind(f)));
}
