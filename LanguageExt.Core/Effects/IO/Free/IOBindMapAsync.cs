using System;
using System.Threading.Tasks;
using LanguageExt.DSL;
using LanguageExt.Traits;

namespace LanguageExt;

record IOBindMapAsync<A, B, C>(Task<A> Value, Func<A, K<IO, B>> Ff, Func<B, C> Fg) : InvokeAsyncIO<C>
{
    public override IO<D> Map<D>(Func<C, D> f) => 
        new IOBindMapAsync<A, B, C, D>(Value, Ff, Fg, f);

    public override IO<D> Bind<D>(Func<C, K<IO, D>> f) => 
        new IOBindMapAsync2<A, B, C, D>(Value, Ff, Fg, f);

    public override async ValueTask<IO<C>> Invoke(EnvIO envIO) =>
        Ff(await Value).As().Map(Fg);
}

record IOBindMapAsync<A, B, C, D>(Task<A> Value, Func<A, K<IO, B>> Ff, Func<B, C> Fg, Func<C, D> Fh) : InvokeAsyncIO<D>
{
    public override IO<E> Map<E>(Func<D, E> f) =>
        new IOBindMapAsync<A, B, C, E>(Value, Ff, Fg, x => f(Fh(x)));

    public override IO<E> Bind<E>(Func<D, K<IO, E>> f) => 
        new IOBindMapAsync2<A, B, C, E>(Value, Ff, Fg, x => f(Fh(x)));

    public override async ValueTask<IO<D>> Invoke(EnvIO envIO) =>
        Ff(await Value).As().Map(Fg).Map(Fh);
}

record IOBindMapAsync2<A, B, C, D>(Task<A> Value, Func<A, K<IO, B>> Ff, Func<B, C> Fg, Func<C, K<IO, D>> Fh) : InvokeAsyncIO<D>
{
    public override IO<E> Map<E>(Func<D, E> f) =>
        new IOBindMapAsync2<A, B, C, E>(Value, Ff, Fg, x => Fh(x).Map(f));

    public override IO<E> Bind<E>(Func<D, K<IO, E>> f) => 
        new IOBindMapAsync2<A, B, C, E>(Value, Ff, Fg, x => Fh(x).As().Bind(f));

    public override async ValueTask<IO<D>> Invoke(EnvIO envIO) =>
        Ff(await Value).As().Map(Fg).Bind(Fh);
}
