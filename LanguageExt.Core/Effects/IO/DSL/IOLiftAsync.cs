using System;
using System.Threading.Tasks;

namespace LanguageExt.DSL;
    
record IOLiftAsync<A>(Func<EnvIO, Task<A>> F) : IODslAsync<A>
{
    public override IODsl<B> Map<B>(Func<A, B> f) =>
        new IOLiftAsync<B>(async x => f(await F(x).ConfigureAwait(false)));
}
