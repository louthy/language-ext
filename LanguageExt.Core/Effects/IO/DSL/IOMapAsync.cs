using System;
using System.Threading.Tasks;

namespace LanguageExt.DSL;

record IOMapAsync<A, B>(Task<A> Value, Func<A, B> Ff) : DslInvokeIOAsync<B>
{
    public override IODsl<C> Map<C>(Func<B, C> f) =>
        new IOMapAsync<A, B, C>(Value, Ff, f);

    public override async ValueTask<B> Invoke(EnvIO envIO) =>
        Ff(await Value);
}

record IOMapAsync<A, B, C>(Task<A> Value, Func<A, B> Ff, Func<B, C> Fg) : DslInvokeIOAsync<C>
{
    public override IODsl<D> Map<D>(Func<C, D> f) =>
        new IOMapAsync<A, B, C, D>(Value, Ff, Fg, f);

    public override async ValueTask<C> Invoke(EnvIO envIO) =>
        Fg(Ff(await Value));
}

record IOMapAsync<A, B, C, D>(Task<A> Value, Func<A, B> Ff, Func<B, C> Fg, Func<C, D> Fh) : DslInvokeIOAsync<D>
{
    public override IODsl<E> Map<E>(Func<D, E> f) =>
        new IOMapAsync<A, B, C, D, E>(Value, Ff, Fg, Fh, f);

    public override async ValueTask<D> Invoke(EnvIO envIO) =>
        Fh(Fg(Ff(await Value)));
}

record IOMapAsync<A, B, C, D, E>(Task<A> Value, Func<A, B> Ff, Func<B, C> Fg, Func<C, D> Fh, Func<D, E> Fi) : DslInvokeIOAsync<E>
{
    public override IODsl<F> Map<F>(Func<E, F> f) =>
        new IOMapAsync<A, B, C, D, F>(Value, Ff, Fg, Fh, x => f(Fi(x)));

    public override async ValueTask<E> Invoke(EnvIO envIO) =>
        Fi(Fh(Fg(Ff(await Value))));
}
