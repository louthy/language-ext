using System;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

record IOMap<A, B, C>(Func<A, B> Ff, IO<A> Fa, Func<B, K<IO, C>> Next) : InvokeSyncIO<C>
{
    public override IO<D> Map<D>(Func<C, D> f) => 
        new IOMap<A, B, D>(Ff, Fa, x => Next(x).Map(f));

    public override IO<D> Bind<D>(Func<C, K<IO, D>> f) => 
        new IOMap<A, B, D>(Ff, Fa, x => Next(x).Bind(f));

    public override IO<D> BindAsync<D>(Func<C, ValueTask<K<IO, D>>> f) => 
        new IOMap<A, B, D>(Ff, Fa, x => Next(x).As().BindAsync(f));

    public override IO<C> Invoke(EnvIO envIO)
    {
        var task = Fa.RunAsync(envIO);
        return task.IsCompleted
                   ? Next(Ff(task.Result)).As()
                   : new IOPureMapAsync<A, B, C>(Ff, task, Next);
    }
    
    public override string ToString() => 
        "IO map";
}

record IOPureMap<A, B, C>(Func<A, B> Ff, A Fa, Func<B, K<IO, C>> Next) : InvokeSyncIO<C>
{
    public override IO<D> Map<D>(Func<C, D> f) => 
        new IOPureMap<A, B, D>(Ff, Fa, x => Next(x).Map(f));

    public override IO<D> Bind<D>(Func<C, K<IO, D>> f) => 
        new IOPureMap<A, B, D>(Ff, Fa, x => Next(x).Bind(f));

    public override IO<D> BindAsync<D>(Func<C, ValueTask<K<IO, D>>> f) => 
        new IOPureMap<A, B, D>(Ff, Fa, x => Next(x).As().BindAsync(f));

    public override IO<C> Invoke(EnvIO envIO) =>
        Next(Ff(Fa)).As();
    
    public override string ToString() => 
        "IO pure map";
}

record IOPureMapAsync<A, B, C>(Func<A, B> Ff, ValueTask<A> Fa, Func<B, K<IO, C>> Next) : InvokeAsyncIO<C>
{
    public override IO<D> Map<D>(Func<C, D> f) => 
        new IOPureMapAsync<A, B, D>(Ff, Fa, x => Next(x).As().Map(f));

    public override IO<D> Bind<D>(Func<C, K<IO, D>> f) => 
        new IOPureMapAsync<A, B, D>(Ff, Fa, x => Next(x).As().Bind(f));

    public override IO<D> BindAsync<D>(Func<C, ValueTask<K<IO, D>>> f) => 
        new IOPureMapAsync<A, B, D>(Ff, Fa, x => Next(x).As().BindAsync(f));

    public override async ValueTask<IO<C>> Invoke(EnvIO envIO) =>
        Next(Ff(await Fa)).As();
    
    public override string ToString() => 
        "IO pure map async";
}
