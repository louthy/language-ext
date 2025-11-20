using System;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.DSL;
    
record IOLiftAsync<A, B>(Func<EnvIO, Task<A>> F, Func<A, K<IO, B>> Next) : InvokeAsyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOLiftAsync<A, C>(F, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOLiftAsync<A, C>(F, x => Next(x).Bind(f));

    public override IO<C> BindAsync<C>(Func<B, ValueTask<K<IO, C>>> f) => 
        new IOLiftAsync<A, C>(F, x => Next(x).As().BindAsync(f));

    public override async ValueTask<IO<B>> Invoke(EnvIO envIO) =>
        Next(await F(envIO)).As();
    
    public override string ToString() => 
        "IO lift async";
}
    
record IOLiftVAsync<A, B>(Func<EnvIO, ValueTask<A>> F, Func<A, K<IO, B>> Next) : InvokeAsyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOLiftVAsync<A, C>(F, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOLiftVAsync<A, C>(F, x => Next(x).Bind(f));

    public override IO<C> BindAsync<C>(Func<B, ValueTask<K<IO, C>>> f) => 
        new IOLiftVAsync<A, C>(F, x => Next(x).As().BindAsync(f));

    public override async ValueTask<IO<B>> Invoke(EnvIO envIO) =>
        Next(await F(envIO)).As();
    
    public override string ToString() => 
        "IO lift vasync";
}
