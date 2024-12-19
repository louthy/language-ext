using System;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

record IOMap<A, B, C>(Func<A, B> Ff, IO<A> Fa, Func<B, K<IO, C>> Next) : InvokeAsyncIO<C>
{
    public override IO<D> Map<D>(Func<C, D> f) => 
        new IOMap<A, B, D>(Ff, Fa, x => Next(x).As().Map(f));

    public override IO<D> Bind<D>(Func<C, K<IO, D>> f) => 
        new IOMap<A, B, D>(Ff, Fa, x => Next(x).As().Bind(f));

    public override async ValueTask<IO<C>> Invoke(EnvIO envIO)
    {
        var a = await Fa.RunAsync(envIO);
        return Next(Ff(a)).As();
    }
}

record IOPureMap<A, B, C>(Func<A, B> Ff, A Fa, Func<B, K<IO, C>> Next) : InvokeSyncIO<C>
{
    public override IO<D> Map<D>(Func<C, D> f) => 
        new IOPureMap<A, B, D>(Ff, Fa, x => Next(x).As().Map(f));

    public override IO<D> Bind<D>(Func<C, K<IO, D>> f) => 
        new IOPureMap<A, B, D>(Ff, Fa, x => Next(x).As().Bind(f));

    public override IO<C> Invoke(EnvIO envIO) =>
        Next(Ff(Fa)).As();
}

record IOPureMapAsync<A, B, C>(Func<A, B> Ff, Task<A> Fa, Func<B, K<IO, C>> Next) : InvokeAsyncIO<C>
{
    public override IO<D> Map<D>(Func<C, D> f) => 
        new IOPureMapAsync<A, B, D>(Ff, Fa, x => Next(x).As().Map(f));

    public override IO<D> Bind<D>(Func<C, K<IO, D>> f) => 
        new IOPureMapAsync<A, B, D>(Ff, Fa, x => Next(x).As().Bind(f));

    public override async ValueTask<IO<C>> Invoke(EnvIO envIO) =>
        Next(Ff(await Fa)).As();
}
