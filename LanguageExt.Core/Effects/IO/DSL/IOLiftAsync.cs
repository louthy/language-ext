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

    public override async ValueTask<IO<B>> Invoke(EnvIO envIO) =>
        Next(await F(envIO)).As();
}
