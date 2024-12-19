using System;

namespace LanguageExt.DSL;

record IOLiftSync<A>(Func<EnvIO, A> F) : DslInvokeIO<A>
{
    public override IODsl<B> Map<B>(Func<A, B> f) =>
        new IOLiftSync<B>(x => f(F(x)));

    public override A Invoke(EnvIO envIO) =>
        F(envIO);
}
