using System;
using System.Threading.Tasks;
using LanguageExt.DSL;
using LanguageExt.Traits;

namespace LanguageExt;

abstract record IOBind<A> : IO<A>
{
    public abstract IO<A> Invoke(EnvIO envIO);
}

abstract record IOBindAsync<A> : IO<A>
{
    public abstract ValueTask<IO<A>> Invoke(EnvIO envIO);
}

record IOBind<A, B>(A Value, Func<A, K<IO, B>> F) : IOBind<B>
{
    public override IO<C> Map<C>(Func<B, C> f) =>
        new IOBind<A, C>(Value, x => F(x).As().Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) =>
        new IOBind<A, C>(Value, x => F(x).As().Bind(f));

    public override IO<B> Invoke(EnvIO envIO) =>
        F(Value).As();
}

record IOBindAsync<A, B>(Task<A> Value, Func<A, K<IO, B>> F) : IOBindAsync<B>
{
    public override IO<C> Map<C>(Func<B, C> f) =>
        new IOBindAsync<A, C>(Value, x => F(x).As().Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) =>
        new IOBindAsync<A, C>(Value, x => F(x).As().Bind(f));

    public override async ValueTask<IO<B>> Invoke(EnvIO envIO) =>
        F(await Value).As();
}
