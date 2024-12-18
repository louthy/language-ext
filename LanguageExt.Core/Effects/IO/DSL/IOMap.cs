using System;
using System.Threading.Tasks;

namespace LanguageExt.DSL;

abstract record IOMap<A> : IODsl<A>
{
    public abstract A Invoke(EnvIO envIO);
}

abstract record IOMapAsync<A> : IODsl<A>
{
    public abstract ValueTask<A> Invoke(EnvIO envIO);
}

record IOMap<A, B>(A Value, Func<A, B> F) : IOMap<B>
{
    public override IODsl<C> Map<C>(Func<B, C> g) =>
        new IOMap<A, C>(Value, x => g(F(x)));

    public override B Invoke(EnvIO envIO) =>
        F(Value);
}

record IOMapAsync<A, B>(Task<A> Value, Func<A, B> F) : IOMapAsync<B>
{
    public override IODsl<C> Map<C>(Func<B, C> g) =>
        new IOMapAsync<A, C>(Value, x => g(F(x)));

    public override async ValueTask<B> Invoke(EnvIO envIO) =>
        F(await Value);
}
