using System;
using System.Threading.Tasks;

namespace LanguageExt.DSL;

static class IODsl
{
    public static IODsl<A> Lift<A>(Func<EnvIO, A> f) => new IOLiftSync<A>(f);
    public static IODsl<A> Lift<A>(Func<EnvIO, Task<A>> f) => new IOLiftAsync<A>(f);
}

abstract record IODsl<A>
{
    public abstract IODsl<B> Map<B>(Func<A, B> f);
}
