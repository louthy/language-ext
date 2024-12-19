using System;
using System.Threading.Tasks;

namespace LanguageExt.DSL;
    
record IOLiftAsync<A>(Func<EnvIO, Task<A>> F) : DslInvokeIOAsync<A>
{
    public override IODsl<B> Map<B>(Func<A, B> f) =>
        new IOLiftAsync<B>(async x => f(await F(x).ConfigureAwait(false)));

    public override async ValueTask<A> Invoke(EnvIO envIO) =>
        await F(envIO);
}
