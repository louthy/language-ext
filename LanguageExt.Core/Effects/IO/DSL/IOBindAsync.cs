using System;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

record IOBindAsync<A, B>(ValueTask<A> Value, Func<A, K<IO, B>> F) : InvokeAsyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) =>
        new IOBindMapAsync<A, B, C>(Value, F, f);

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) =>
        new IOBindBindAsync<A, B, C>(Value, F, f);

    public override IO<C> BindAsync<C>(Func<B, ValueTask<K<IO, C>>> f) =>
        new IOBindBindAsync2<A, B, C>(Value, x => new ValueTask<K<IO, B>>(F(x)), f);

    public override async ValueTask<IO<B>> Invoke(EnvIO envIO) =>
        F(await Value).As();
    
    public override string ToString() => 
        "IO bind async";
}

record IOBindBindAsync<A, B, C>(ValueTask<A> Value, Func<A, K<IO, B>> F, Func<B, K<IO, C>> G) : InvokeAsyncIO<C>
{
    public override IO<D> Map<D>(Func<C, D> f) =>
        new IOBindBindAsync<A, B, D>(Value, F, x => G(x).As().Map(f));

    public override IO<D> Bind<D>(Func<C, K<IO, D>> f) =>
        new IOBindBindAsync<A, B, D>(Value, F, x => G(x).As().Bind(f));

    public override IO<D> BindAsync<D>(Func<C, ValueTask<K<IO, D>>> f) =>
        new IOBindBindAsync<A, B, D>(Value, F, x => G(x).As().BindAsync(f));

    public override async ValueTask<IO<C>> Invoke(EnvIO envIO) =>
        F(await Value).Bind(G).As();
    
    public override string ToString() => 
        "IO bind bind async";
}

record IOBindMapAsync<A, B, C>(ValueTask<A> Value, Func<A, K<IO, B>> F, Func<B, C> G) : InvokeAsyncIO<C>
{
    public override IO<D> Map<D>(Func<C, D> f) =>
        new IOBindMapAsync<A, B, D>(Value, F, x => f(G(x)));

    public override IO<D> Bind<D>(Func<C, K<IO, D>> f) =>
        new IOBindBindAsync<A, B, D>(Value, F, x => f(G(x)));

    public override IO<D> BindAsync<D>(Func<C, ValueTask<K<IO, D>>> f) =>
        new IOBindBindAsync<A, B, D>(Value, F, x => IO.pure(G(x)).BindAsync(f));

    public override async ValueTask<IO<C>> Invoke(EnvIO envIO) =>
        F(await Value).Map(G).As();
    
    public override string ToString() => 
        "IO bind map async";
}

record IOBindAsync2<A, B>(ValueTask<A> Value, Func<A, ValueTask<K<IO, B>>> F) : InvokeAsyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) =>
        new IOBindAsync2<A, C>(Value, async x => (await F(x)).As().Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) =>
        new IOBindBindSync2<A, B, C>(Value, F, f);

    public override IO<C> BindAsync<C>(Func<B, ValueTask<K<IO, C>>> f) =>
        new IOBindBindAsync2<A, B, C>(Value, F, f);

    public override async ValueTask<IO<B>> Invoke(EnvIO envIO) =>
        (await F(await Value)).As();
    
    public override string ToString() => 
        "IO bind bind async";
}

record IOBindBindAsync2<A, B, C>(ValueTask<A> Value, Func<A, ValueTask<K<IO, B>>> F, Func<B, ValueTask<K<IO, C>>> G) : InvokeAsyncIO<C>
{
    public override IO<D> Map<D>(Func<C, D> f) =>
        new IOBindBindAsync2<A, B, D>(Value, F, async x => (await G(x)).Map(f));

    public override IO<D> Bind<D>(Func<C, K<IO, D>> f) =>
        new IOBindBindAsync2<A, B, D>(Value, F, async x => (await G(x)).Bind(f));

    public override IO<D> BindAsync<D>(Func<C, ValueTask<K<IO, D>>> f) => 
        new IOBindBindAsync2<A, B, D>(Value, F, async x => (await G(x)).As().BindAsync(f));

    public override async ValueTask<IO<C>> Invoke(EnvIO envIO) =>
        (await F(await Value)).As().BindAsync(G);
    
    public override string ToString() => 
        "IO bind bind async";
}

record IOBindBindSync2<A, B, C>(ValueTask<A> Value, Func<A, ValueTask<K<IO, B>>> F, Func<B, K<IO, C>> G) : InvokeAsyncIO<C>
{
    public override IO<D> Map<D>(Func<C, D> f) =>
        new IOBindBindSync2<A, B, D>(Value, F, x => G(x).Map(f));

    public override IO<D> Bind<D>(Func<C, K<IO, D>> f) =>
        new IOBindBindSync2<A, B, D>(Value, F, x => G(x).Bind(f));

    public override IO<D> BindAsync<D>(Func<C, ValueTask<K<IO, D>>> f) => 
        new IOBindBindSync2<A, B, D>(Value, F, x => G(x).As().BindAsync(f));

    public override async ValueTask<IO<C>> Invoke(EnvIO envIO) =>
        (await F(await Value)).As().Bind(G);
    
    public override string ToString() => 
        "IO bind bind async";
}
