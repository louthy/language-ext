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
        new IOMap<A, B, C>(Value, F, g);

    public override B Invoke(EnvIO envIO) =>
        F(Value);
}

record IOMap<A, B, C>(A Value, Func<A, B> F, Func<B, C> G) : IOMap<C>
{
    public override IODsl<D> Map<D>(Func<C, D> h) =>
        new IOMap<A, B, C, D>(Value, F, G, h);

    public override C Invoke(EnvIO envIO) =>
        G(F(Value));
}

record IOMap<A, B, C, D>(A Value, Func<A, B> F, Func<B, C> G, Func<C, D> H) : IOMap<D>
{
    public override IODsl<E> Map<E>(Func<D, E> f) =>
        new IOMap<A, B, C, E>(Value, F, G, x => f(H(x)));

    public override D Invoke(EnvIO envIO) =>
        H(G(F(Value)));
}

record IOMapAsync<A, B>(Task<A> Value, Func<A, B> F) : IOMapAsync<B>
{
    public override IODsl<C> Map<C>(Func<B, C> g) =>
        new IOMapAsync<A, B, C>(Value, F, g);

    public override async ValueTask<B> Invoke(EnvIO envIO) =>
        F(await Value);
}

record IOMapAsync<A, B, C>(Task<A> Value, Func<A, B> F, Func<B, C> G) : IOMapAsync<C>
{
    public override IODsl<D> Map<D>(Func<C, D> h) =>
        new IOMapAsync<A, B, C, D>(Value, F, G, h);

    public override async ValueTask<C> Invoke(EnvIO envIO) =>
        G(F(await Value));
}

record IOMapAsync<A, B, C, D>(Task<A> Value, Func<A, B> F, Func<B, C> G, Func<C, D> H) : IOMapAsync<D>
{
    public override IODsl<E> Map<E>(Func<D, E> f) =>
        new IOMapAsync<A, B, C, E>(Value, F, G, x => f(H(x)));

    public override async ValueTask<D> Invoke(EnvIO envIO) =>
        H(G(F(await Value)));
}
