using System;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

record IOBindAsync<A, B>(ValueTask<A> Value, Func<A, K<IO, B>> F) : InvokeAsyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) =>
        new IOBindAsync<A, C>(Value, x => F(x).As().Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) =>
        new IOBindAsync<A, C>(Value, x => F(x).As().Bind(f));

    public override IO<C> BindAsync<C>(Func<B, ValueTask<K<IO, C>>> f) =>
        new IOBindAsync<A, C>(Value, x => F(x).As().BindAsync(f));

    public override async ValueTask<IO<B>> Invoke(EnvIO envIO) =>
        F(await Value).As();
    
    public override string ToString() => 
        "IO bind async";
}

record IOBindAsync2<A, B>(ValueTask<A> Value, Func<A, ValueTask<K<IO, B>>> F) : InvokeAsyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) =>
        new IOBindAsync2<A, C>(Value, async x => (await F(x)).As().Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) =>
        new IOBindAsync2<A, C>(Value, async x => (await F(x)).As().Bind(f));

    public override IO<C> BindAsync<C>(Func<B, ValueTask<K<IO, C>>> f) => 
        new IOBindAsync2<A, C>(Value, async x => (await F(x)).As().BindAsync(f));

    public override async ValueTask<IO<B>> Invoke(EnvIO envIO) =>
        (await F(await Value)).As();
    
    public override string ToString() => 
        "IO bind async";
}
